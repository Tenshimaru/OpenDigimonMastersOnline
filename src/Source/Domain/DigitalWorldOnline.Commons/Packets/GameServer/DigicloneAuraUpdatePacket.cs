using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigicloneAuraUpdatePacket : PacketWriter
    {
        private const int PacketNumber = 3214;

        /// <summary>
        /// Updates the digiclone aura for the target tamer.
        /// </summary>
        /// <param name="partnerHandler">Target tamer handler</param>
        /// <param name="digiclone">Clone level information</param>
        public DigicloneAuraUpdatePacket(ushort partnerHandler, DigimonDigicloneModel digiclone)
        {
            Type(PacketNumber);
            WriteInt(partnerHandler);
            WriteShort(digiclone.CloneLevel);
            WriteShort(digiclone.ATLevel);
            WriteShort(digiclone.BLLevel);
            WriteShort(digiclone.CTLevel);
            WriteShort(digiclone.EVLevel);
            WriteShort(digiclone.HPLevel);
        }
    }
}