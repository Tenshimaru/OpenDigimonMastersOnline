using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterBuffListModel
    {
        /// <summary>
        /// Returns true if the character haves any active buff.
        /// </summary>
        public bool HasActiveBuffs => ActiveBuffs.Any(x => x.Duration > 0);

        /// <summary>
        /// Returns character's active buffs list.
        /// </summary>
        public List<CharacterBuffModel> ActiveBuffs => Buffs.Where(x => x.BuffId > 0).ToList();

        /// <summary>
        /// Adds a new buff to the list.
        /// </summary>
        /// <param name="buff">The new buff.</param>
        public void Add(CharacterBuffModel buff)
        {
            if (Buffs.Any(x => x.BuffId == buff.BuffId)) return;
            Buffs.Add(buff);
        }

        public CharacterBuffModel ActiveTamerSkill()
        {
            return Buffs.FirstOrDefault(x => x.SkillId / 1000000 == 8 && x.Cooldown >= 0);
        }
        /// <summary>
        /// Removes an active buff from the list.
        /// </summary>
        /// <param name="buffId">The target buff id.</param>
        public bool ForceExpired(int buffId)
        {
            var BuffToExpired = Buffs.FirstOrDefault(x => x.BuffId == buffId);

            if(BuffToExpired == null) 
                return false;

            BuffToExpired.SetDuration(-1);
            BuffToExpired.SetEndDate(DateTime.Now.AddMinutes(1));
           
            return true;
        }

        public void RemoveAll()
        {
             Buffs.Clear();
        }

        /// <summary>
        /// Removes an active buff from the list.
        /// </summary>
        /// <param name="buffId">The target buff id.</param>
        public bool Remove(int buffId)
        {
            return Buffs.RemoveAll(x => x.BuffId == buffId) > 0;
        }
        /// <summary>
        /// Updates the target buff in buff list.
        /// </summary>
        /// <param name="buff">The buff to be updated.</param>
        public void Update(CharacterBuffModel buff) => Buffs.FirstOrDefault(x => x.Id == buff.Id)?.IncreaseDuration(buff.Duration);

        /// <summary>
        /// Serializes buffs list.
        /// </summary>
        public byte[] ToArray()
        {
            byte[] buffer = Array.Empty<byte>();

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
