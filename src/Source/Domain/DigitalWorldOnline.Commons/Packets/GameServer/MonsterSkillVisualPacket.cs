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
    public class MonsterSkillVisualPacket : PacketWriter
    {
        private const int PacketNumber = 1123;

        /// <summary>
        /// Set the target as out of combat.
        /// </summary>
        /// <param name="handler">The target handler to set</param>
        public MonsterSkillVisualPacket(int  AttackHandler,int TargetSkillId)
        {
           Type(1123);
           WriteInt(AttackHandler);
           WriteInt(TargetSkillId);
        }
    }
}
