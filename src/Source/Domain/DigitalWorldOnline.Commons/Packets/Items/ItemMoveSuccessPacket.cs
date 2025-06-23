using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ItemMoveSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 3904;

        /// <summary>
        /// Returns success on item movimentation.
        /// </summary>
        /// <param name="originSlot">Origin slot of the item.</param>
        /// <param name="destinationSlot">Destination slot of the item.</param>
        public ItemMoveSuccessPacket(short originSlot, short destinationSlot)
        {
            Type(PacketNumber);
            WriteShort(originSlot);
            WriteShort(destinationSlot);
        }
    }
}