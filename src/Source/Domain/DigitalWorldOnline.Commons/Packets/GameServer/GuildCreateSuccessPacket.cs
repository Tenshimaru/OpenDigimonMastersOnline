using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildCreateSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 2101;

        /// <summary>
        /// Creates a new guild.
        /// </summary>
        /// <param name="guildLeaderName">The name of the tamer who's creating the guild</param>
        /// <param name="itemSlot">The slot number of the item used to create the guild</param>
        /// <param name="guildName">The guild name</param>
        public GuildCreateSuccessPacket(string guildLeaderName, int itemSlot, string guildName)
        {
            Type(PacketNumber);
            WriteString(guildLeaderName);
            WriteInt(itemSlot);
            WriteString(guildName);
        }
    }
}