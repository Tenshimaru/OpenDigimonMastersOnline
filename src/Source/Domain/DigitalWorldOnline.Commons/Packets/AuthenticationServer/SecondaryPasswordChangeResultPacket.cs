using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class SecondaryPasswordChangeResultPacket : PacketWriter
    {
        private const int PacketNumber = 9806;

        /// <summary>
        /// The result of changing the secondary password.
        /// </summary>
        public SecondaryPasswordChangeResultPacket(SecondaryPasswordChangeEnum checkEnum)
        {
            Type(PacketNumber);
            WriteInt(checkEnum.GetHashCode());
        }
    }
}
