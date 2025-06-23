using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class CharacterListPacket : PacketWriter
    {
        private const int PacketNumber = 1301;

        public CharacterListPacket(IEnumerable<CharacterModel> characters)
        {
            Type(PacketNumber);

            foreach (var character in characters.Where(x => x.Partner != null))
            {
                WriteByte(character.Position);
                WriteShort(character.Location.MapId);
                WriteInt(character.Model.GetHashCode());
                WriteByte(character.Level);
                WriteString(character.Name);

                for (int i = 0; i < GeneralSizeEnum.Equipment.GetHashCode(); i++)
                {
                    if(i == 11)
                        WriteBytes(character.Digivice.Items[0].ToArray(true));
                    else
                        WriteBytes(character.Equipment.Items[i].ToArray(true));
                }

                WriteInt(character.Partner.BaseType);
                WriteByte(character.Partner.Level);
                WriteString(character.Partner.Name);
                WriteShort(character.Partner.Size);

                //TODO: Ver o que esses 2 mudam
                WriteShort(0); //??
                WriteShort(0); //??
                WriteShort(character.SealList.SealLeaderId);
            }

            WriteByte(99);
        }
    }
}
