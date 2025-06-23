using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Readers;

namespace DigitalWorldOnline.Game
{
    public class GamePacketReader : PacketReaderBase, IPacketReader
    {
        public GameServerPacketEnum Enum => (GameServerPacketEnum)Type;

        public GamePacketReader(byte[] buffer)
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
