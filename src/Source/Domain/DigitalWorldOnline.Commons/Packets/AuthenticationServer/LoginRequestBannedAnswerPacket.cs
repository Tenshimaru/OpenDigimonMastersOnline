using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class LoginRequestBannedAnswerPacket : PacketWriter
    {
        private const int PacketNumber = 3308;

        /// <summary>
        /// The answer for banned account upon login
        /// </summary>
        /// <param name="accountBlock">The account block information</param>
        public LoginRequestBannedAnswerPacket(uint RemainingTimeInSeconds, string Reason)
        {
            Type(PacketNumber);
            WriteUInt(RemainingTimeInSeconds);
            WriteString(Reason);
        }
    }
}
