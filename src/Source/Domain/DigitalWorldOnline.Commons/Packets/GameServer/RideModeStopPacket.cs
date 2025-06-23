using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class RideModeStopPacket : PacketWriter
    {
        private const int PacketNumber = 1326;

        /// <summary>
        /// Ends ride mode.
        /// </summary>
        /// <param name="tamerHandler">Target tamer handler</param>
        /// <param name="partnerHandler">Target partner handler</param>
        public RideModeStopPacket(ushort tamerHandler, ushort partnerHandler)
        {
            Type(PacketNumber);
            WriteUInt(tamerHandler);
            WriteUInt(partnerHandler);
        }
    }
}