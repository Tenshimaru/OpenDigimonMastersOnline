using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
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
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConsignedShopPurchaseItemPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ConsignedShopPurchaseItem;

        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public ConsignedShopPurchaseItemPacketProcessor(AssetsLoader assets, ILogger logger, IMapper mapper,
            ISender sender)
        {
            _assets = assets;
            _logger = logger;
            _mapper = mapper;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Information($"ConsigmentShop Buy Packet 1518");

            var shopHandler = packet.ReadInt();
            var shopSlot = packet.ReadInt();
            var shopSlotInDatabase = shopSlot - 1;
            var boughtItemId = packet.ReadInt();
            var boughtAmount = packet.ReadInt();
            packet.Skip(60);
            var boughtUnitPrice = packet.ReadInt64();

            _logger.Information(
                $"boughtItemId: {boughtItemId} | boughtUnitPrice: {boughtUnitPrice} | boughtAmount: {boughtAmount}");

            _logger.Debug($"Searching consigned shop {shopHandler}...");
            var shop = _mapper.Map<ConsignedShop>(await _sender.Send(new ConsignedShopByHandlerQuery(shopHandler)));

            if (shop == null)
            {
                _logger.Error($"Consigned shop {shopHandler} not found...");
                client.Send(new UnloadConsignedShopPacket(shopHandler));
                return;
            }

            var seller =
                _mapper.Map<CharacterModel>(await _sender.Send(new CharacterAndItemsByIdQuery(shop.CharacterId)));

            if (seller == null)
            {
                _logger.Error($"Deleting consigned shop {shopHandler}...");
                await _sender.Send(new DeleteConsignedShopCommand(shopHandler));

                _logger.Error($"Consigned shop owner {shop.CharacterId} not found...");
                client.Send(new UnloadConsignedShopPacket(shopHandler));
                return;
            }

            if (seller.Name == client.Tamer.Name)
            {
                client.Send(new NoticeMessagePacket($"You cannot buy from the store itself!"));
                return;
            }

            var boughtItem = seller.ConsignedShopItems.Items.FirstOrDefault(x => x.Slot == shopSlotInDatabase);

            if (boughtItem == null)
            {
                client.Send(new ConsignedShopBoughtItemPacket(TamerShopActionEnum.NoPartFound, shopSlot, boughtAmount)
                    .Serialize());
                return;
            }

            if (boughtItem.Amount < boughtAmount)
            {
                _logger.Information($"Amount exceeding the amount of the item in the consigned shop.");
                client.Send(new ConsignedShopBoughtItemPacket(true));
                return;
            }
            
            var totalValue = boughtItem.TamerShopSellPrice * boughtAmount;

            _logger.Information(
                $"bought item price: {boughtItem.TamerShopSellPrice} | bought item amount: {boughtAmount} | total value: {totalValue} | slot {shopSlotInDatabase} | slot sent: {shopSlot}");

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

            _logger.Information($"Removing {totalValue} bits from {client.Tamer.Name}");
            client.Tamer.Inventory.RemoveBits(totalValue);

            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));

            var newItem = new ItemModel(boughtItem.ItemId, boughtAmount);
            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == boughtItem.ItemId));

            _logger.Information($"Adding items to {client.Tamer.Name}");
            client.Tamer.Inventory.AddItems(((ItemModel)newItem.Clone()).GetList());

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

            // ----------------------------------------------------------------------

            var sellerClient = client.Server.FindByTamerId(shop.CharacterId);

            if (sellerClient is { IsConnected: true })
            {
                _logger.Debug($"Sending system message packet {sellerClient.TamerId}...");
                var itemName = _assets.ItemInfo.FirstOrDefault(x => x.ItemId == boughtItem.ItemId)?.Name ?? "item";
                sellerClient.Send(
                    new NoticeMessagePacket($"You sold {boughtAmount}x {itemName} for {client.Tamer.Name}!"));

                _logger.Debug($"Adding {totalValue} bits to {sellerClient.TamerId} consigned warehouse...");
                sellerClient.Tamer.ConsignedWarehouse.AddBits(totalValue);

                _logger.Debug($"Updating {sellerClient.TamerId} consigned warehouse...");
                await _sender.Send(new UpdateItemListBitsCommand(sellerClient.Tamer.ConsignedWarehouse));

                _logger.Debug($"Removing consigned shop bought item...");
                seller.ConsignedShopItems.RemoveOrReduceItems(((ItemModel)newItem.Clone()).GetList(), true);

                _logger.Debug($"Updating {seller.Id} consigned shop items...");
                await _sender.Send(new UpdateItemsCommand(seller.ConsignedShopItems));
            }
            else
            {
                _logger.Information($"Adding {totalValue} bits to {seller.Name} consigned warehouse...");
                seller.ConsignedWarehouse.AddBits(totalValue);

                _logger.Debug($"Removing consigned shop bought item...");
                seller.ConsignedShopItems.RemoveOrReduceItems(((ItemModel)newItem.Clone()).GetList(), true);

                await _sender.Send(new UpdateItemListBitsCommand(seller.ConsignedWarehouse));

                await _sender.Send(new UpdateItemsCommand(seller.ConsignedShopItems));
            }

            if (seller.ConsignedShopItems.Count == 0)
            {
                _logger.Debug($"Deleting consigned shop {shopHandler}...");
                await _sender.Send(new DeleteConsignedShopCommand(shopHandler));

                _logger.Debug($"Sending unload consigned shop packet {shopHandler}...");
                client.Send(new UnloadConsignedShopPacket(shopHandler));

                _logger.Debug($"Sending consigned shop close packet...");
                sellerClient?.Send(new ConsignedShopClosePacket());
            }
            else
            {
                seller.ConsignedShopItems.Items.ForEach(item =>
                {
                    item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));
                });
            }
            
            _logger.Debug($"Sending consigned shop bought item packet...");
            client.Send(new ConsignedShopBoughtItemPacket(TamerShopActionEnum.TamerShopWindow, shopSlot, boughtAmount));
        }
    }
}