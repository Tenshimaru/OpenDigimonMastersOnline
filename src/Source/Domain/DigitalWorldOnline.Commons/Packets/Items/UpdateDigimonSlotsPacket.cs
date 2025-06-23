using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class UpdateDigimonSlotsPacket : PacketWriter
    {
        private const int PacketNumber = 1102;

        /// <summary>
        /// Load the tamer's digimon slots.
        /// </summary>
        /// <param name="digimonSlots">Slots amount</param>
        public UpdateDigimonSlotsPacket(byte digimonSlots)
        {
            Type(PacketNumber);
            WriteInt(digimonSlots);
        }
    }
}
