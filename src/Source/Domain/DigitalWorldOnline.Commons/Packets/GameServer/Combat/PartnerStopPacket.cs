using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class PartnerStopPacket : PacketWriter
    {
        private const int PacketNumber = 1034;

        /// <summary>
        /// Calls the partner back.
        /// </summary>
        /// <param name="partnerHandler">The partner handler</param>
        public PartnerStopPacket(int partnerHandler)
        {
            Type(PacketNumber);
            WriteUInt((uint)partnerHandler);
        }
    }
}