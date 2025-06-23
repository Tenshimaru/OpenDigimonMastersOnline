using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class CharacterCreatedPacket : PacketWriter
    {
        private const int PacketNumber = 1306;

        public CharacterCreatedPacket(CharacterModel character, short handshake)
        {
            Type(PacketNumber);

            WriteShort(handshake);
            WriteShort(0);
            WriteShort(0);
            WriteShort(0);
            WriteShort(handshake);
            WriteShort(0);
            WriteShort(0);
            WriteShort(0);
            WriteByte(character.Position);
            WriteShort(character.Location.MapId);
            //WriteByte(0);
            WriteInt(character.Model.GetHashCode());
            WriteByte(0x1);
            WriteString(character.Name);

            for (int i = 0; i < GeneralSizeEnum.Equipment.GetHashCode(); i++) //654: +68
                WriteBytes(new byte[68]);

            WriteInt(character.Partner.Model.GetHashCode());
            WriteByte(1);
            WriteString(character.Partner.Name);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
            WriteByte(0);
        }
    }
}
