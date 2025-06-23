using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Readers;

namespace DigitalWorldOnline.Account
{
    public class AuthenticationPacketReader : PacketReaderBase
    {
        public AuthenticationServerPacketEnum Enum => (AuthenticationServerPacketEnum)Type;

        public AuthenticationPacketReader(byte[] buffer)
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
