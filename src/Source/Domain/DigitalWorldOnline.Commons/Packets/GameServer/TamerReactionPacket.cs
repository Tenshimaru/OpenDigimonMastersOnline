using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerReactionPacket : PacketWriter
    {
        private const int PacketNumber = 1058;

        /// <summary>
        /// Runs the tamer action (dance, yellow, etc).
        /// </summary>
        public TamerReactionPacket(int handler, int type, int value)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteInt(type);
            WriteInt(value);
        }
    }
}