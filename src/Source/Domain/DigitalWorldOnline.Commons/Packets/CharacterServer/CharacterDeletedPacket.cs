using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class CharacterDeletedPacket : PacketWriter
    {
        private const int PacketNumber = 1304;

        public CharacterDeletedPacket(DeleteCharacterResultEnum result)
        {
            Type(PacketNumber);
            WriteInt(result.GetHashCode());
        }
    }
}
