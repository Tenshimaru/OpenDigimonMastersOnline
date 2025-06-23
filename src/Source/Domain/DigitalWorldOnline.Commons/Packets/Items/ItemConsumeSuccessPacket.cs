using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ItemConsumeSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 3901;

        public ItemConsumeSuccessPacket(int handler, short slot)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteShort(slot);
        }
    }
}