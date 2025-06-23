using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class KillSpawnMinimapNotifyPacket : PacketWriter
    {
        private const int PacketNumber = 1059;

        /// <summary>
        /// Notify the tamer for the incoming boss.
        /// </summary>
        /// <param name="targetMobType">Boss type</param>
        /// <param name="remainingMobs">Remaining mobs untill boss spawn</param>
        public KillSpawnMinimapNotifyPacket(int targetMobType, byte remainingMobs)
        {
            Type(PacketNumber);
            WriteInt(remainingMobs);
            WriteInt(targetMobType);
            WriteInt(remainingMobs);
        }
    }
}