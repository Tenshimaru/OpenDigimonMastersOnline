using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class IncubatorClosePacket : PacketWriter
    {
        private const int PacketNumber = 3948;

        /// <summary>
        /// Closes the incubator window.
        /// </summary>
        public IncubatorClosePacket()
        {
            Type(PacketNumber);
            WriteInt(0); // Simple acknowledgment
        }
    }
}
