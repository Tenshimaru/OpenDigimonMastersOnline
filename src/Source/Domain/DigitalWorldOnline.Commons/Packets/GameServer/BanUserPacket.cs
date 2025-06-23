using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class BanUserPacket : PacketWriter
    {
        private const int PacketNumber = 9939;

        /// <summary>
        /// Bans the target client.
        /// </summary>
        public BanUserPacket(uint RemainingTimeInSeconds, string Reason)
        {
            Type(PacketNumber);
            WriteUInt(RemainingTimeInSeconds);
            WriteString(Reason);
        }
    }
}
