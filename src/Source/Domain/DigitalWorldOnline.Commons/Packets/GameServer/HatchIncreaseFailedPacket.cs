using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class HatchIncreaseFailedPacket : PacketWriter
    {
        private const int PacketNumber = 1040;

        /// <summary>
        /// Returns the fail result of hatch increase try.
        /// </summary>
        /// <param name="tamerHandler">Target tamer handler</param>
        /// <param name="hatchResult">Hatch increase fail enumeration</param>
        public HatchIncreaseFailedPacket(ushort tamerHandler, HatchIncreaseResultEnum hatchResult)
        {
            Type(PacketNumber);
            WriteUInt(tamerHandler);
            WriteByte((byte)hatchResult);
        }
    }
}