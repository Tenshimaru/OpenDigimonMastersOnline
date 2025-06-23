using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonArchivePacket : PacketWriter
    {
        private const int PacketNumber = 3243;

        /// <summary>
        /// Manages the digimon archive.
        /// </summary>
        /// <param name="digiviceSlot">Target digivice slot</param>
        /// <param name="archiveSlot">Target archive slot</param>
        /// <param name="price">Management price</param>
        public DigimonArchivePacket(int digiviceSlot, int archiveSlot)
        {
            Type(PacketNumber);
            WriteInt(digiviceSlot + 1000);
            WriteInt(archiveSlot + 1000);
        }
    }
}