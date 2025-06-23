using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;
using System.Runtime.InteropServices;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonArchiveLoadPacket : PacketWriter
    {
        private const int PacketNumber = 3204;

        /// <summary>
        /// Loads the current digimon archive info.
        /// </summary>
        /// <param name="digimonArchive">Archieve information</param>
        public DigimonArchiveLoadPacket(CharacterDigimonArchiveModel digimonArchive)
        {
            Type(PacketNumber);
            WriteInt(0);                    // nType
            WriteInt(digimonArchive.Slots); // nOpenSlotSize
            
            for (int i = 0; i < 3; i++)
            {
                WriteInt(0);
            }

            foreach (var digimonArchiveItem in digimonArchive.DigimonArchives.Where(x => x.Digimon != null))
            {
                var digimon = digimonArchiveItem.Digimon!;

                WriteInt(digimonArchiveItem.Slot);

                WriteUInt(digimon.GeneralHandler);
                WriteInt(digimon.BaseType);
                WriteString(digimon.Name);
                WriteShort(digimon.Size);
                WriteInt64(digimon.CurrentExperience * 100);
                WriteInt64(digimon.TranscendenceExperience * 100);
                WriteShort(digimon.Level);

                WriteInt(digimon.HP);
                WriteInt(digimon.DS);
                WriteInt(digimon.DE);
                WriteInt(digimon.AT);
                WriteInt(digimon.CurrentHp);
                WriteInt(digimon.CurrentDs);
                WriteInt(digimon.Friendship);
                WriteInt(digimon.BL);
                WriteInt(digimon.EV);
                WriteInt(digimon.CC);
                WriteInt(digimon.CD);
                WriteInt(digimon.AS);
                WriteInt(digimon.AR);
                WriteInt(digimon.HT);
                WriteInt(0);
                WriteInt(0);
                WriteInt(0);
                WriteInt(0);
                WriteByte((byte)digimon.HatchGrade);
                WriteInt(digimon.BaseType);

                WriteByte((byte)digimon.Evolutions.Count);

                for (int i = 0; i < digimon.Evolutions.Count; i++)
                {
                    var form = digimon.Evolutions[i];
                    WriteBytes(form.ToArray());
                }

                WriteShort(digimon.Digiclone.CloneLevel);
                WriteShort(digimon.Digiclone.ATValue);
                WriteShort(digimon.Digiclone.BLValue);
                WriteShort(digimon.Digiclone.CTValue);
                WriteShort(0); //DE Value (not implemented on client-side)
                WriteShort(digimon.Digiclone.EVValue);
                WriteShort(0); //HT Value (not implemented on client-side)
                WriteShort(digimon.Digiclone.HPValue);
                WriteShort(digimon.Digiclone.ATLevel);
                WriteShort(digimon.Digiclone.BLLevel);
                WriteShort(digimon.Digiclone.CTLevel);
                WriteShort(0); //DE Level (not implemented on client-side)
                WriteShort(digimon.Digiclone.EVLevel);
                WriteShort(0); //HT Level (not implemented on client-side)
                WriteShort(digimon.Digiclone.HPLevel);

                WriteShort(digimon.AttributeExperience.Data);
                WriteShort(digimon.AttributeExperience.Vaccine);
                WriteShort(digimon.AttributeExperience.Virus);

                WriteShort(digimon.AttributeExperience.Ice);
                WriteShort(digimon.AttributeExperience.Water);
                WriteShort(digimon.AttributeExperience.Fire);
                WriteShort(digimon.AttributeExperience.Land);
                WriteShort(digimon.AttributeExperience.Wind);
                WriteShort(digimon.AttributeExperience.Wood);
                WriteShort(digimon.AttributeExperience.Light);
                WriteShort(digimon.AttributeExperience.Dark);
                WriteShort(digimon.AttributeExperience.Thunder);
                WriteShort(digimon.AttributeExperience.Steel);

                WriteUInt(digimon.GeneralHandler);
                WriteByte(0);
            }

            WriteInt(1888);
        }
    }
}