using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class KillSpawnChatNotifyPacket : PacketWriter
    {
        private const int PacketNumber = 1060;

        /// <summary>
        /// Notify the tamer for the incoming boss.
        /// </summary>
        public KillSpawnChatNotifyPacket(int mapId, byte channel, int targetMobType)
        {
            Type(PacketNumber);
            WriteInt(mapId);
            WriteInt(channel);
            WriteInt(targetMobType);
            WriteInt(0);

        }
    }
}