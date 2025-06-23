using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartnerSwitchPacket : PacketWriter
    {
        private const int PacketNumber = 1041;

        /// <summary>
        /// Updates the character current partner.
        /// </summary>
        /// <param name="handler">General handler.</param>
        /// <param name="oldPartnerCurrentType">Old partner type</param>
        /// <param name="newPartner">New partner</param>
        /// <param name="slot">Target digivice slot</param>
        public PartnerSwitchPacket(ushort handler, int oldPartnerCurrentType, DigimonModel newPartner, byte slot)
        {
            Type(PacketNumber);
            WriteUInt(handler);
            WriteInt(oldPartnerCurrentType);
            WriteByte(slot);
            WriteInt(newPartner.BaseType);
            WriteByte(newPartner.Level);
            WriteString(newPartner.Name);
            WriteShort(newPartner.Size);
            WriteInt(0);
            WriteShort(newPartner.Digiclone.CloneLevel);
            WriteShort(newPartner.Digiclone.ATValue);
            WriteShort(newPartner.Digiclone.BLValue);
            WriteShort(newPartner.Digiclone.CTValue);
            WriteShort(0); //AS
            WriteShort(newPartner.Digiclone.EVValue);
            WriteShort(0); //HT
            WriteShort(newPartner.Digiclone.HPValue);

            WriteShort(newPartner.Digiclone.ATLevel);
            WriteShort(newPartner.Digiclone.BLLevel);
            WriteShort(newPartner.Digiclone.CTLevel);
            WriteShort(0); //AS
            WriteShort(newPartner.Digiclone.EVLevel);
            WriteShort(0); //HT
            WriteShort(newPartner.Digiclone.HPLevel);

            WriteShort((short)newPartner.BuffList.ActiveBuffs.Count);
            foreach (var buff in newPartner.BuffList.ActiveBuffs)
            {
                WriteShort((short)buff.BuffId);
                WriteShort((short)buff.TypeN);
                WriteInt(UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            WriteShort(newPartner.AttributeExperience.Data);
            WriteShort(newPartner.AttributeExperience.Vaccine);
            WriteShort(newPartner.AttributeExperience.Virus);

            WriteShort(newPartner.AttributeExperience.Ice);
            WriteShort(newPartner.AttributeExperience.Water);
            WriteShort(newPartner.AttributeExperience.Fire);
            WriteShort(newPartner.AttributeExperience.Land);
            WriteShort(newPartner.AttributeExperience.Wind);
            WriteShort(newPartner.AttributeExperience.Wood);
            WriteShort(newPartner.AttributeExperience.Light);
            WriteShort(newPartner.AttributeExperience.Dark);
            WriteShort(newPartner.AttributeExperience.Thunder);
            WriteShort(newPartner.AttributeExperience.Steel);
        }
    }
}