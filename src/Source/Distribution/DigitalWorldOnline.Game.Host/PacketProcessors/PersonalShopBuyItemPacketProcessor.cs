using AutoMapper;
using DigitalWorldOnline.Application;
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
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PersonalShopPurchaseItemPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerShopBuy;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private bool hasItem = false;

        public PersonalShopPurchaseItemPacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            AssetsLoader assets,
            ILogger logger,
            IMapper mapper,
            ISender sender)
        {
            _mapServer = mapServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _mapper = mapper;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Information($"PersonalShopBuy");

            var shopHandler = packet.ReadInt();
            var shopSlot = packet.ReadInt();
            var shopSlotInDatabase = shopSlot - 1;
            var boughtItemId = packet.ReadInt();
            var boughtAmount = packet.ReadInt();

            packet.Skip(60);

            var boughtUnitPrice = packet.ReadInt64();

            _logger.Information($"Searching Personal Shop {shopHandler}...");
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            GameClient? PersonalShop = null;

            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    PersonalShop = _dungeonsServer.FindClientByTamerHandle(shopHandler);
                    break;

                case MapTypeEnum.Event:
                    PersonalShop = _eventServer.FindClientByTamerHandle(shopHandler);
                    break;

                case MapTypeEnum.Pvp:
                    PersonalShop = _pvpServer.FindClientByTamerHandle(shopHandler);
                    break;

                default:
                    PersonalShop = _mapServer.FindClientByTamerHandle(shopHandler);
                    break;
            }

            if (PersonalShop != null)
            {
                var boughtItem = PersonalShop.Tamer.TamerShop.Items.FirstOrDefault(x => x.Slot == shopSlotInDatabase);

                if (boughtItem == null)
                {
                    client.Send(new PersonalShopBuyItemPacket(TamerShopActionEnum.NoPartFound).Serialize());
                    return;
                }

                hasItem = false;
                
                if (boughtItem.Amount < boughtAmount)
                {
                    _logger.Information($"Amount exceeding the amount of the item in the consigned shop.");
                    client.Send(new PersonalShopBuyItemPacket(TamerShopActionEnum.TamerShopRequest));
                    return;
                }
                
                var totalValue = boughtItem.TamerShopSellPrice * boughtAmount;

                if (client.Tamer.Inventory.Bits < totalValue)
                {
                    //sistema de banimento permanente
                    var banProcessor = SingletonResolver.GetService<BanForCheating>();
                    var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name,
                        AccountBlockEnum.Permanent, "Cheating", client,
                        "You tried to buy an item with an invalid amount of bits using a cheat method, So be happy with ban!");

                    var chatPacket = new NoticeMessagePacket(banMessage).Serialize();
                    client.SendToAll(chatPacket);
                    return;
                }

                _logger.Information($"You spend {totalValue} bits");
                client.Tamer.Inventory.RemoveBits(totalValue);

                await _sender.Send(
                    new UpdateItemListBitsCommand(client.Tamer.Inventory.Id, client.Tamer.Inventory.Bits));

                /*var Slot = PersonalShop.Tamer.TamerShop.Items.FirstOrDefault(x => x.ItemId == boughtItemId);
                PersonalShop.Tamer.TamerShop.Items[Slot.Slot].Amount -= boughtAmount;*/

                // ------------------------------------------------------------------------------------

                var totalValuewithDescount = (totalValue / 100) * 98;

                _logger.Information($"Seller win {totalValuewithDescount} bits");
                PersonalShop.Tamer.Inventory.AddBits(totalValuewithDescount);

                /*PersonalShop.Send(new LoadInventoryPacket(PersonalShop.Tamer.Inventory, InventoryTypeEnum.Inventory)
                    .Serialize());*/
                await _sender.Send(new UpdateItemListBitsCommand(PersonalShop.Tamer.Inventory.Id,
                    PersonalShop.Tamer.Inventory.Bits));

                // ------------------------------------------------------------------------------------

                _logger.Debug(
                    $"Tentando comprar Item em {PersonalShop.Tamer.ShopName} {shopHandler} » {shopHandler} {shopSlotInDatabase} {boughtItemId} {boughtAmount} {boughtUnitPrice}.");
                var newItem = new ItemModel(boughtItem.ItemId, boughtAmount);
                newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == boughtItem.ItemId));

                _logger.Debug($"Removing consigned shop bought item...");
                PersonalShop.Tamer.TamerShop.RemoveOrReduceItems(((ItemModel)newItem.Clone()).GetList(), true);

                _logger.Debug($"Updating {PersonalShop.Tamer.Name} personal shop items...");
                await _sender.Send(new UpdateItemsCommand(PersonalShop.Tamer.TamerShop));

                PersonalShop.Tamer.TamerShop.CheckEmptyItems();

                _logger.Debug($"Adding bought item...");
                client.Tamer.Inventory.AddItems(((ItemModel)newItem.Clone()).GetList());

                _logger.Debug($"Updating item list...");

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

                _logger.Debug($"Sending consigned shop item list view packet...");
                
                client.Send(
                    new PersonalShopBuyItemPacket(TamerShopActionEnum.TamerShopWindow, shopSlot, boughtAmount)
                        .Serialize());
                PersonalShop.Send(new PersonalShopSellItemPacket(shopSlot, boughtAmount).Serialize());

                /*PersonalShop.Send(
                    new NoticeMessagePacket(
                        $"You sold {newItem.Amount}x {newItem.ItemInfo.Name} for {client.Tamer.Name}!"));*/

                foreach (var item in PersonalShop.Tamer.TamerShop.Items.Where(x => x.ItemId > 0))
                {
                    hasItem = true;
                }

                if (hasItem == false)
                {
                    PersonalShop.Send(new NoticeMessagePacket($"Your personal shop as been closed!"));
                    PersonalShop.Tamer.UpdateCurrentCondition(ConditionEnum.Default);
                    PersonalShop.Send(new PersonalShopPacket());
                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonsServer.BroadcastForTamerViewsAndSelf(PersonalShop.TamerId,
                                new SyncConditionPacket(PersonalShop.Tamer.GeneralHandler,
                                    PersonalShop.Tamer.CurrentCondition).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTamerViewsAndSelf(PersonalShop.TamerId,
                                new SyncConditionPacket(PersonalShop.Tamer.GeneralHandler,
                                    PersonalShop.Tamer.CurrentCondition).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTamerViewsAndSelf(PersonalShop.TamerId,
                                new SyncConditionPacket(PersonalShop.Tamer.GeneralHandler,
                                    PersonalShop.Tamer.CurrentCondition).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTamerViewsAndSelf(PersonalShop.TamerId,
                                new SyncConditionPacket(PersonalShop.Tamer.GeneralHandler,
                                    PersonalShop.Tamer.CurrentCondition).Serialize());
                            break;
                    }
                }
            }
            else
            {
                _logger.Error($"PersonalShop not found ...");
            }
        }
    }
}