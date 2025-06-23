using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildInviteDenyPacket : PacketWriter
    {
        private const int PacketNumber = 2105;

        /// <summary>
        /// Refuses an invitation to join a guild.
        /// </summary>
        /// <param name="targetName">Tamer name which denied</param>
        public GuildInviteDenyPacket(string targetName)
        {
            Type(PacketNumber);
            WriteString(targetName);
        }
    }
}