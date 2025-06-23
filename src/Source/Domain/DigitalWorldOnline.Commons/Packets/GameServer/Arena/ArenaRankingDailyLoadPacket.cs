using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ArenaRankingDailyLoadPacket : PacketWriter
    {
        private const int PacketNumber = 4130;

        /// <summary>
        /// Load the arena points and remaining daily reset time.
        /// </summary>
        /// <param name="remainingMinutes">The daily reset remaining minutes (UTC).</param>
        /// <param name="points">The points amount.</param>
        public ArenaRankingDailyLoadPacket(long remainingMinutes,int points)
        {
            Type(PacketNumber);
            WriteByte(1);
            WriteInt64(remainingMinutes);
            WriteInt(points); 
        }
    }
}