using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildDeletePacket : PacketWriter
    {
        private const int PacketNumber = 2102;

        /// <summary>
        /// Deletes a guild.
        /// </summary>
        /// <param name="guildName">The guild name</param>
        public GuildDeletePacket(string guildName)
        {
            Type(PacketNumber);
            WriteString(guildName);
        }
    }
}