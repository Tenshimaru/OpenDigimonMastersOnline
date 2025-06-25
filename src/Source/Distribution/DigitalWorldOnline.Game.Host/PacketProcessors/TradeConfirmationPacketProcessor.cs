using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;
using System.Collections.Concurrent;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TradeConfirmationPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TradeConfirmation;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        // Thread-safe dictionary for trade confirmation locks
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _tradeConfirmationLocks = new();

        public TradeConfirmationPacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer, ILogger logger, ISender sender)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            // Check if client is in a valid trade
            if (!client.Tamer.TradeCondition || client.Tamer.TargetTradeGeneralHandle == 0)
            {
                _logger.Warning($"[TRADE] {client.Tamer.Name} attempted to confirm trade without being in a valid trade");
                return;
            }

            GameClient? targetClient;

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapConfig!.Type)
            {
                case MapTypeEnum.Dungeon:
                    targetClient = _dungeonServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                case MapTypeEnum.Event:
                    targetClient = _eventServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                case MapTypeEnum.Pvp:
                    targetClient = _pvpServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                default:
                    targetClient = _mapServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;
            }

            // Check if target client still exists and is in trade
            if (targetClient == null || !targetClient.Tamer.TradeCondition)
            {
                _logger.Warning($"[TRADE] Target client not found or not in trade for {client.Tamer.Name}");
                InvalidTrade(client, targetClient);
                return;
            }

            // Create unique lock key based on player IDs (ordered to prevent deadlock)
            var tradeKey = client.TamerId < targetClient.TamerId
                ? $"{client.TamerId}_{targetClient.TamerId}"
                : $"{targetClient.TamerId}_{client.TamerId}";

            var tradeLock = _tradeConfirmationLocks.GetOrAdd(tradeKey, _ => new SemaphoreSlim(1, 1));

            try
            {
                await tradeLock.WaitAsync();

                // Re-verify that trade is still valid after acquiring lock
                if (!client.Tamer.TradeCondition || !targetClient.Tamer.TradeCondition)
                {
                    _logger.Warning($"[TRADE] Trade was cancelled while waiting for confirmation lock");
                    InvalidTrade(client, targetClient);
                    return;
                }

                client.Send(new TradeConfirmationPacket(client.Tamer.GeneralHandler));
                targetClient.Send(new TradeConfirmationPacket(client.Tamer.GeneralHandler));
                client.Tamer.SetTradeConfirm(true);

                await ProcessTradeConfirmation(client, targetClient);
            }
            finally
            {
                tradeLock.Release();

                // Clean up old locks to prevent memory leaks
                if (_tradeConfirmationLocks.Count > 1000)
                {
                    var keysToRemove = _tradeConfirmationLocks.Keys.Take(100).ToList();
                    foreach (var key in keysToRemove)
                    {
                        if (_tradeConfirmationLocks.TryRemove(key, out var lockToDispose))
                        {
                            lockToDispose.Dispose();
                        }
                    }
                }
            }
        }

        private async Task ProcessTradeConfirmation(GameClient client, GameClient targetClient)
        {

            if (client.Tamer.TradeConfirm && targetClient.Tamer.TradeConfirm)
            {
                // Integrity validations before trade
                if (!ValidateTradeIntegrity(client, targetClient))
                {
                    InvalidTrade(client, targetClient);
                    return;
                }

                // Check inventory space
                if (client.Tamer.Inventory.TotalEmptySlots < targetClient.Tamer.TradeInventory.Count)
                {
                    _logger.Warning($"[TRADE] {client.Tamer.Name} doesn't have enough inventory space for trade");
                    InvalidTrade(client, targetClient);
                    return;
                }
                else if (targetClient.Tamer.Inventory.TotalEmptySlots < client.Tamer.TradeInventory.Count)
                {
                    _logger.Warning($"[TRADE] {targetClient.Tamer.Name} doesn't have enough inventory space for trade");
                    InvalidTrade(client, targetClient);
                    return;
                }

                // Detailed trade log for audit
                var firstTamerItems = client.Tamer.TradeInventory.EquippedItems.Select(x => $"{x.ItemId} x{x.Amount}");
                var secondTamerItems = targetClient.Tamer.TradeInventory.EquippedItems.Select(x => $"{x.ItemId} x{x.Amount}");
                var firstTamerBits = client.Tamer.TradeInventory.Bits;
                var secondTamerBits = targetClient.Tamer.TradeInventory.Bits;

                _logger.Information($"[TRADE_AUDIT] {client.Tamer.Name} (ID: {client.TamerId}) traded [{string.Join('|', firstTamerItems)}] and {firstTamerBits} bits with {targetClient.Tamer.Name} (ID: {targetClient.TamerId})");
                _logger.Information($"[TRADE_AUDIT] {targetClient.Tamer.Name} (ID: {targetClient.TamerId}) traded [{string.Join('|', secondTamerItems)}] and {secondTamerBits} bits with {client.Tamer.Name} (ID: {client.TamerId})");

                // Execute trade atomically
                await ExecuteTradeTransaction(client, targetClient);
            }
        }

        private bool ValidateTradeIntegrity(GameClient client, GameClient targetClient)
        {
            try
            {
                // Check if both players still have the items they're trying to trade
                foreach (var tradeItem in client.Tamer.TradeInventory.EquippedItems)
                {
                    var actualAmount = client.Tamer.Inventory.CountItensById(tradeItem.ItemId);
                    if (actualAmount < tradeItem.Amount)
                    {
                        _logger.Error($"[TRADE_INTEGRITY] {client.Tamer.Name} doesn't have enough {tradeItem.ItemInfo.Name} (has {actualAmount}, trying to trade {tradeItem.Amount})");
                        return false;
                    }
                }

                foreach (var tradeItem in targetClient.Tamer.TradeInventory.EquippedItems)
                {
                    var actualAmount = targetClient.Tamer.Inventory.CountItensById(tradeItem.ItemId);
                    if (actualAmount < tradeItem.Amount)
                    {
                        _logger.Error($"[TRADE_INTEGRITY] {targetClient.Tamer.Name} doesn't have enough {tradeItem.ItemInfo.Name} (has {actualAmount}, trying to trade {tradeItem.Amount})");
                        return false;
                    }
                }

                // Check bits
                if (client.Tamer.TradeInventory.Bits > client.Tamer.Inventory.Bits)
                {
                    _logger.Error($"[TRADE_INTEGRITY] {client.Tamer.Name} doesn't have enough bits (has {client.Tamer.Inventory.Bits}, trying to trade {client.Tamer.TradeInventory.Bits})");
                    return false;
                }

                if (targetClient.Tamer.TradeInventory.Bits > targetClient.Tamer.Inventory.Bits)
                {
                    _logger.Error($"[TRADE_INTEGRITY] {targetClient.Tamer.Name} doesn't have enough bits (has {targetClient.Tamer.Inventory.Bits}, trying to trade {targetClient.Tamer.TradeInventory.Bits})");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[TRADE_INTEGRITY] Error validating trade integrity between {client.Tamer.Name} and {targetClient.Tamer.Name}");
                return false;
            }
        }

        private async Task ExecuteTradeTransaction(GameClient client, GameClient targetClient)
        {
            try
            {
                // Backup inventories for rollback in case of error
                var clientInventoryBackup = client.Tamer.Inventory.EquippedItems.Clone();
                var targetInventoryBackup = targetClient.Tamer.Inventory.EquippedItems.Clone();
                var clientBitsBackup = client.Tamer.Inventory.Bits;
                var targetBitsBackup = targetClient.Tamer.Inventory.Bits;

                #region ITEM TRADE

                // Remove items from original inventories
                if (client.Tamer.TradeInventory.Count > 0)
                {
                    if (!client.Tamer.Inventory.RemoveOrReduceItems(client.Tamer.TradeInventory.EquippedItems.Clone()))
                    {
                        _logger.Error($"[TRADE_ERROR] Failed to remove items from {client.Tamer.Name}'s inventory");
                        throw new InvalidOperationException("Failed to remove items from client inventory");
                    }
                }

                if (targetClient.Tamer.TradeInventory.Count > 0)
                {
                    if (!targetClient.Tamer.Inventory.RemoveOrReduceItems(targetClient.Tamer.TradeInventory.EquippedItems.Clone()))
                    {
                        _logger.Error($"[TRADE_ERROR] Failed to remove items from {targetClient.Tamer.Name}'s inventory");
                        // Rollback client inventory - restore items manually
                        RestoreInventoryItems(client.Tamer.Inventory, clientInventoryBackup);
                        throw new InvalidOperationException("Failed to remove items from target inventory");
                    }
                }

                // Add items to destination inventories
                if (targetClient.Tamer.TradeInventory.Count > 0)
                {
                    if (!client.Tamer.Inventory.AddItems(targetClient.Tamer.TradeInventory.EquippedItems.Clone()))
                    {
                        _logger.Error($"[TRADE_ERROR] Failed to add items to {client.Tamer.Name}'s inventory");
                        // Rollback both inventories - restore items manually
                        RestoreInventoryItems(client.Tamer.Inventory, clientInventoryBackup);
                        RestoreInventoryItems(targetClient.Tamer.Inventory, targetInventoryBackup);
                        throw new InvalidOperationException("Failed to add items to client inventory");
                    }
                }

                if (client.Tamer.TradeInventory.Count > 0)
                {
                    if (!targetClient.Tamer.Inventory.AddItems(client.Tamer.TradeInventory.EquippedItems.Clone()))
                    {
                        _logger.Error($"[TRADE_ERROR] Failed to add items to {targetClient.Tamer.Name}'s inventory");
                        // Rollback all changes - restore items manually
                        RestoreInventoryItems(client.Tamer.Inventory, clientInventoryBackup);
                        RestoreInventoryItems(targetClient.Tamer.Inventory, targetInventoryBackup);
                        throw new InvalidOperationException("Failed to add items to target inventory");
                    }
                }

                #endregion

                #region BITS TRADE

                if (client.Tamer.TradeInventory.Bits >= 1)
                {
                    client.Tamer.Inventory.RemoveBits(client.Tamer.TradeInventory.Bits);
                    targetClient.Tamer.Inventory.AddBits(client.Tamer.TradeInventory.Bits);
                }

                if (targetClient.Tamer.TradeInventory.Bits >= 1)
                {
                    targetClient.Tamer.Inventory.RemoveBits(targetClient.Tamer.TradeInventory.Bits);
                    client.Tamer.Inventory.AddBits(targetClient.Tamer.TradeInventory.Bits);
                }

                #endregion

                // Clear trade state
                targetClient.Tamer.ClearTrade();
                client.Tamer.ClearTrade();

                // Send final confirmation
                client.Send(new TradeFinalConfirmationPacket(client.Tamer.GeneralHandler));
                targetClient.Send(new TradeFinalConfirmationPacket(client.Tamer.GeneralHandler));

                // Save to database
                await _sender.Send(new UpdateItemsCommand(targetClient.Tamer.Inventory));
                await _sender.Send(new UpdateItemListBitsCommand(targetClient.Tamer.Inventory));

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));

                // Update inventories on client
                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
                targetClient.Send(new LoadInventoryPacket(targetClient.Tamer.Inventory, InventoryTypeEnum.Inventory));

                _logger.Information($"[TRADE_SUCCESS] Trade completed successfully between {client.Tamer.Name} and {targetClient.Tamer.Name}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[TRADE_ERROR] Error executing trade between {client.Tamer.Name} and {targetClient.Tamer.Name}");
                InvalidTrade(client, targetClient);
                throw;
            }
        }

        private static void InvalidTrade(GameClient client, GameClient? targetClient)
        {
            try
            {
                if (targetClient != null)
                {
                    client.Send(new TradeCancelPacket(targetClient.Tamer.GeneralHandler));
                    targetClient.Send(new TradeCancelPacket(client.Tamer.GeneralHandler));
                    targetClient.Tamer.ClearTrade();
                }
                else
                {
                    client.Send(new TradeCancelPacket(0)); // Handle null target
                }

                client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                client.Tamer.ClearTrade();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent further issues
                Serilog.Log.Error(ex, "[TRADE_ERROR] Error in InvalidTrade method");
            }
        }

        /// <summary>
        /// Manually restore inventory items from backup
        /// </summary>
        private void RestoreInventoryItems(ItemListModel inventory, List<ItemModel> backup)
        {
            try
            {
                // Clear current inventory
                inventory.Clear();

                // Restore items from backup
                foreach (var backupItem in backup)
                {
                    if (backupItem.ItemId > 0 && backupItem.Amount > 0)
                    {
                        var restoredItem = (ItemModel)backupItem.Clone();
                        inventory.AddItemWithSlot(restoredItem, backupItem.Slot);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[TRADE_ERROR] Failed to restore inventory items from backup");
            }
        }
    }

}

