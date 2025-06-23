using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class FriendChangeNamePacket : PacketWriter
    {
        private const int PacketNumber = 2410;

        /// <summary>
        /// Sends a notification upon a friend disconnects from the game.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="name">Friend character name</param>
        /// <param name="isFoe"></param>
        public FriendChangeNamePacket(string oldName, string name, bool isFoe = false)
        {
            Type(PacketNumber);
            WriteByte(Convert.ToByte(isFoe));
            WriteString(oldName);
            WriteString(name);
        }
    }
}