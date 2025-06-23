namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class KillSpawnConfigModel
    {

        public bool Spawn ()
        {
            var sourceMobList = SourceMobs.Where(x => x.CurrentSourceMobRequiredAmount == 0).ToList();

            if(sourceMobList.Count == SourceMobs.Count)
                return true;

            return false;
        }
        public void DecreaseTempMobs(KillSpawnTargetMobConfigModel mob)
        {
            if (TempMobs == null)
                TempMobs = TargetMobs.ToList();

            TempMobs.Remove(mob);
        }
        public void ResetCurrentSourceMobAmount()
        {       

            if (TempMobs != null )
            {
                TempMobs = null;

                foreach (var mob in SourceMobs)
                {
                    mob.ResetCurrentSourceMobAmount();
                }
            }
            else
            {
                foreach (var mob in SourceMobs)
                {
                    mob.ResetCurrentSourceMobAmount();
                }
            }
        }


    }
}
