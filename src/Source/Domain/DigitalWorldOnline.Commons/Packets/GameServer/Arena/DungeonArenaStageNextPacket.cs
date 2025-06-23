using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Arena
{
    public class DungeonArenaNextStagePacket : PacketWriter
    {
        private const int PacketNumber = 4126;


        public DungeonArenaNextStagePacket(int currentStage, int NpcId, int remainTime)
        {
            Type(PacketNumber);
            WriteByte((byte)currentStage);
            WriteInt(NpcId);
            WriteInt(remainTime);
        }
    }
}
