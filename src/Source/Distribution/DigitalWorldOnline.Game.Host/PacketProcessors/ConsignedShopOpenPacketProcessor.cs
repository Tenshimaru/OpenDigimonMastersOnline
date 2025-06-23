using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConsignedShopOpenPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ConsignedShopOpen;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public ConsignedShopOpenPacketProcessor(
            MapServer mapServer,
            EventServer eventServer,
            DungeonsServer dungeonsServer,
            PvpServer pvpServer,
            AssetsLoader assets,
            ILogger logger,
            ISender sender)
        {
            _mapServer = mapServer;
            _eventServer = eventServer;
            _dungeonsServer = dungeonsServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Verbose($"--- ConsigmentShop Open Packet 1516 ---");

            var posX = packet.ReadInt();
            var posY = packet.ReadInt();
            packet.Skip(4);
            var shopName = packet.ReadString();
            packet.Skip(9);
            var sellQuantity = packet.ReadInt();

            _logger.Verbose(
                $"Shop Location: Map {client.Tamer.Location.MapId} ({posX}, {posY}), ShopName: {shopName}, Items Amount: {sellQuantity}\n");

            List<ItemModel> sellList = new(sellQuantity);


            //_logger.Information($"-------------------------------------\n");

            for (int i = 0; i < sellQuantity; i++)
            {
                var itemId = packet.ReadInt();
                var itemAmount = packet.ReadInt();

                Console.WriteLine($"Item Index: {i} | ItemId: {itemId} | ItemAmount: {itemAmount}");

                // Adiciona um novo item à lista
                var sellItem = new ItemModel(itemId, itemAmount);

                packet.Skip(64);

                var price = packet.ReadInt64();
                sellItem.SetSellPrice(price);

                packet.Skip(8);
                sellList.Add(sellItem);

                Console.WriteLine($"Item Index: {i} | Price: {price}\n");
            }


            _logger.Information($"sell items count: {sellList.Count}");

            //_logger.Information($"-------------------------------------");

            foreach (var item in sellList)
            {
                //_logger.Information($"item: {item.ItemId} and amount: {item.Amount}");
                item.SetItemInfo(_assets.ItemInfo.First(x => x.ItemId == item.ItemId));

                var itemsCount = sellList.Count(x =>
                    x.ItemId == item.ItemId && x.TamerShopSellPrice != item.TamerShopSellPrice);

                if (itemsCount > 0)
                {
                   // _logger.Error($"Tamer {client.Tamer.Name} tryed to add 2 items of same id with different price!");
                    client.Send(new DisconnectUserPacket("You cant add 2 items of same id with different price!")
                        .Serialize());
                    return;
                }

                var HasQuanty = client.Tamer.Inventory.CountItensById(item.ItemId);

                if (item.Amount > HasQuanty)
                {
                    //sistema de banimento permanente
                    var banProcessor = SingletonResolver.GetService<BanForCheating>();
                    var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name,
                        AccountBlockEnum.Permanent, "Cheating", client, "You tried to open consigned shop with amount you don't have using a cheat method, So be happy with ban!");

                    var chatPacket = new NoticeMessagePacket(banMessage);
                    client.Send(chatPacket); // Envia a mensagem no chat

                    // client.Send(new DisconnectUserPacket($"YOU HAVE BEEN PERMANENTLY BANNED").Serialize());

                    return;
                }
            }

            _logger.Verbose($"Updating consigned shop item list...");
            client.Tamer.ConsignedShopItems.AddItems(sellList.Clone(), true);
            await _sender.Send(new UpdateItemsCommand(client.Tamer.ConsignedShopItems));

            _logger.Verbose($"Updating tamer inventory item list...");
            client.Tamer.Inventory.RemoveOrReduceItems(sellList.Clone());
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

            _logger.Verbose($"Creating consigned shop...");
            var newShop = ConsignedShop.Create(client.TamerId, shopName, posX, posY, client.Tamer.Location.MapId,
                client.Tamer.Channel, client.Tamer.ShopItemId);

            var Id = await _sender.Send(new CreateConsignedShopCommand(newShop));

            newShop.SetId(Id.Id);
            newShop.SetGeneralHandler(Id.GeneralHandler);

            _logger.Verbose($"Sending consigned shop load packet...");

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new LoadConsignedShopPacket(newShop).Serialize());
                    break;

                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new LoadConsignedShopPacket(newShop).Serialize());
                    break;

                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new LoadConsignedShopPacket(newShop).Serialize());
                    break;

                default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new LoadConsignedShopPacket(newShop).Serialize());
                    break;
            }


            _logger.Verbose($"Sending personal shop close packet...");
            client.Tamer.UpdateShopItemId(0);
            client.Send(new PersonalShopPacket(TamerShopActionEnum.CloseWindow, client.Tamer.ShopItemId));
            client.Tamer.RestorePreviousCondition();

            _logger.Verbose($"Sending sync in condition packet...");

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