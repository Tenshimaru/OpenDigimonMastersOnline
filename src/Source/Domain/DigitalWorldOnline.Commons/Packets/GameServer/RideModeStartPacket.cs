using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class RideModeStartPacket : PacketWriter
    {
        private const int PacketNumber = 1325;

        /// <summary>
        /// Starts ride mode.
        /// </summary>
        /// <param name="tamerHandler">Target tamer handler</param>
        /// <param name="partnerHandler">Target partner handler</param>
        public RideModeStartPacket(ushort tamerHandler, ushort partnerHandler)
        {
            Type(PacketNumber);
            WriteUInt(tamerHandler);
            WriteUInt(partnerHandler);
        }
    }
}