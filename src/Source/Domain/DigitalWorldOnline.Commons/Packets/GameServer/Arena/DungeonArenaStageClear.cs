using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Arena
{
    public class DungeonArenaStageClearPacket : PacketWriter
    {
        private const int PacketNumber = 4125;


        public DungeonArenaStageClearPacket(int mobType, int currentStage, int totalPoints, int currentPoints, int NpcId)
        {
            byte Final = 0;

            if (currentStage == 40)
                Final = 1;

            Type(PacketNumber);
            WriteInt(currentStage);
            WriteByte(Final);
            WriteInt(mobType);
            WriteShort((short)totalPoints);
            WriteShort((short)currentPoints);
            WriteInt(NpcId);

        }
    }
}
