using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Readers;

namespace DigitalWorldOnline.Character
{
    public class CharacterPacketReader : PacketReaderBase
    {
        public CharacterServerPacketEnum Enum => (CharacterServerPacketEnum)Type;

        public CharacterPacketReader(byte[] buffer)
        {
            Packet = new(buffer);

            Length = ReadShort();

            Type = ReadShort();

            Packet.Seek(Length - 2, SeekOrigin.Begin);

            int checksum = ReadShort();

            if (checksum != (Length ^ CheckSumValidation))
                throw new Exception("Invalid packet checksum");

            Packet.Seek(4, SeekOrigin.Begin);
        }
    }
}
