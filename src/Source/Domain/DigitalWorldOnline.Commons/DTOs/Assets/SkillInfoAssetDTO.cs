using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class SkillInfoAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Skill id
        /// </summary>
        public int SkillId { get; set; }
        
        /// <summary>
        /// Skill name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Skill Digimon Family Type
        /// </summary>
        public byte FamilyType { get; set; }

        /// <summary>
        /// DS usage
        /// </summary>
        public int DSUsage { get; set; }
        
        /// <summary>
        /// HP usage
        /// </summary>
        public int HPUsage { get; set; }
        
        /// <summary>
        /// Damage or Healling value
        /// </summary>
        public int Value { get; set; }
        
        /// <summary>
        /// Total casting time
        /// </summary>
        public float CastingTime { get; set; }
        
        /// <summary>
        /// Cooldown in seconds
        /// </summary>
        public int Cooldown { get; set; }
        
        /// <summary>
        /// Max skill upgrade level
        /// </summary>
        public byte MaxLevel { get; set; }
        
        /// <summary>
        /// Required skill points to level up the skill
        /// </summary>
        public byte RequiredPoints { get; set; }
        
        /// <list type="number">
        ///     <listheader>
        ///         <term>Skill target type</term>
        ///         <description>The type of the skill target</description>
        ///     </listheader>
        ///     <item>
        ///         <term>51</term>
        ///         <description>Single target and single shot damage</description>
        ///     </item>
        ///     <item>
        ///         <term>21</term>
        ///         <description>Discover...</description>
        ///     </item>
        /// </list>
        public byte Target { get; set; }

        /// <summary>
        /// The skill area of effect (AoE). Zero means not AoE skill.
        /// </summary>
        public int AreaOfEffect { get; set; }
        
        /// <summary>
        /// Minimal damage of an Area of Effect skill.
        /// </summary>
        public int AoEMinDamage { get; set; }
        
        /// <summary>
        /// Maximum damage of an Area of Effect skill.
        /// </summary>
        public int AoEMaxDamage { get; set; }
        
        /// <summary>
        /// Range to start casting.
        /// </summary>
        public int Range { get; set; }

        public int SkillType { get; set; }

        /// <summary>
        /// Level for unlocking the skill.
        /// </summary>
        public byte UnlockLevel { get; set; }
        
        /// <summary>
        /// Memory chips used if this is a memory skill.
        /// </summary>
        public byte MemoryChips { get; set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int FirstConditionCode { get; set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int SecondConditionCode { get; set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int ThirdConditionCode { get; set; }
        
        /// <summary>
        /// The explicit type of the casted skill.
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// The description about the skill.
        /// </summary>
        public string Description { get; set; }
    }
}