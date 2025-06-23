using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class LevelUpPacket : PacketWriter
    {
        private const int PacketNumber = 1019;

        /// <summary>
        /// Receives any kind of EXP for the tamer and partner.
        /// </summary>
        public LevelUpPacket(int handler, byte level)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteByte(level);
        }
    }
}