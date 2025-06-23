using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class SetSealLeaderPacket : PacketWriter
    {
        private const int PacketNumber = 3232;

        /// <summary>
        /// Set the current seal leader
        /// </summary>
        /// <param name="handler">Target tamer</param>
        /// <param name="sealLeaderSequential">Target seal leader sequential id</param>
        public SetSealLeaderPacket(int handler, short sealLeaderSequential)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteShort(sealLeaderSequential);
        }
    }
}