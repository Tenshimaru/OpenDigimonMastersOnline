using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class LoginRequestAnswerPacket : PacketWriter
    {
        private const int PacketNumber = 3301;

        /// <summary>
        /// The answer for login request fail
        /// </summary>
        /// <param name="failReason">The fail reason enumeration</param>
        public LoginRequestAnswerPacket(LoginFailReasonEnum failReason)
        {
            //TODO: try to use int/short instead of bytes

            Type(PacketNumber);
            WriteByte((byte)failReason.GetHashCode());
            WriteByte(39);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
        }

        /// <summary>
        /// The answer for login request success
        /// </summary>
        /// <param name="secPassScreen">The secondary password screen enumeration</param>
        public LoginRequestAnswerPacket(SecondaryPasswordScreenEnum secPassScreen)
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteByte((byte)secPassScreen.GetHashCode());
        }
    }
}
