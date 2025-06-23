using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class PickItemPacket : PacketWriter
    {
        private const int PacketNumber = 3910;

        /// <summary>
        /// Try to pick an item from the ground.
        /// </summary>
        /// <param name="appearanceHandler">The tamer appearance handler</param>
        /// <param name="item">The received item</param>
        public PickItemPacket(int appearanceHandler, ItemModel item)
        {
            Type(PacketNumber);
            WriteInt(appearanceHandler);
            WriteInt(item.ItemId);
            WriteShort((short)item.Amount);
            WriteByte(0);
            WriteInt(0);
        }
    }
}