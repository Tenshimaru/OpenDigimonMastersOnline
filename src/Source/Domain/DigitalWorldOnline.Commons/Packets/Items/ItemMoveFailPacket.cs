using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ItemMoveFailPacket : PacketWriter
    {
        private const int PacketNumber = 3905;

        /// <summary>
        /// Returns fail on item movimentation.
        /// </summary>
        /// <param name="originSlot">Origin slot of the item.</param>
        /// <param name="destinationSlot">Destination slot of the item.</param>
        public ItemMoveFailPacket(short originSlot, short destinationSlot)
        {
            Type(PacketNumber);
            WriteShort(originSlot);
            WriteShort(destinationSlot);
        }
    }
}