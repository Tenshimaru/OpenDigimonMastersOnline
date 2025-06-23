using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.GameHost.EventsServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TamerShopOpenPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerShopOpen;

        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public TamerShopOpenPacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer,
            PvpServer pvpServer, AssetsLoader assets, ILogger logger, ISender sender)
        {
            _mapServer = mapServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Verbose($"--- PersonalShop Open Packet 1511 ---");

            var shopName = packet.ReadString();

            packet.Skip(1);

            var sellQuantity = packet.ReadInt();

            _logger.Verbose(
                $"Shop Location: Map {client.Tamer.Location.MapId} ShopName: {shopName}, Items Amount: {sellQuantity}\n");
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            List<ItemModel> sellList = new(sellQuantity);

            //_logger.Information($"-------------------------------------\n");

            for (int i = 0; i < sellQuantity; i++)
            {
                var itemId = packet.ReadInt();
                var itemAmount = packet.ReadInt();

                _logger.Verbose($"Item Index: {i} | ItemId: {itemId} | ItemAmount: {itemAmount}");

                var sellItem = new ItemModel(itemId, itemAmount);

                packet.Skip(64);

                var price = packet.ReadInt64();
                sellItem.SetSellPrice(price);

                packet.Skip(8);

                //_logger.Information("--- Pacote Bruto ---");
                //string packetHex = BitConverter.ToString(packetData);
                //_logger.Information(packetHex);
                //_logger.Information("--------------------");

                sellList.Add(sellItem);

                _logger.Verbose($"Item Index: {i} | Price: {price}\n");
                //break;
            }

            //_logger.Information($"-------------------------------------");

            foreach (var item in sellList)
            {
                item.SetItemInfo(_assets.ItemInfo.First(x => x.ItemId == item.ItemId));
                foreach (var item2 in sellList)
                {
                    if (item2.ItemId == item.ItemId && item2.TamerShopSellPrice != item.TamerShopSellPrice)
                    {
                        _logger.Error(
                            $"Tamer {client.Tamer.Name} tryed to add 2 items of same id with different price!");
                        client.Send(new DisconnectUserPacket("You cant add 2 items of same id with different price!")
                            .Serialize());
                        return;
                    }
                }

                var HasQuanty = client.Tamer.Inventory.CountItensById(item.ItemId);
                if (item.Amount > HasQuanty)
                {
                    //sistema de banimento permanente
                    var banProcessor = SingletonResolver.GetService<BanForCheating>();
                    var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name,
                        AccountBlockEnum.Permanent, "Cheating", client, "You tried to open a shop invalid amount of item using a cheat method, So be happy with ban!");

                    var chatPacket = new NoticeMessagePacket(banMessage).Serialize();
                    client.SendToAll(chatPacket); // Envia a mensagem no chat

                    // client.Send(new DisconnectUserPacket($"YOU HAVE BEEN PERMANENTLY BANNED").Serialize());
                    return;
                }

                _logger.Debug($"{item.ItemId} {item.Amount} {item.TamerShopSellPrice}");
            }

            _logger.Debug($"Updating tamer shop item list...");
            client.Tamer.TamerShop.AddItems(sellList.Clone());
            await _sender.Send(new UpdateItemsCommand(client.Tamer.TamerShop));

            _logger.Debug($"Updating tamer inventory item list...");
            client.Tamer.Inventory.RemoveOrReduceItems(sellList.Clone());
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

            client.Tamer.UpdateCurrentCondition(ConditionEnum.TamerShop);
            client.Tamer.UpdateShopName(shopName);

            _logger.Debug($"Sending sync in condition packet...");
            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition, shopName)
                            .Serialize());
                    break;

                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition, shopName)
                            .Serialize());
                    break;

                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition, shopName)
                            .Serialize());
                    break;

                default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition, shopName)
                            .Serialize());
                    break;
            }

            client.Send(new PersonalShopItemsViewPacket(client.Tamer.TamerShop, client.Tamer.ShopName));

            _logger.Debug($"Sending tamer shop open packet...");
            client.Send(new PersonalShopPacket(client.Tamer.ShopItemId));
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());

            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory.Id, client.Tamer.Inventory.Bits));
        }
    }
}