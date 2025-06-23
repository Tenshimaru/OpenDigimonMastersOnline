using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class SecondaryPasswordChangedPacket : PacketWriter
    {
        private const int PacketNumber = 9806;

        /// <summary>
        /// Secondary password has been changed succesfully
        /// </summary>
        public SecondaryPasswordChangedPacket()
        {
            Type(PacketNumber);
            WriteInt(0);
        }
    }
}
