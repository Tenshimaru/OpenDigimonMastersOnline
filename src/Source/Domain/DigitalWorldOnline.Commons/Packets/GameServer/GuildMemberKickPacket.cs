using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildMemberKickPacket : PacketWriter
    {
        private const int PacketNumber = 2106;

        /// <summary>
        /// Sends a notification for member kicked from guild.
        /// </summary>
        /// <param name="targetName">Target member name</param>
        public GuildMemberKickPacket(string targetName)
        {
            Type(PacketNumber);
            WriteString(targetName);
        }
    }
}