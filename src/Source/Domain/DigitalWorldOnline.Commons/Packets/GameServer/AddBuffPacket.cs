using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class AddBuffPacket : PacketWriter
    {
        private const int PacketNumber = 4000;

        /// <summary>
        /// Applys a new buff to the target.
        /// </summary>
        /// <param name="handler">Target handler</param>
        /// <param name="buff">Target buff info</param>
        /// <param name="duration">Buff duration</param>
        public AddBuffPacket(int handler, BuffInfoAssetModel buff, short TypeN, int duration)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort((short)buff.BuffId);
            WriteShort(TypeN);
            WriteInt(duration);
            WriteInt(buff.SkillCode);
        }
        public AddBuffPacket(int handler, BuffInfoAssetModel buff, short TypeN, uint duration)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort((short)buff.BuffId);
            WriteShort(TypeN);
            WriteUInt(duration);
            WriteInt(buff.SkillCode);
        }
        public AddBuffPacket(int handler, int BuffId, int SkillCode, short TypeN, int duration)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort((short)BuffId);
            WriteShort(TypeN);
            WriteInt(duration);
            WriteInt(SkillCode);
        }

        // ---------------------------------------------------------------------------------------------

        public class AddDotDebuffPacket : PacketWriter
        {
            private const int PacketNumber = 4011;
            public AddDotDebuffPacket(int hitHandler, int targetHandler, int nBuffCode, byte nHpRate, int nDamage, byte bDie)
            {
                Type(PacketNumber);
                WriteUInt((uint)hitHandler);    // nHitterUID
                WriteUInt((uint)targetHandler); // nTargetUID
                WriteUShort((ushort)nBuffCode); // nBuffCode
                WriteByte(nHpRate);             // nHpRate
                WriteInt(nDamage);              // nDamage
                WriteByte(bDie);                // bDie
            }
        }

        // ---------------------------------------------------------------------------------------------
        
        public class SkillBuffPacket :PacketWriter
        {
            private const int PacketNumber = 4012;
            public SkillBuffPacket(int hitHandler,int nBuffCode,byte skillLevel,int time,int skillCode)
            {
                Type(PacketNumber);
                WriteUInt((uint)hitHandler);
                WriteUShort((ushort)nBuffCode);
                WriteByte(skillLevel);
                WriteUInt((uint)time);
                WriteUInt((uint)skillCode);
            }
        }
        
        // ---------------------------------------------------------------------------------------------

        public class AddStunDebuffPacket : PacketWriter
        {
            private const int PacketNumber = 4013;
            public AddStunDebuffPacket(int handler, int BuffId, int SkillCode, int duration)
            {
                Type(PacketNumber);
                WriteUInt((uint)handler);
                WriteInt(SkillCode);
                WriteShort((short)BuffId);
                WriteInt(duration);

            }
        }

        // ---------------------------------------------------------------------------------------------
    }
}