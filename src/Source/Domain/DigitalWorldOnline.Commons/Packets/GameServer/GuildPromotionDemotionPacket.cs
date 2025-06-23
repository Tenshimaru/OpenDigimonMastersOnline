using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildPromotionDemotionPacket : PacketWriter
    {
        /// <summary>
        /// Guild member authority change.
        /// </summary>
        /// <param name="packetType">Authority rank type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="authorityDescription">New authority type description</param>
        public GuildPromotionDemotionPacket(int packetType, string memberName, string authorityDescription)
        {
            Type(packetType);
            WriteString(memberName);
            WriteString(authorityDescription);
        }
    }
}