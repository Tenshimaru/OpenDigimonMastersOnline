using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Events
{
    public sealed partial class TimeRewardModel
    {
        public bool ReedemRewards => StartTime.Date < DateTime.Now.Date;
        public DateTime LastTimeRewardUpdate = DateTime.Now;

        private int _currentTime = 0;

        public int CurrentTime
        {
            get
            {
                return _currentTime;
            }

            set
            {
                _currentTime = value;
            }
        }

        public int RemainingTime
        {
            get
            {
                switch (RewardIndex)
                {
                    default: return -1;

                    case TimeRewardIndexEnum.First:
                        return (int)(TimeRewardDurationEnum.First - AtualTime); 
                    case TimeRewardIndexEnum.Second:
                        return (int)(TimeRewardDurationEnum.Second - AtualTime);
                    case TimeRewardIndexEnum.Third:
                        return (int)(TimeRewardDurationEnum.Third - AtualTime);
                    case TimeRewardIndexEnum.Fourth:
                        return (int)(TimeRewardDurationEnum.Fourth - AtualTime);
                }
            }
        }

        public void SetStartTime()
        {
            StartTime = DateTime.Now;
            RewardIndex = TimeRewardIndexEnum.First;
        }

        public void SetLastTimeRewardDate()
        {
            LastTimeRewardUpdate = DateTime.Now;
        }

        public void SetAtualTime()
        {
            AtualTime = CurrentTime;
        }

        public bool TimeCompleted()
        {
            return RewardIndex switch
            {
                TimeRewardIndexEnum.First => AtualTime >= (int)TimeRewardDurationEnum.First,
                TimeRewardIndexEnum.Second => AtualTime >= (int)TimeRewardDurationEnum.Second,
                TimeRewardIndexEnum.Third => AtualTime >= (int)TimeRewardDurationEnum.Third,
                TimeRewardIndexEnum.Fourth => AtualTime >= (int)TimeRewardDurationEnum.Fourth,
                _ => false,
            };
        }

    }
}