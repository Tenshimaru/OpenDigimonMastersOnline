using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Items;

using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemRemovePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ItemRemove;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ItemRemovePacketProcessor(
            ISender sender,
            ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var slot = packet.ReadShort();
            var posx = packet.ReadInt();
            var posy = packet.ReadInt();
            var amount = packet.ReadShort();

            var targetItem = client.Tamer.Inventory.FindItemBySlot(slot);

            if (targetItem?.ItemId > 0)
            {
                //if (targetItem.ItemInfo?.ItemBoundType == 0)
                //{
                //    var temp = (ItemModel)targetItem.Clone();
                //
                //    if (client.Tamer.Inventory.RemoveOrReduceItems(targetItem.GetList()))
                //    {
                //        var drop = _dropManager.CreateItemDrop(
                //            client.TamerId,
                //            client.Tamer.GeneralHandler,
                //            temp.ItemId,
                //            temp.Amount,
                //            temp.Amount,
                //            client.Tamer.Location.MapId,
                //            client.Tamer.Location.X,
                //            client.Tamer.Location.Y,
                //            true
                //        );
                //
                //        _mapServer.AddMapDrop(drop);
                //
                //        _logger.Verbose($"Tamer {client.TamerId} throw away {targetItem.ItemId} x{targetItem.Amount} at {client.Tamer.Location.MapId}.");
                //    }
                //}
                //else
                //{

                _logger.Verbose($"Character {client.TamerId} deleted {targetItem.ItemId} x{amount} at {client.Tamer.Location.MapId} x{posx} y{posy}.");
                
                client.Tamer.Inventory.RemoveOrReduceItem(targetItem, amount, slot);
                //}
                
                await _sender.Send(new UpdateItemCommand(targetItem));

                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
            }
        }
    }
}