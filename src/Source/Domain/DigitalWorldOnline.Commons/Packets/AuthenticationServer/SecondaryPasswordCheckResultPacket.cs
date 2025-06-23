using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class SecondaryPasswordCheckResultPacket : PacketWriter
    {
        private const int PacketNumber = 9804;

        /// <summary>
        /// User entered correct secondary password or skipped the secondary password request.
        /// </summary>
        public SecondaryPasswordCheckResultPacket(SecondaryPasswordCheckEnum checkEnum)
        {
            Type(PacketNumber);
            WriteInt(checkEnum.GetHashCode());
        }
    }
}
