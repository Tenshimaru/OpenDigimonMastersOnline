using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class SplitItemPacket : PacketWriter
    {
        private const int PacketNumber = 3907;

        /// <summary>
        /// Splits an item.
        /// </summary>
        /// <param name="originSlot">Origin item slot.</param>
        /// <param name="destinationSlot">Destination item slot.</param>
        /// <param name="amountToSplit">Amount splitted to the new slot.</param>
        public SplitItemPacket(short originSlot, short destinationSlot, short amountToSplit)
        {
            Type(PacketNumber);
            WriteShort(originSlot);
            WriteShort(destinationSlot);
            WriteShort(amountToSplit);
        }
    }
}
