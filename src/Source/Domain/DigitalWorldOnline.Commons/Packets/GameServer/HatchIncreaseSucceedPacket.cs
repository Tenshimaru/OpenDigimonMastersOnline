using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class HatchIncreaseSucceedPacket : PacketWriter
    {
        private const int PacketNumber = 1037;

        /// <summary>
        /// Returns the succeed result of hatch increase try.
        /// </summary>
        /// <param name="tamerHandler">Target tamer handler</param>
        /// <param name="hatchLevel">Hatch current grade</param>
        public HatchIncreaseSucceedPacket(ushort tamerHandler, int hatchLevel)
        {
            Type(PacketNumber);
            WriteUInt(tamerHandler);
            WriteByte((byte)hatchLevel);
        }
    }
}