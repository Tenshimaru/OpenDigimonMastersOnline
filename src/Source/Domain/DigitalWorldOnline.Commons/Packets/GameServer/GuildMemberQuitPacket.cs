using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildMemberQuitPacket : PacketWriter
    {
        private const int PacketNumber = 2107;

        /// <summary>
        /// Sends a notification for member leaving guild.
        /// </summary>
        /// <param name="targetName">Target member name</param>
        public GuildMemberQuitPacket(string targetName)
        {
            Type(PacketNumber);
            WriteString(targetName);
        }
    }
}