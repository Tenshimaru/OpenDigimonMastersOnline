using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class FriendDisconnectPacket : PacketWriter
    {
        private const int PacketNumber = 2409;

        /// <summary>
        /// Sends a notification upon a friend disconnects from the game.
        /// </summary>
        /// <param name="name">Friend character name</param>
        public FriendDisconnectPacket(string name)
        {
            Type(PacketNumber);
            WriteString(name);
        }
    }
}