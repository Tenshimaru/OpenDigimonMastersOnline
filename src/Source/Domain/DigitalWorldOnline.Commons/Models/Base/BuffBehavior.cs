using DigitalWorldOnline.Commons.Models.Asset;

namespace DigitalWorldOnline.Commons.Models
{
    public partial class Buff
    {
        /// <summary>
        /// Flag for expired buffs.
        /// </summary>
        public bool Expired => Duration == -1 || (Duration > 0 && DateTime.Now.AddSeconds(3) >= EndDate);

        public bool DebuffExpired => DateTime.Now >= EndDate;

        /// <summary>
        /// Remaining time in seconds.
        /// </summary>
        public int RemainingSeconds => (EndDate - DateTime.Now).TotalSeconds > 0 ? (int)(EndDate - DateTime.Now).TotalSeconds : 0;

        /// <summary>
        /// Increase buff duration.
        /// </summary>
        /// <param name="duration">Duration (in seconds) to increase</param>
        public void IncreaseDuration(int duration)
        {
            Duration += duration;
            EndDate = DateTime.Now.AddSeconds(Duration);
        }

        public void IncreaseEndDate(int duration)
        {
            Duration = duration;
            EndDate = DateTime.Now.AddSeconds(duration);
        }

        /// <summary>
        /// Creates a new buff object.
        /// </summary>
        /// <param name="buffId">Buff id (client reference)</param>
        /// <param name="skillId">Skill id (client reference)</param>
        /// <param name="duration">Duration (in seconds)</param>
        public static Buff Create(int buffId, int skillId, int duration = 0)
        {
            return new Buff()
            {
                BuffId = buffId,
                SkillId = skillId,
                Duration = duration,
                EndDate = DateTime.Now.AddSeconds(duration)
            };
        }

        /// <summary>
        /// Updates the buff info.
        /// </summary>
        /// <param name="buffInfo">The info asset</param>
        public void SetBuffInfo(BuffInfoAssetModel? buffInfo) => BuffInfo ??= buffInfo;

        /// <summary>
        /// Updates the buff id.
        /// </summary>
        /// <param name="buffId">The new buff id</param>
        public void SetBuffId(int buffId) => BuffId = buffId;
        public void SetTypeN(int typeN) => TypeN = typeN;
        public void SetCooldown(int cooldown)
        {
            Cooldown = cooldown;
            CoolEndDate = DateTime.Now.AddSeconds(cooldown);
        }
        /// <summary>
        /// Updates the buff skill id.
        /// </summary>
        /// <param name="skillId">The new skill id</param>
        public void SetSkillId(int skillId) => SkillId = skillId;
        /// <summary>
        /// Updates the buff duration.
        /// </summary>
        /// <param name="duration">The new buff duration</param>
        public void SetDuration(int duration, bool fixedValue = false)
        {
            if (fixedValue)
            {
                Duration = duration;
                EndDate = DateTime.UtcNow.AddSeconds(Duration);
            }
            else
            {
                Duration += duration;
                EndDate = EndDate.AddSeconds(duration);
            }
        }

        public void SetEndDate(DateTime endDate) => EndDate = endDate;

        /// <summary>
        /// Serializes the buff object.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            using MemoryStream m = new();
            m.Write(BitConverter.GetBytes(BuffId), 0, 4);
            m.Write(BitConverter.GetBytes(Duration), 0, 4);
            m.Write(BitConverter.GetBytes(SkillId), 0, 4);

            return m.ToArray();
        }
    }
}