using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterTamerSkillDTO
    {
        public Guid Id { get; set; }
        public TamerSkillTypeEnum Type { get; set; }
        public int SkillId { get; set; }
        public int Cooldown { get; set; }
        public int Duration { get; set; }
        public DateTime EndCooldown { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public CharacterDTO Character { get; set; }
        public long CharacterId { get; set; }
    }
}
