
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;

namespace DigitalWorldOnline.Commons.Model.Character
{
    public class CharacterTamerSkillModel
    {
        public Guid Id { get; set; }
        public TamerSkillTypeEnum Type { get; set; }
        public int SkillId { get; set; }
        public int Cooldown { get; set; }
        public DateTime EndCooldown { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }

        public bool Expired => DateTime.Now >= EndDate;
        public int RemainingCooldownSeconds => (EndCooldown - DateTime.Now).TotalSeconds > 0 ? (int)(EndCooldown - DateTime.Now).TotalSeconds : 0;
        public int RemainingMinutes => (EndDate - DateTime.Now).TotalMinutes > 0 ? (int)(EndDate - DateTime.Now).TotalMinutes : 0;

        public CharacterTamerSkillModel(TamerSkillTypeEnum type, int skillId, int cooldown, DateTime endCooldown)
        {
            Id = Guid.NewGuid();
            Type = type;
            SkillId = skillId;
            Cooldown = cooldown;
            EndCooldown = endCooldown;
        }
        public void SetCooldown(int cooldown)
        {
            Cooldown = cooldown;
            EndCooldown = DateTime.Now.AddSeconds(cooldown);
        }

        public void SetTamerSkill(int skillId, int cooldown,TamerSkillTypeEnum type, int duration = 0)
        {
            SkillId = skillId;
            EndCooldown = DateTime.Now.AddSeconds(cooldown);
            Duration = duration;
            EndDate = DateTime.Now.AddMinutes(duration);
            Type = type;
        }
        public void IncreaseEndDate(int duration)
        {
            Duration += duration;
            EndDate = DateTime.Now.AddSeconds(duration);
        }
    }
}
