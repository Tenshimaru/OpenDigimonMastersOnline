using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartnerDeletePacket : PacketWriter
    {
        private const int PacketNumber = 1042;

        /// <summary>
        /// Deletes the target partner.
        /// </summary>
        /// <param name="slot">Target digivice slot</param>
        public PartnerDeletePacket(int slot)
        {
            Type(PacketNumber);
            WriteInt(slot);
        }
    }
}