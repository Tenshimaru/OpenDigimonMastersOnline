using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class MonsterSkillDamagePacket : PacketWriter
    {
        private const int PacketNumber = 16011;

        /// <summary>
        /// Set the target as out of combat.
        /// </summary>
        /// <param name="handler">The target handler to set</param>
        public MonsterSkillDamagePacket(int targetHandle,int targetSkillId, int finalDamage,List<CharacterModel> targetTamers)
        {
            Type(PacketNumber);
            WriteInt(targetHandle);
            WriteInt(targetSkillId);
            WriteShort((short)targetTamers.Count);

            foreach (var target in targetTamers)
            {

                WriteByte(target.Partner.Alive ? (byte)0 : (byte)1);

                long monsterHpMult = (long)target.Partner.CurrentHp * 255L;
                var HpRate  = (byte)(monsterHpMult / target.Partner.HP);
                WriteInt(target.Partner.GeneralHandler);
                WriteInt(finalDamage * -1);
                WriteByte(HpRate);

            }
        }
    }
}
