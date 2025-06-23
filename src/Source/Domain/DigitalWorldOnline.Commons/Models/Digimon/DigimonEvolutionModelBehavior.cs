namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonEvolutionModel
    {

        /// <summary>
        /// Increases the current skill experience.
        /// <param name="value">Amount to increase</param>
        /// </summary>
        public void IncreaseSkillExperience(int value) => SkillExperience += value;

        /// <summary>
        /// Restart the current skill experience.
        /// <param name="value">Amount to increase</param>
        /// </summary>
        public void RestartSkillExperience(int value) => SkillExperience = value;

        /// <summary>
        /// Unlocks the target evolution.
        /// </summary>
        public void Unlock(byte value = 1) => Unlocked = value;

        /// <summary>
        /// Unlocks the target evolution ride mode.
        /// </summary>
        public void UnlockRide() => Unlocked += 8;

        /// <summary>
        /// Increases the skill points amount.
        /// </summary>
        /// <param name="points">Points to increase</param>
        public void IncreaseSkillPoints(byte points = 2)
        {
            SkillPoints += points;
            SkillMastery++;
        }

        /// <summary>
        /// Decrease the skill points amount.
        /// </summary>
        /// <param name="points">Points to increase</param>
        public void DecreaseSkillPoints(byte points)
        {
            SkillPoints -= points;
        }
        /// <summary>
        /// Inserts new skills into the list.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        public void AddSkill(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Skills.Add(new DigimonEvolutionSkillModel());
        }

        public void SetId(long id)
        {
            Id = id;
        }
        /// <summary>
        /// Serializes the object into byte array.
        /// </summary>
        public byte[] ToArray()
        {
            using MemoryStream m = new();


            m.Write(BitConverter.GetBytes(GetSkillExpValue(SkillExperience, SkillMastery)), 0, 4);
            m.WriteByte(Unlocked);
            m.WriteByte(0);
            m.WriteByte(0);
            m.WriteByte(0);

            m.WriteByte(SkillPoints);

            foreach (var skill in Skills)
                m.WriteByte(skill.CurrentLevel);

            foreach (var skill in Skills)
                m.WriteByte(skill.MaxLevel);

            return m.ToArray();
        }
        public int GetSkillExpValue(int skillExp, int skillLevel)
        {
            // Calcula o valor inteiro correspondente ao nível
            int m_nSkillExpLevel = skillLevel;

            // Calcula o valor inteiro correspondente à experiência
            int m_nSkillExp = skillExp;

            // Combina os valores para formar o valor inteiro final
            int value = (m_nSkillExpLevel << 26) | m_nSkillExp;

            return value;
        }
    }
}
