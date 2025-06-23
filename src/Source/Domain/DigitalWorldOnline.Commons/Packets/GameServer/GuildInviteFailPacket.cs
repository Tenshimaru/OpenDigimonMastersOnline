using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildInviteFailPacket : PacketWriter
    {
        private const int PacketNumber = 2110;

        public GuildInviteFailPacket(GuildInviteFailEnum inviteFailEnum, string targetName)
        {
            Type(PacketNumber);
            WriteInt(inviteFailEnum.GetHashCode());
            WriteString(targetName);
        }
    }
}