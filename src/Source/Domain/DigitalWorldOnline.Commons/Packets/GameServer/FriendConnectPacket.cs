using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class FriendConnectPacket : PacketWriter
    {
        private const int PacketNumber = 2408;

        /// <summary>
        /// Sends a notification upon a friend connects to the game.
        /// </summary>
        /// <param name="name">Friend character name</param>
        public FriendConnectPacket(string name)
        {
            Type(PacketNumber);
            WriteString(name);
        }
    }
}