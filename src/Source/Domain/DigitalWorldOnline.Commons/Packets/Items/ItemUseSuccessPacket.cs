using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ItemUseSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 3901;

        /// <summary>
        /// Returns success on item consume.
        /// </summary>
        /// <param name="handler">Source handler value.</param>
        /// <param name="slot">Target item slot.</param>
        public ItemUseSuccessPacket(int handler, short slot)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteShort(slot);
        }
    }
}