using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class AvailableNamePacket : PacketWriter
    {
        private const int PacketNumber = 1302;

        public AvailableNamePacket(bool available)
        {
            Type(PacketNumber);

            WriteInt(Convert.ToInt32(available));
        }
    }
}
