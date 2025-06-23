using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class SkillUpdateCooldownPacket : PacketWriter
    {
        private const int PacketNumber = 3246;
   
        /// <summary>
        /// Set the target as out of combat.
        /// </summary>
        /// <param name="handler">The target handler to set</param>
        public SkillUpdateCooldownPacket( int handler, int currentType,DigimonEvolutionModel? evolution, List<int> skillIds)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteInt(currentType);
            WriteInt(skillIds.Count);

            for (int i = 0; i < skillIds.Count; i++)
            {
            
                
               
                if (evolution.Skills[i].RemainingSeconds > 0)
                {
                    WriteInt(skillIds[i]);
                    var EndTime = (int)DateTimeOffset.UtcNow.AddSeconds(DateTime.Now.AddSeconds(evolution.Skills[i].RemainingSeconds).Subtract(DateTime.Now).TotalSeconds).ToUnixTimeSeconds();
                    WriteInt(EndTime);
                }
            }
        }
    }
}