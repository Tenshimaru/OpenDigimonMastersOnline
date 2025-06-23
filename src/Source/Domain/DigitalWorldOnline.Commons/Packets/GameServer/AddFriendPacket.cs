using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class AddFriendPacket : PacketWriter
    {
        private const int PacketNumber = 2401;

        /// <summary>
        /// Add a friend
        /// </summary>
        /// <param name="name">Friend character name</param>
        public AddFriendPacket(string name, byte status)
        {
            Type(PacketNumber);
            WriteByte(status);
            WriteString(name);
        }
    }
}