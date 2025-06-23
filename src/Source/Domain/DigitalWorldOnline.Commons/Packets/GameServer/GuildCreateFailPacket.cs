using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildCreateFailPacket : PacketWriter
    {
        private const int PacketNumber = 2101;

        /// <summary>
        /// Failure on creating a new guild.
        /// </summary>
        /// <param name="guildLeaderName">The name of the tamer who's creating the guild</param>
        /// <param name="guildName">The guild name</param>
        public GuildCreateFailPacket(string guildLeaderName, string guildName)
        {
            Type(PacketNumber);
            WriteString(guildLeaderName);
            WriteInt(-1);
            WriteString(guildName);
        }
    }
}