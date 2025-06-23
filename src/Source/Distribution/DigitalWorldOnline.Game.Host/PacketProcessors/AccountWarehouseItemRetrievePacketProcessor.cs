using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class AccountWarehouseItemRetrievePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.RetrivieAccountWarehouseItem;

        private readonly ILogger _logger;
        private readonly ISender _sender;

        public AccountWarehouseItemRetrievePacketProcessor(ILogger logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var itemSlot = packet.ReadShort();

            var targetItem = client.Tamer.AccountCashWarehouse.FindItemBySlot(itemSlot);

            if (targetItem != null)
            {
                var newItem = (ItemModel)targetItem.Clone();

                newItem.SetItemId(targetItem.ItemId);
                newItem.SetAmount(targetItem.Amount);
                newItem.SetItemInfo(targetItem.ItemInfo);

                if (newItem.IsTemporary)
                    newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                if (client.Tamer.Inventory.AddItemGiftStorage(newItem))
                {
                    client.Tamer.AccountCashWarehouse.RemoveItem(targetItem, itemSlot);

                    client.Tamer.AccountCashWarehouse.Sort();

                    client.Send(new AccountWarehouseItemRetrievePacket(newItem, itemSlot)); //  Aqui pode ser que o newItem esta errado

                    client.Send(new LoadAccountWarehousePacket(client.Tamer.AccountCashWarehouse));
                    client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

                    await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    await _sender.Send(new UpdateItemsCommand(client.Tamer.AccountCashWarehouse));
                }
                else
                {
                    _logger.Warning($"Failed to add item in Inventory!! Tamer {client.Tamer.Name} dont have free slots");
                }
            }
            else
            {
                _logger.Error($"AccountWarehouse Item not found !!");
            }
        }
    }
}