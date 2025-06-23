namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterDebuffModel : Buff
    {
        /// <summary>
        /// Creates a new digimon buff.
        /// </summary>
        /// <param name="buffId">Buff id (client reference).</param>
        /// <param name="skillId">Skill id (client reference).</param>
        /// <param name="duration">Buff duration (in seconds).</param>
        public static CharacterDebuffModel Create(int buffId, int skillId, int TypeN = 0, int duration = 0, int Cooldown = 0)
        {
            var buff = new CharacterDebuffModel();
            buff.SetSkillId(skillId);
            buff.SetBuffId(buffId);
            buff.SetTypeN(TypeN);
            buff.SetCooldown(Cooldown);
            buff.IncreaseDuration(duration);
            return buff;
        }
    }
}