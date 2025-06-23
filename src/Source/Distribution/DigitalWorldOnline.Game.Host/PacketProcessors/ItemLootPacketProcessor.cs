using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Enums.Party;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using System.IO;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.GameHost.EventsServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemLootPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.LootItem;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ItemLootPacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            AssetsLoader assets,
            ISender sender,
            ILogger logger)
        {
            _partyManager = partyManager;
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var dropHandler = packet.ReadInt();
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            Drop? targetDrop;
            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    targetDrop = _dungeonServer.GetDrop(client.Tamer.Location.MapId, dropHandler, client.TamerId);
                    break;

                case MapTypeEnum.Event:
                    targetDrop = _eventServer.GetDrop(client.Tamer.Location.MapId, dropHandler, client.TamerId);
                    break;

                case MapTypeEnum.Pvp:
                    targetDrop = _pvpServer.GetDrop(client.Tamer.Location.MapId, dropHandler, client.TamerId);
                    break;

                default:
                    targetDrop = _mapServer.GetDrop(client.Tamer.Location.MapId, dropHandler, client.TamerId);
                    break;
            }

            if (targetDrop == null)
            {
                client.Send(UtilitiesFunctions.GroupPackets(
                    new SystemMessagePacket($"Drop has no data.").Serialize(),
                    new PickItemFailPacket(PickItemFailReasonEnum.Unknow).Serialize(),
                    new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()
                ));
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadDropsPacket(dropHandler).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadDropsPacket(dropHandler).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadDropsPacket(dropHandler).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadDropsPacket(dropHandler).Serialize());
                        break;
                }

                return;
            }

            if (targetDrop.Collected)
                return;

            var dropClone = (ItemModel)targetDrop.DropInfo.Clone();
            var party = _partyManager.FindParty(client.TamerId);

            var LootType = false;
            var OrderType = PartyLootShareTypeEnum.Normal;

            if (party != null)
            {
                switch (party.LootType)
                {
                    case PartyLootShareTypeEnum.Free:
                        LootType = true;
                        break;
                    case PartyLootShareTypeEnum.Order:
                        OrderType = PartyLootShareTypeEnum.Order;
                        break;
                }
            }

            if (targetDrop.OwnerId == client.TamerId || LootType)
            {
                targetDrop.SetCollected(true);

                if (targetDrop.BitsDrop)
                {
                    if (party != null)
                    {
                        var partyClients = new List<GameClient>();

                        var partyMembersInMap = party.Members.Values
                            .Where(x => x.Id != client.TamerId && x.Location.MapId == client.Tamer.Location.MapId)
                            .Select(x => x.Id);
                        foreach (var partyMemberId in partyMembersInMap)
                        {
                            GameClient? partyMemberClient;
                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    partyMemberClient = _dungeonServer.FindClientByTamerId(partyMemberId);
                                    break;

                                case MapTypeEnum.Event:
                                    partyMemberClient = _eventServer.FindClientByTamerId(partyMemberId);
                                    break;

                                case MapTypeEnum.Pvp:
                                    partyMemberClient = _pvpServer.FindClientByTamerId(partyMemberId);
                                    break;

                                default:
                                    partyMemberClient = _mapServer.FindClientByTamerId(partyMemberId);
                                    break;
                            }

                            if (partyMemberClient == null)
                                continue;

                            partyClients.Add(partyMemberClient);
                        }

                        partyClients.Add(client);

                        var bitsAmount = dropClone.Amount / partyClients.Count;

                        foreach (var partyClient in partyClients)
                        {
                            partyClient.Tamer.Inventory.AddBits(bitsAmount);

                            await UpdateItemListBits(partyClient);

                            partyClient.Send(new PickBitsPacket(partyClient.Tamer.GeneralHandler, bitsAmount));
                        }

                        _logger.Verbose($"Character {client.TamerId} looted bits x{bitsAmount} for party {party.Id}.");
                    }
                    else
                    {
                        client.Send(new PickBitsPacket(client.Tamer.GeneralHandler, dropClone.Amount));

                        client.Tamer.Inventory.AddBits(dropClone.Amount);

                        _logger.Verbose($"Character {client.TamerId} looted bits x{dropClone.Amount}.");
                    }

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UnloadDropsPacket(targetDrop).Serialize());

                            _dungeonServer.RemoveDrop(targetDrop, client.TamerId);
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UnloadDropsPacket(targetDrop).Serialize());

                            _eventServer.RemoveDrop(targetDrop, client.TamerId);
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UnloadDropsPacket(targetDrop).Serialize());

                            _pvpServer.RemoveDrop(targetDrop, client.TamerId);
                            break;

                        default:
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UnloadDropsPacket(targetDrop).Serialize());

                            _mapServer.RemoveDrop(targetDrop, client.TamerId);
                            break;
                    }


                    await UpdateItemListBits(client);

                    client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
                }
                else
                {
                    var itemInfo = _assets.ItemInfo.FirstOrDefault(x => x.ItemId == targetDrop.DropInfo.ItemId);

                    if (itemInfo == null)
                    {
                        _logger.Warning($"Item has no data info {targetDrop.DropInfo.ItemId}.");
                        client.Send(
                            UtilitiesFunctions.GroupPackets(
                                new PickItemFailPacket(PickItemFailReasonEnum.Unknow).Serialize(),
                                new SystemMessagePacket($"Item has no data info {targetDrop.DropInfo.ItemId}.")
                                    .Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory)
                                    .Serialize()
                            )
                        );
                        return;
                    }

                    targetDrop.SetCollected(true);

                    var aquireClone = (ItemModel)targetDrop.DropInfo.Clone();

                    targetDrop.DropInfo.SetItemInfo(itemInfo);
                    dropClone.SetItemInfo(itemInfo);
                    aquireClone.SetItemInfo(itemInfo);

                    if (OrderType != PartyLootShareTypeEnum.Order)
                    {
                        if (client.Tamer.Inventory.AddItem(aquireClone))
                        {
                            await UpdateItemsTask(client);

                            _logger.Verbose(
                                $"Character {client.TamerId} looted item {dropClone.ItemId} x{dropClone.Amount}.");
                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        UtilitiesFunctions.GroupPackets(
                                            new PickItemPacket(client.Tamer.AppearenceHandler, dropClone).Serialize(),
                                            new UnloadDropsPacket(targetDrop).Serialize()
                                        )
                                    );

                                    _dungeonServer.RemoveDrop(targetDrop, client.TamerId);
                                    break;

                                case MapTypeEnum.Event:
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        UtilitiesFunctions.GroupPackets(
                                            new PickItemPacket(client.Tamer.AppearenceHandler, dropClone).Serialize(),
                                            new UnloadDropsPacket(targetDrop).Serialize()
                                        )
                                    );

                                    _eventServer.RemoveDrop(targetDrop, client.TamerId);
                                    break;

                                case MapTypeEnum.Pvp:
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        UtilitiesFunctions.GroupPackets(
                                            new PickItemPacket(client.Tamer.AppearenceHandler, dropClone).Serialize(),
                                            new UnloadDropsPacket(targetDrop).Serialize()
                                        )
                                    );

                                    _pvpServer.RemoveDrop(targetDrop, client.TamerId);
                                    break;

                                default:
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        UtilitiesFunctions.GroupPackets(
                                            new PickItemPacket(client.Tamer.AppearenceHandler, dropClone).Serialize(),
                                            new UnloadDropsPacket(targetDrop).Serialize()
                                        )
                                    );

                                    _mapServer.RemoveDrop(targetDrop, client.TamerId);
                                    break;
                            }


                            if (party != null)
                            {
                                foreach (var memberId in party.GetMembersIdList())
                                {
                                    var targetMessage = _mapServer.FindClientByTamerId(memberId);
                                    var targetDungeon = _dungeonServer.FindClientByTamerId(memberId);
                                    var targetEvent = _eventServer.FindClientByTamerId(memberId);
                                    var targetPvp = _pvpServer.FindClientByTamerId(memberId);

                                    targetMessage?.Send(new PartyLootItemPacket(client.Tamer, aquireClone).Serialize());

                                    targetDungeon?.Send(new PartyLootItemPacket(client.Tamer, aquireClone).Serialize());

                                    targetEvent?.Send(new PartyLootItemPacket(client.Tamer, aquireClone).Serialize());

                                    targetPvp?.Send(new PartyLootItemPacket(client.Tamer, aquireClone).Serialize());
                                }
                            }
                        }
                        else
                        {
                            targetDrop.SetCollected(false);

                            _logger.Verbose(
                                $"Character {client.TamerId} has not enough free space to loot drop handler {dropHandler} " +
                                $"with item {targetDrop.DropInfo.ItemId} x{targetDrop.DropInfo.Amount}.");

                            client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                        }
                    }
                    else
                    {
                        var randomIndex = new Random().Next(party.Members.Count + 1);
                        var sortedPlayer = party.Members.ElementAt(randomIndex).Value;
                        var diceNumber = new Random().Next(0, 255);

                        var sortedClient = _mapServer.FindClientByTamerId(sortedPlayer.Id);

                        if (sortedClient != null)
                        {
                            if (sortedClient.Tamer.Inventory.AddItem(aquireClone))
                            {
                                await UpdateItemsTask(sortedClient);

                                _logger.Verbose(
                                    $"Character {client.TamerId} looted item {dropClone.ItemId} x{dropClone.Amount}.");


                                switch (mapConfig?.Type)
                                {
                                    case MapTypeEnum.Dungeon:
                                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            UtilitiesFunctions.GroupPackets(
                                                new PickItemPacket(client.Tamer.AppearenceHandler, aquireClone)
                                                    .Serialize(),
                                                new UnloadDropsPacket(targetDrop).Serialize()
                                            )
                                        );

                                        _dungeonServer.RemoveDrop(targetDrop, client.TamerId);
                                        break;

                                    case MapTypeEnum.Event:
                                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            UtilitiesFunctions.GroupPackets(
                                                new PickItemPacket(client.Tamer.AppearenceHandler, aquireClone)
                                                    .Serialize(),
                                                new UnloadDropsPacket(targetDrop).Serialize()
                                            )
                                        );

                                        _eventServer.RemoveDrop(targetDrop, client.TamerId);
                                        break;

                                    case MapTypeEnum.Pvp:
                                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            UtilitiesFunctions.GroupPackets(
                                                new PickItemPacket(client.Tamer.AppearenceHandler, aquireClone)
                                                    .Serialize(),
                                                new UnloadDropsPacket(targetDrop).Serialize()
                                            )
                                        );

                                        _pvpServer.RemoveDrop(targetDrop, client.TamerId);
                                        break;

                                    default:
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            UtilitiesFunctions.GroupPackets(
                                                new PickItemPacket(client.Tamer.AppearenceHandler, aquireClone)
                                                    .Serialize(),
                                                new UnloadDropsPacket(targetDrop).Serialize()
                                            )
                                        );

                                        _mapServer.RemoveDrop(targetDrop, client.TamerId);

                                        break;
                                }

                                if (party != null)
                                {
                                    foreach (var memberId in party.GetMembersIdList())
                                    {
                                        var targetMessage = _mapServer.FindClientByTamerId(memberId);
                                        var targetDungeon = _dungeonServer.FindClientByTamerId(memberId);
                                        var targetEvent = _eventServer.FindClientByTamerId(memberId);
                                        var targetPvp = _pvpServer.FindClientByTamerId(memberId);

                                        targetMessage?.Send(new PartyLootItemPacket(sortedClient.Tamer,
                                            aquireClone, (byte)diceNumber, client.Tamer.Name).Serialize());

                                        targetDungeon?.Send(new PartyLootItemPacket(sortedClient.Tamer,
                                            aquireClone, (byte)diceNumber, client.Tamer.Name).Serialize());

                                        targetEvent?.Send(new PartyLootItemPacket(sortedClient.Tamer,
                                            aquireClone, (byte)diceNumber, client.Tamer.Name).Serialize());

                                        targetPvp?.Send(new PartyLootItemPacket(sortedClient.Tamer,
                                            aquireClone, (byte)diceNumber, client.Tamer.Name).Serialize());
                                    }
                                }
                            }
                            else
                            {
                                _logger.Verbose(
                                    $"Character {client.TamerId} has not enough free space to loot drop handler {dropHandler} " +
                                    $"with item {targetDrop.DropInfo.ItemId} x{targetDrop.DropInfo.Amount}.");

                                sortedClient.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.Verbose(
                    $"Character {client.TamerId} has no right to loot drop handler {dropHandler} with item {targetDrop.DropInfo.ItemId} x{targetDrop.DropInfo.Amount}.");
                client.Send(new PickItemFailPacket(PickItemFailReasonEnum.NotTheOwner));
            }
        }

        public async Task UpdateItemsTask(GameClient client)
        {
            await Task.Run(async () =>
            {
                // Coloque seu código aqui para enviar o comando
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            });
        }

        public async Task UpdateItemListBits(GameClient client)
        {
            await Task.Run(async () =>
            {
                // Coloque seu código aqui para enviar o comando
                await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            });
        }
    }
}