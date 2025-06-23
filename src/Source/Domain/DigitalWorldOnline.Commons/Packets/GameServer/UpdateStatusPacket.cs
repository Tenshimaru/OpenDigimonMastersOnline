using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateStatusPacket : PacketWriter
    {
        private const int PacketNumber = 1043;

        /// <summary>
        /// Updates the tamer and partner current status.
        /// </summary>
        /// <param name="character">The tamer to be updated</param>
        public UpdateStatusPacket(CharacterModel character)
        {
            Type(PacketNumber);
            WriteInt(character.HP);
            WriteInt(character.DS);
            WriteInt(character.CurrentHp);
            WriteInt(character.CurrentDs);
            WriteShort(character.AT);
            WriteShort(character.DE);
            WriteShort((short)character.MS);
            WriteInt(character.Partner.HP);
            WriteInt(character.Partner.DS);
            WriteInt(character.Partner.CurrentHp);
            WriteInt(character.Partner.CurrentDs);
            WriteShort(character.Partner.FS);
            WriteShort((short)character.Partner.AT);
            WriteShort(character.Partner.DE);
            WriteShort((short)character.Partner.CC);
            WriteShort((short)character.Partner.AS);
            WriteShort((short)character.Partner.EV);
            WriteShort(character.Partner.HT);
            WriteShort(character.Partner.AR);
            WriteShort(character.Partner.BL);

            WriteShort(character.Partner.Digiclone.CloneLevel);
            WriteShort(character.Partner.Digiclone.ATValue);
            WriteShort(character.Partner.Digiclone.BLValue);
            WriteShort(character.Partner.Digiclone.CTValue);
            WriteShort(0); //AS
            WriteShort(character.Partner.Digiclone.EVValue);
            WriteShort(0); //HT
            WriteShort(character.Partner.Digiclone.HPValue);

            WriteShort(character.Partner.Digiclone.ATLevel);
            WriteShort(character.Partner.Digiclone.BLLevel);
            WriteShort(character.Partner.Digiclone.CTLevel);
            WriteShort(0); //AS
            WriteShort(character.Partner.Digiclone.EVLevel);
            WriteShort(0); //HT
            WriteShort(character.Partner.Digiclone.HPLevel);
        }
    }
}