using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PersonalShopPreparePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PersonalShopPrepare;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public PersonalShopPreparePacketProcessor(
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

            //_logger.Information($"--- Personal/Consignment Shop Prepare Packet 1510 ---\n");

            var requestType = (TamerShopActionEnum)packet.ReadInt();
            var itemSlot = packet.ReadShort();

            //_logger.Information($"---------------------------------------");
            //_logger.Information($"RequestType: {(int)requestType} - {requestType} | ItemSlot: {itemSlot}");

            if (requestType == TamerShopActionEnum.TamerShopRequest ||
                requestType == TamerShopActionEnum.ConsignedShopRequest)
            {
                var itemUsed = client.Tamer.Inventory.FindItemBySlot(itemSlot);

                //_logger.Information($"ItemUsed: {itemUsed.ItemId} - {itemUsed.ItemInfo?.Name} | Section: {itemUsed.ItemInfo?.Section}\n");

                if (itemUsed.ItemInfo?.Section != 7601)
                {
                    if (client.Tamer.ConsignedShop != null)
                    {
                        _logger.Debug($"Opening New PersonalShop !!");
                        client.Send(new PersonalShopPacket(TamerShopActionEnum.TamerShopRequest, 0));
                        return;
                    }
                }
            }

            var itemId = 0;
            var action = TamerShopActionEnum.CloseWindow;

            switch (requestType)
            {
                case TamerShopActionEnum.TamerShopRequest:
                case TamerShopActionEnum.TamerShopWithItensCloseRequest:
                    action = TamerShopActionEnum.TamerShopWindow;
                    break;
                case TamerShopActionEnum.ConsignedShopRequest:
                    action = TamerShopActionEnum.ConsignedShopWindow;
                    break;
                case TamerShopActionEnum.CloseShopRequest:
                case TamerShopActionEnum.TamerShopWithoutItensCloseRequest:
                    action = TamerShopActionEnum.CloseWindow;
                    break;
            }

            //_logger.Information($"---------------------------------------");

            if (itemSlot <= GeneralSizeEnum.InventoryMaxSlot.GetHashCode())
            {
                itemId = client.Tamer.Inventory.Items[itemSlot]?.ItemId ?? -1;
                _logger.Debug($"Shop item id {itemId}");

                if (itemId == 0) return;
            }

            client.Tamer.UpdateShopItemId(itemId);

            _logger.Verbose($"Sending sync condition packet...");

            if (requestType == TamerShopActionEnum.CloseShopRequest)
                client.Tamer.RestorePreviousCondition();
            else
                client.Tamer.UpdateCurrentCondition(ConditionEnum.PreparingShop);

            if (requestType == TamerShopActionEnum.TamerShopWithoutItensCloseRequest)
                client.Tamer.UpdateCurrentCondition(ConditionEnum.Default);

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

            _logger.Verbose($"Sending personal shop packet with action {action}...");

            if (requestType == TamerShopActionEnum.TamerShopWithItensCloseRequest)
            {
                client.Tamer.UpdateCurrentCondition(ConditionEnum.Default);

                client.Tamer.Inventory.AddItems(client.Tamer.TamerShop.Items);
                client.Tamer.TamerShop.Clear();

                client.Send(new PersonalShopPacket());

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new UpdateItemsCommand(client.Tamer.TamerShop));

                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());
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
            else if (requestType == TamerShopActionEnum.TamerShopWithoutItensCloseRequest)
            {
                client.Send(new PersonalShopPacket());
            }
            else
            {
                client.Send(new PersonalShopPacket(action, client.Tamer.ShopItemId));
            }

            //_logger.Information($"---------------------------------------");
        }
    }
}