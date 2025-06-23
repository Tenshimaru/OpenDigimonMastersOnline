using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class PickBitsPacket : PacketWriter
    {
        private const int PacketNumber = 3911;

        /// <summary>
        /// Try to pick bits from the ground.
        /// </summary>
        /// <param name="appearanceHandler">The tamer general handler.</param>
        /// <param name="value">The received bits amount.</param>
        public PickBitsPacket(int handler, int value)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteInt(value);
            WriteInt(0);
            WriteInt(0);
            WriteInt(0);
        }
    }
}