using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class RemoveBuffPacket : PacketWriter
    {
        private const int PacketNumber = 4002;

        /// <summary>
        /// Remove buff(s) from the target.
        /// </summary>
        /// <param name="handler">Target handler</param>
        /// <param name="buffId">Target buff id</param>
        /// <param name="amount">Amount to remove</param>
        public RemoveBuffPacket(int handler, int buffId, short amount = 1)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort(amount);
            WriteUShort((ushort)buffId);
        }
    }
}