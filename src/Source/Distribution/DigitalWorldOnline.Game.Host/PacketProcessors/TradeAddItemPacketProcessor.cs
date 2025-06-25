using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using DigitalWorldOnline.Game.Services;
using MediatR;
using Serilog;
using System.Collections.Concurrent;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TradeAddItemPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TradeAddItem;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly SecurityAuditService _securityAudit;
        private readonly IntegrityValidationService _integrityValidation;

        // Thread-safe dictionary para locks de troca por par de jogadores
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _tradeLocks = new();

        public TradeAddItemPacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer, ILogger logger, ISender sender, SecurityAuditService securityAudit, IntegrityValidationService integrityValidation)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _securityAudit = securityAudit;
            _integrityValidation = integrityValidation;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var inventorySlot = packet.ReadShort();
            var amount = packet.ReadShort();
            var slotAtual = client.Tamer.TradeInventory.EquippedItems.Count;

            // Basic input validations
            if (inventorySlot < 0 || amount <= 0)
            {
                _securityAudit.LogTradeAttempt(client, null, "ADD_ITEM_INVALID_PARAMS", false, $"slot={inventorySlot}, amount={amount}");
                _logger.Warning($"[TRADE] Invalid parameters from {client.Tamer.Name}: slot={inventorySlot}, amount={amount}");
                return;
            }

            // Validate player state
            var playerValidation = _integrityValidation.ValidatePlayerState(client, "TRADE_ADD_ITEM");
            if (!playerValidation.IsValid)
            {
                _securityAudit.LogTradeAttempt(client, null, "ADD_ITEM_INVALID_STATE", false, playerValidation.ErrorMessage);
                client.Send(new ChatMessagePacket(playerValidation.ErrorMessage, ChatTypeEnum.Notice, "System"));
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

            // Check if client is in a valid trade
            if (!client.Tamer.TradeCondition || client.Tamer.TargetTradeGeneralHandle == 0)
            {
                _securityAudit.LogTradeAttempt(client, null, "ADD_ITEM_NOT_IN_TRADE", false, "Not in valid trade state");
                _logger.Warning($"[TRADE] {client.Tamer.Name} attempted to add item without being in a valid trade");
                return;
            }

            var Item = client.Tamer.Inventory.FindItemBySlot(inventorySlot);

            // Validate item integrity
            var itemValidation = _integrityValidation.ValidateItemIntegrity(Item);
            if (!itemValidation.IsValid)
            {
                _securityAudit.LogTradeAttempt(client, null, "ADD_ITEM_INVALID_ITEM", false, $"slot={inventorySlot}, error={itemValidation.ErrorMessage}");
                _logger.Warning($"[TRADE] {client.Tamer.Name} attempted to add invalid item from slot {inventorySlot}: {itemValidation.ErrorMessage}");
                return;
            }

            // Validate inventory operation
            var inventoryValidation = _integrityValidation.ValidateInventoryOperation(client, Item, amount, "TRADE_ADD");
            if (!inventoryValidation.IsValid)
            {
                _securityAudit.LogTradeAttempt(client, null, "ADD_ITEM_INVENTORY_VIOLATION", false, inventoryValidation.ErrorMessage);
                client.Send(new ChatMessagePacket("Invalid item operation.", ChatTypeEnum.Notice, "System"));
                return;
            }

            // Create unique lock key based on player IDs (ordered to prevent deadlock)
            var tradeKey = client.TamerId < client.Tamer.TargetTradeGeneralHandle
                ? $"{client.TamerId}_{client.Tamer.TargetTradeGeneralHandle}"
                : $"{client.Tamer.TargetTradeGeneralHandle}_{client.TamerId}";

            var tradeLock = _tradeLocks.GetOrAdd(tradeKey, _ => new SemaphoreSlim(1, 1));

            try
            {
                await tradeLock.WaitAsync();

                // Re-verify that trade is still valid after acquiring lock
                if (!client.Tamer.TradeCondition)
                {
                    _logger.Warning($"[TRADE] Trade was cancelled while waiting for lock for {client.Tamer.Name}");
                    return;
                }

                // Strict inventory quantity verification
                var actualItemAmount = client.Tamer.Inventory.CountItensById(Item.ItemId);
                if (actualItemAmount < amount || Item.Amount < amount)
                {
                    await HandleInvalidTradeAmount(client, targetClient, Item, amount, actualItemAmount);
                    return;
                }
            }
            finally
            {
                tradeLock.Release();

                // Clean up old locks to prevent memory leaks
                if (_tradeLocks.Count > 1000)
                {
                    var keysToRemove = _tradeLocks.Keys.Take(100).ToList();
                    foreach (var key in keysToRemove)
                    {
                        if (_tradeLocks.TryRemove(key, out var lockToDispose))
                        {
                            lockToDispose.Dispose();
                        }
                    }
                }
            }

            // Check if item was already added to prevent duplication (improved)
            if (client.Tamer.TradeInventory.EquippedItems.Any(i => i.ItemId == Item.ItemId && i.Slot == inventorySlot))
            {
                _logger.Warning($"[TRADE] {client.Tamer.Name} attempted to add duplicate item {Item.ItemInfo.Name} from slot {inventorySlot} in trade.");
                client.Send(new ChatMessagePacket("Item already added to trade.", ChatTypeEnum.Notice, "System"));
                return;
            }

            // Check trade item limit
            if (client.Tamer.TradeInventory.EquippedItems.Count >= 8) // Assuming limit of 8 items
            {
                _logger.Warning($"[TRADE] {client.Tamer.Name} attempted to add item beyond trade limit");
                client.Send(new ChatMessagePacket("Trade inventory is full.", ChatTypeEnum.Notice, "System"));
                return;
            }

            // Select empty slot for new item
            var EmptSlot = client.Tamer.TradeInventory.GetEmptySlot;
            if (EmptSlot == -1)
            {
                client.Send(new ChatMessagePacket("No empty slot available in trade inventory.", ChatTypeEnum.Notice, "System"));
                return;
            }

            // Clone and add item to trade
            var NewItem = (ItemModel)Item.Clone();
            NewItem.Amount = amount;

            if (!client.Tamer.TradeInventory.AddItemTrade(NewItem))
            {
                _logger.Error($"[TRADE] Failed to add item {Item.ItemInfo.Name} to trade inventory for {client.Tamer.Name}");
                client.Send(new ChatMessagePacket("Failed to add item to trade.", ChatTypeEnum.Notice, "System"));
                return;
            }

            // Log action for audit
            _logger.Information($"[TRADE] {client.Tamer.Name} added {amount}x {Item.ItemInfo.Name} (ID: {Item.ItemId}) to trade");

            // Send update packets to both clients
            client.Send(new TradeAddItemPacket(client.Tamer.GeneralHandler, NewItem.ToArray(), (byte)EmptSlot, inventorySlot));
            targetClient?.Send(new TradeAddItemPacket(client.Tamer.GeneralHandler, NewItem.ToArray(), (byte)EmptSlot, inventorySlot));

            // Lock inventory until trade confirmation
            targetClient?.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));
            client.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));
        }

        private async Task HandleInvalidTradeAmount(GameClient client, GameClient? targetClient, ItemModel item, short requestedAmount, int actualAmount)
        {
            targetClient?.Tamer.ClearTrade();
            targetClient?.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));
            targetClient?.Send(new TradeCancelPacket(client.Tamer.GeneralHandler));

            // Detailed log for investigation
            _logger.Error($"[TRADE_SECURITY] {client.Tamer.Name} attempted invalid trade: requested {requestedAmount}x {item.ItemInfo.Name}, but has {actualAmount}x (item amount: {item.Amount})");

            // Smarter ban system - check if it's really cheating or network error
            var timeSinceLastTrade = DateTime.Now - client.Tamer.LastTradeAttempt;
            if (timeSinceLastTrade.TotalSeconds < 5) // Multiple attempts in short time = suspicious
            {
                var banProcessor = SingletonResolver.GetService<BanForCheating>();
                var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name,
                    AccountBlockEnum.Permanent, "Cheating", client,
                    "Multiple invalid trade attempts detected - possible duplication exploit");

                var chatPacket = new NoticeMessagePacket(banMessage).Serialize();
                client.SendToAll(chatPacket);
            }
            else
            {
                // First attempt - just cancel and warn
                client.Send(new ChatMessagePacket("Invalid item amount for trade.", ChatTypeEnum.Notice, "System"));
                client.Tamer.LastTradeAttempt = DateTime.Now;
            }
        }

    }
}

