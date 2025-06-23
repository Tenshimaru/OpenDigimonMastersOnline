using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemReturnPacket : PacketWriter
    {
        private const int PacketNumber = 3923;

        /// <summary>
        /// Returns the success message on item returning.
        /// </summary>
        /// <param name="receivedBits">Received bits upon returning</param>
        /// <param name="previousBits">Current bits amount</param>
        public ItemReturnPacket(int receivedBits, long previousBits)
        {
            Type(PacketNumber);
            WriteInt(receivedBits);
            WriteInt64(previousBits);
            WriteInt(0);
        }
    }
}