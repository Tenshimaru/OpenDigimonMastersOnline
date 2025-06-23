using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ArenaRankingDailyUpdatePointsPacket : PacketWriter
    {
        private const int PacketNumber = 4131;

        /// <summary>
        /// Load the arena points and remaining daily reset time.
        /// </summary>
        /// <param name="remainingMinutes">The daily reset remaining minutes (UTC).</param>
        /// <param name="points">The points amount.</param>
        public ArenaRankingDailyUpdatePointsPacket(int points)
        {
            Type(PacketNumber);
            WriteByte(100);
            WriteInt(points);
        }
    }
}