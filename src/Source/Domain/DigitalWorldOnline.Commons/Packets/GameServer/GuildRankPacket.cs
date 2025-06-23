using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildRankPacket : PacketWriter
    {
        private const int PacketNumber = 2127;

        /// <summary>
        /// Sends the current guild rank position.
        /// </summary>
        /// <param name="position">Guild rank position</param>
        public GuildRankPacket(short position)
        {
            Type(PacketNumber);
            WriteShort(position);
        }
    }
}