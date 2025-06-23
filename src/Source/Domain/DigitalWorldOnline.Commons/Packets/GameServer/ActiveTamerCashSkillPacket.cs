using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ActiveTamerCashSkill : PacketWriter
    {
        private const int PacketNumber = 1330;

        public ActiveTamerCashSkill(int skillId,int remaningMinutes)
        {

            Type(PacketNumber);
            WriteInt(skillId);
            WriteInt(remaningMinutes);
        }

       
    }

    public class ActiveTamerCashSkillExpire : PacketWriter
    {
        private const int PacketNumber = 1331;

        public ActiveTamerCashSkillExpire(int skillId)
        {

            Type(PacketNumber);
            WriteByte(1);
            WriteInt(skillId);
        }


    }

    public class ActiveTamerCashSkillRemove : PacketWriter
    {
        private const int PacketNumber = 1332;

        public ActiveTamerCashSkillRemove(int skillId)
        {

            Type(PacketNumber);
            WriteByte(1);
            WriteInt(skillId);
        }


    }

}