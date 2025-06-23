using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildAuthorityUpdatePacket : PacketWriter
    {
        private const int PacketNumber = 2129;

        /// <summary>
        /// Updates the target guild authority information.
        /// </summary>
        /// <param name="guildAuthority">Guild authority information</param>
        public GuildAuthorityUpdatePacket(GuildAuthorityModel guildAuthority)
        {
            Type(PacketNumber);
            WriteByte((byte)guildAuthority.Class);
            WriteString(guildAuthority.Title);
            WriteString(guildAuthority.Duty);

            WriteByte(0);
        }
    }
}