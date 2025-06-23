using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class RemoveSealLeaderPacket : PacketWriter
    {
        private const int PacketNumber = 3233;

        /// <summary>
        /// Removes the current seal leader
        /// </summary>
        /// <param name="handler">Target tamer</param>
        /// <param name="sealLeaderSequential">Target seal leader sequential id</param>
        public RemoveSealLeaderPacket(int handler, short sealLeaderSequential)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteShort(sealLeaderSequential);
        }
    }
}