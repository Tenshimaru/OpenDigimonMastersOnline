using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class FriendNotFoundPacket : PacketWriter
    {
        private const int PacketNumber = 2407;

        /// <summary>
        /// Add a friend
        /// </summary>
        /// <param name="name">Friend character name</param>
        public FriendNotFoundPacket(string name)
        {
            Type(PacketNumber);
            WriteString(name);
        }
    }
}