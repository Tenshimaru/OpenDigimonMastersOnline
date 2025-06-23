using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.PersonalShop;

using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConsignedWarehouseRetrievePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ConsignedWarehouseRetrieve;

        private readonly ILogger _logger;
        private readonly ISender _sender;

        public ConsignedWarehouseRetrievePacketProcessor(
            ILogger logger,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var items = client.Tamer.ConsignedWarehouse.Items.Clone();
            var bits = client.Tamer.ConsignedWarehouse.Bits;

            _logger.Debug($"Updating consigned warehouse...");
            client.Tamer.ConsignedWarehouse.RemoveOrReduceItems(items.Clone());
            client.Tamer.ConsignedWarehouse.RemoveBits(bits);

            _logger.Debug($"Updating tamer inventory...");
            client.Tamer.Inventory.AddItems(items.Clone());
            client.Tamer.Inventory.AddBits(bits);

            _logger.Debug($"Sending load inventory packet...");
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            _logger.Debug($"Sending load consigned shop warehouse packet...");
            client.Send(new LoadConsignedShopWarehousePacket(client.Tamer.ConsignedWarehouse));

            _logger.Debug($"Sending consigned shop warehouse item retrieve packet...");
            client.Send(new ConsignedShopWarehouseItemRetrievePacket());

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemsCommand(client.Tamer.ConsignedWarehouse));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.ConsignedWarehouse));
        }
    }
}