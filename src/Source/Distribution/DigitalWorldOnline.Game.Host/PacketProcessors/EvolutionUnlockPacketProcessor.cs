using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class EvolutionUnlockPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.EvolutionUnlock;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;
        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;

        public EvolutionUnlockPacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger,
            MapServer mapServer,
            EventServer eventServer,
            DungeonsServer dungeonsServer,
            PvpServer pvpServer)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;
            _mapServer = mapServer;
            _eventServer = eventServer;
            _dungeonsServer = dungeonsServer;
            _pvpServer = pvpServer;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var evoIndex = packet.ReadInt() - 1;
            var itemSlot = packet.ReadShort();

            var evolution = client.Partner.Evolutions[evoIndex];

            var evoInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?.Lines
                .FirstOrDefault(x => x.Type == evolution.Type);

            //_logger.Information($"evoIndex: {evoIndex}");
            //_logger.Information($"EvolutionID: {evolution.Id} | EvolutionType: {evolution.Type} | EvolutionUnlocked: {evolution.Unlocked}");

            if (evoInfo == null)
            {
                _logger.Error($"Invalid evolution info for type {client.Partner.BaseType} and line {evolution.Type}.");
                client.Send(new SystemMessagePacket(
                    $"Invalid evolution info for type {client.Partner.BaseType} and line {evolution.Type}."));
                return;
            }

            if (itemSlot <= 150)
            {
                var inventoryItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);

                var itemInfo = _assets.EvolutionsArmor.FirstOrDefault(x => x.ItemId == inventoryItem.ItemId);

                byte success = 1;
                short result = 0;

                client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, itemInfo.Amount, inventoryItem.Slot);

                var rand = new Random();

                if (itemInfo.Chance >= rand.Next(100))
                {
                    success = 0;
                    result = 1;

                    evolution.Unlock();

                    _logger.Verbose($"Character {client.TamerId} unlocked evolutionArmor {evolution.Type} " +
                                    $"for {client.Partner.Id} ({client.Partner.BaseType}) with ItemId {itemInfo.ItemId} x{itemInfo.Amount}.");

                    client.Send(new EvolutionArmorUnlockedPacket(result, success));

                    await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    await _sender.Send(new UpdateEvolutionCommand(evolution));
                }
                else
                {
                    client.Send(new EvolutionArmorUnlockedPacket(result, success));
                    await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                }
            }
            else
            {
                var itemSection = evoInfo.UnlockItemSection;
                var requiredAmount = evoInfo.UnlockItemSectionAmount;

                var inventoryItems = client.Tamer.Inventory.FindItemsBySection(itemSection);

                var Rare = false;
                var ItemId = 0;

                if (inventoryItems.Any())
                {
                    while (requiredAmount > 0)
                    {
                        foreach (var inventoryItem in inventoryItems)
                        {
                            var scanAsset = _assets.ScanDetail.FirstOrDefault(scan =>
                                scan.Rewards.Any(reward => reward.ItemId == inventoryItem.ItemId));

                            if (inventoryItem.Amount > requiredAmount)
                            {
                                client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, requiredAmount,
                                    inventoryItem.Slot);
                                requiredAmount = 0;
                            }
                            else
                            {
                                requiredAmount -= inventoryItem.Amount;
                                client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, inventoryItem.Amount,
                                    inventoryItem.Slot);
                            }

                            if (requiredAmount > 0)
                            {
                                continue;
                            }

                            var scanReward =
                                scanAsset?.Rewards.FirstOrDefault(x => x.ItemId == inventoryItem.ItemId);

                            if (scanReward is { Rare: true })
                            {
                                Rare = true;
                                ItemId = scanReward.ItemId;
                            }

                            break;
                        }
                    }
                }
                else
                {
                    _logger.Error($"No items found with section {itemSection} for character {client.TamerId}.");
                    client.Send(new SystemMessagePacket($"Invalid evolution item with section {itemSection}."));
                    return;
                }

                evolution.Unlock();
                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                if (Rare)
                {
                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonsServer.BroadcastForChannel(client.Tamer.Channel,
                                new NeonMessagePacket(NeonMessageTypeEnum.Evolution, client.Tamer.Name, ItemId,
                                    client.Tamer.Partner.CurrentType).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForChannel(client.Tamer.Channel,
                                new NeonMessagePacket(NeonMessageTypeEnum.Evolution, client.Tamer.Name, ItemId,
                                    client.Tamer.Partner.CurrentType).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForChannel(client.Tamer.Channel,
                                new NeonMessagePacket(NeonMessageTypeEnum.Evolution, client.Tamer.Name, ItemId,
                                    client.Tamer.Partner.CurrentType).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForChannel(client.Tamer.Channel,
                                new NeonMessagePacket(NeonMessageTypeEnum.Evolution, client.Tamer.Name, ItemId,
                                    client.Tamer.Partner.CurrentType).Serialize());
                            break;
                    }
                }

                _logger.Verbose($"Character {client.TamerId} unlocked evolution {evolution.Type} " +
                                $"for {client.Partner.Id} ({client.Partner.BaseType}) with item section {itemSection} x{evoInfo.UnlockItemSectionAmount}.");

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new UpdateEvolutionCommand(evolution));

                // --------------------------------------------------------------------------------------------------

                var encyclopedia =
                    client.Tamer.Encyclopedia.FirstOrDefault(x => x.DigimonEvolutionId == evoInfo.EvolutionId);

                //_logger.Information($"Encyclopedia is: {encyclopedia.Id}, evolution id: {evoInfo.EvolutionId}, count: {client.Tamer.Encyclopedia.Count}");
                //_logger.Information($"Encyclopedia is: {client.Tamer.Encyclopedia.Last()?.Id}");

                if (encyclopedia != null)
                {
                    var encyclopediaEvolution = encyclopedia.Evolutions.First(x => x.DigimonBaseType == evolution.Type);

                    encyclopediaEvolution.Unlock();

                    await _sender.Send(new UpdateCharacterEncyclopediaEvolutionsCommand(encyclopediaEvolution));

                    int LockedEncyclopediaCount = encyclopedia.Evolutions.Count(x => x.IsUnlocked == false);

                    if (LockedEncyclopediaCount <= 0)
                    {
                        encyclopedia.SetRewardAllowed();
                        await _sender.Send(new UpdateCharacterEncyclopediaCommand(encyclopedia));
                    }
                }
            }
        }
    }
}