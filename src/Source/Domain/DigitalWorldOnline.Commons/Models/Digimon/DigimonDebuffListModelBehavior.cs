using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonDebuffListModel
    {
        /// <summary>
        /// Returns true if the digimon haves any active buff.
        /// </summary>
        public bool HasActiveBuffs => ActiveBuffs.Any(x => x.Duration > 0);

        /// <summary>
        /// Returns digimon's active buffs list.
        /// </summary>
        public List<DigimonDebuffModel> ActiveBuffs => Buffs.Where(x => x.BuffId > 0).ToList();

        /// <summary>
        /// Adds a new buff to the list.
        /// </summary>
        /// <param name="buff">The new buff.</param>
        public void Add(DigimonDebuffModel buff)
        {
            if (Buffs.Any(x => x.BuffId == buff.BuffId))
                return;

            Buffs.Add(buff);
        }

        /// <summary>
        /// Removes an active buff from the list.
        /// </summary>
        /// <param name="buffId">The target buff id.</param>
        public bool ForceExpired(int buffId)
        {
            var BuffToExpired = Buffs.FirstOrDefault(x => x.BuffId == buffId);

            if (BuffToExpired == null)
                return false;

            BuffToExpired.SetDuration(-1);
            BuffToExpired.SetEndDate(DateTime.Now.AddMinutes(1));

            return true;
        }

        public bool Remove(int buffId)
        {
            return Buffs.RemoveAll(x => x.BuffId == buffId) > 0;
        }

        public DigimonDebuffModel TamerBaseSkill()
        {
            return Buffs.FirstOrDefault(x => x.SkillId / 1000000 == 8 && x.Duration == 0);
        }
        public DigimonDebuffModel ActiveTamerSkill()
        {
            return Buffs.FirstOrDefault(x => x.SkillId / 1000000 == 8 && x.Cooldown >= 0);
        }

        /// <summary>
        /// Updates the target buff in buff list.
        /// </summary>
        /// <param name="buff">The buff to be updated.</param>
        public void Update(DigimonDebuffModel buff) => Buffs.FirstOrDefault(x => x.Id == buff.Id)?.IncreaseDuration(buff.Duration);

        public bool HaveActiveSkill(int skillId)
        {
            var targetSkill = Buffs.FirstOrDefault(x => x.SkillId == skillId);

            if (targetSkill != null)
                return true;

            return false;
        }

        public bool ActiveDebuffReductionDamage()
        {
            // Verifica se existe algum buff ativo que reduza o poder de ataque
            return ActiveBuffs.Any(buff =>
                buff.BuffInfo.SkillInfo.Apply.Any(apply =>
                    apply.Attribute == Enums.SkillCodeApplyAttributeEnum.AttackPowerDown));
        }

        /// <summary>
        /// Serializes buffs list.
        /// </summary>
        public byte[] ToArray()
        {
            byte[] buffer;

            using (MemoryStream m = new())
            {
                foreach (var buff in Buffs)
                    m.Write(buff.ToArray(), 0, 12);

                buffer = m.ToArray();
            }

            return buffer;
        }
    }
}
