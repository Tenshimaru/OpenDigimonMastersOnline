using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class FriendInformationPacket : PacketWriter
    {
        private const int PacketNumber = 3129;

        /// <summary>
        /// Load the tamer's friend information.
        /// </summary>
        public FriendInformationPacket()
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteShort(9);
        }
    }
}