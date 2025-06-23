using System;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonEvolutionSkillModel
    {

        /// <summary>
        /// Flag for expired buffs.
        /// </summary>
        public bool Expired => DateTime.Now >= EndDate;

        /// <summary>
        /// Remaining time in seconds.
        /// </summary>
        public int RemainingSeconds
        {
            get
            {
                TimeSpan remainingTime = EndDate - DateTime.Now;
                if (remainingTime.TotalSeconds > 0)
                {
                    return (int)remainingTime.TotalSeconds;
                }
                return -1; // Return 0 if the remaining time is non-positive (i.e., if it's already expired).
            }
        }

        /// <summary>
        /// Increases the current skill level.
        /// </summary>
        /// <param name="levels">Levels to increase</param>
        public bool IncreaseSkillLevel(byte levels = 1)
        { 
            if (CurrentLevel == MaxLevel || CurrentLevel + levels > MaxLevel)
                return false;

            CurrentLevel += levels;

            return true;
        }

        public void SetCooldown(int duration)
        {
            Duration = duration;
            EndDate = DateTime.Now.AddSeconds(duration);
        }

        public void SetId(long id)
        {
            Id = id;
        }
        public void ResetCooldown()
        {
            Duration = 0;
            EndDate = DateTime.MaxValue;
        }
    }
}
