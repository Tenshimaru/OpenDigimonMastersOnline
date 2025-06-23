using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class KillSpawnEndChatNotifyPacket : PacketWriter
    {
        private const int PacketNumber = 1061;

        /// <summary>
        /// Notify the tamer the end of the kill spawn.
        /// </summary>
        /// <param name="targetMobType">Boss type</param>
        public KillSpawnEndChatNotifyPacket(int targetMobType)
        {
            Type(PacketNumber);
            WriteInt(targetMobType);
            WriteInt(0);
        }
    }
}