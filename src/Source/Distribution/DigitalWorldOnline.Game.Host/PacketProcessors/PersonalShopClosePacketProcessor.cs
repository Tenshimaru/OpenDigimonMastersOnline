using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using System;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.GameHost.EventsServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PersonalShopClosePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerShopClose;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public PersonalShopClosePacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender)
        {
            _mapServer = mapServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            _logger.Debug($"Getting parameters...");
            var requestType = packet.ReadByte();

            client.Tamer.UpdateCurrentCondition(ConditionEnum.Default);
            client.Send(new PersonalShopPacket());


            client.Tamer.Inventory.AddItems(client.Tamer.TamerShop.Items);
            client.Tamer.TamerShop.Clear();

            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemsCommand(client.Tamer.TamerShop));

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition)
                            .Serialize());
                    break;

                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition)
                            .Serialize());
                    break;

                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition)
                            .Serialize());
                    break;

                default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition)
                            .Serialize());
                    break;
            }
        }
    }
}