using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigicloneResetPacket : PacketWriter
    {
        private const int PacketNumber = 1083;

        /// <summary>
        /// Resets the current digiclone level.
        /// </summary>
        /// <param name="digiclone">Digiclone info</param>
        public DigicloneResetPacket(DigimonDigicloneModel digiclone)
        {
            Type(PacketNumber);
            WriteShort(digiclone.CloneLevel);

            WriteShort(digiclone.ATValue);
            WriteShort(digiclone.BLValue);
            WriteShort(digiclone.CTValue);
            WriteShort(0); //Valor atual do as (versao 487 nao tem)
            WriteShort(digiclone.EVValue);
            WriteShort(0); //Valor atual do HT (versao 487 nao tem)
            WriteShort(digiclone.HPValue);

            WriteShort(digiclone.ATLevel);
            WriteShort(digiclone.BLLevel);
            WriteShort(digiclone.CTLevel);
            WriteShort(0); //Level atual do as (versao 487 nao tem)
            WriteShort(digiclone.EVLevel);
            WriteShort(0); //Level atual do HT (versao 487 nao tem)
            WriteShort(digiclone.HPLevel);
        }
    }
}