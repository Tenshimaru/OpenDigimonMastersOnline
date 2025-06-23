using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonArchiveManagePacket : PacketWriter
    {
        private const int PacketNumber = 3201;

        /// <summary>
        /// Manages the digimon archive.
        /// </summary>
        /// <param name="digiviceSlot">Target digivice slot</param>
        /// <param name="archiveSlot">Target archive slot</param>
        /// <param name="price">Management price</param>
        public DigimonArchiveManagePacket(int digiviceSlot, int archiveSlot, int price)
        {
            Type(PacketNumber);
            WriteInt(digiviceSlot);
            WriteInt(archiveSlot + 1000);
            WriteInt(price);
        }
    }
}