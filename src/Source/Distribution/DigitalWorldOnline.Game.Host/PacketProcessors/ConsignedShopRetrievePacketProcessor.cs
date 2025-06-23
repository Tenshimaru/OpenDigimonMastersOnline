using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConsignedShopRetrievePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ConsignedShopRetrieve;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public ConsignedShopRetrievePacketProcessor(
            AssetsLoader assets,
            MapServer mapServer,
            EventServer eventServer,
            DungeonsServer dungeonsServer,
            PvpServer pvpServer,
            ILogger logger,
            IMapper mapper,
            ISender sender)
        {
            _assets = assets;
            _mapServer = mapServer;
            _eventServer = eventServer;
            _dungeonsServer = dungeonsServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _mapper = mapper;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var shop = _mapper.Map<ConsignedShop>(await _sender.Send(new ConsignedShopByTamerIdQuery(client.TamerId)));

            if (shop != null)
            {
                var items = client.Tamer.ConsignedShopItems.Items.Clone();

                items.ForEach(item =>
                {
                    item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));
                });

                _logger.Debug($"Removing consigned shop items...");
                client.Tamer.ConsignedShopItems.RemoveOrReduceItems(items.Clone());

                _logger.Debug($"Deleting consigned shop {shop.GeneralHandler}...");
                await _sender.Send(new DeleteConsignedShopCommand(shop.GeneralHandler));

                _logger.Debug($"Adding consigned shop items to consigned shop warehouse...");
                client.Tamer.ConsignedWarehouse.AddItems(items.Clone());

                _logger.Debug($"Broadcasting unload consigned shop packet...");
                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadConsignedShopPacket(shop).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadConsignedShopPacket(shop).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadConsignedShopPacket(shop).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UnloadConsignedShopPacket(shop).Serialize());
                        break;
                }

                await _sender.Send(new UpdateItemsCommand(client.Tamer.ConsignedShopItems));
                await _sender.Send(new UpdateItemsCommand(client.Tamer.ConsignedWarehouse));
            }

            _logger.Debug($"Sending consigned shop close packet...");
            client.Send(new ConsignedShopClosePacket());
        }
    }
}