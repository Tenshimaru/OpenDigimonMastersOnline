using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class MobTinklePacket : PacketWriter
    {
        private const int PacketNumber = 1053;

        /// <summary>
        /// Warning tinkle once entering battle.
        /// </summary>
        /// <param name="handler">The handler of the target that is tinkleing.</param>
        public MobTinklePacket(int handler)
        {
            Type(PacketNumber);
            WriteInt(handler);
        }
    }
}