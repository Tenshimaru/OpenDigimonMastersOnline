using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerXaiResourcesPacket : PacketWriter
    {
        private const int PacketNumber = 16032;

        /// <summary>
        /// Show the current tamer's XAI resources.
        /// </summary>
        /// <param name="xgauge">The tamer's current xGauge</param>
        /// /// <param name="xcrystals">The tamer's current xCrystals</param>
        public TamerXaiResourcesPacket(int xgauge, short xcrystals)
        {
            Type(PacketNumber);
            WriteInt(xgauge);
            WriteShort(xcrystals);
        }
    }
}
