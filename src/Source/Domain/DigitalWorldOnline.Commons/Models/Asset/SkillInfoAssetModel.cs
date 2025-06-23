using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class SkillInfoAssetModel
    {
        /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Skill id
        /// </summary>
        public int SkillId { get; private set; }
        
        /// <summary>
        /// Skill name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Skill Digimon Family Type
        /// </summary>
        public byte FamilyType { get; private set; }

        /// <summary>
        /// DS usage
        /// </summary>
        public int DSUsage { get; private set; }
        
        /// <summary>
        /// HP usage
        /// </summary>
        public int HPUsage { get; private set; }
        
        /// <summary>
        /// Damage or Healling value
        /// </summary>
        public int Value { get; private set; }
        
        /// <summary>
        /// Total casting time
        /// </summary>
        public float CastingTime { get; private set; }
        
        /// <summary>
        /// Cooldown in seconds
        /// </summary>
        public int Cooldown { get; private set; }
        
        /// <summary>
        /// Max skill upgrade level
        /// </summary>
        public byte MaxLevel { get; private set; }
        
        /// <summary>
        /// Required skill points to level up the skill
        /// </summary>
        public byte RequiredPoints { get; private set; }
        
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
        public byte Target { get; private set; }

        /// <summary>
        /// The skill area of effect (AoE). Zero means not AoE skill.
        /// </summary>
        public int AreaOfEffect { get; private set; }
        
        /// <summary>
        /// Minimal damage of an Area of Effect skill.
        /// </summary>
        public int AoEMinDamage { get; private set; }
        
        /// <summary>
        /// Maximum damage of an Area of Effect skill.
        /// </summary>
        public int AoEMaxDamage { get; private set; }
        
        /// <summary>
        /// Range to start casting.
        /// </summary>
        public int Range { get; private set; }

        /// <summary>
        /// Type of the Skill (0 - Normal, 1 - AoE, 2 - Inplosion).
        /// </summary>
        public int SkillType { get; private set; }

        /// <summary>
        /// Level for unlocking the skill.
        /// </summary>
        public byte UnlockLevel { get; private set; }

        /// <summary>
        /// Memory chips used if this is a memory skill.
        /// </summary>
        public byte MemoryChips { get; private set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int FirstConditionCode { get; private set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int SecondConditionCode { get; private set; }
        
        /// <summary>
        /// Buff or debuff inflicted by the skill.
        /// </summary>
        public int ThirdConditionCode { get; private set; }
        
        /// <summary>
        /// The explicit type of the casted skill.
        /// </summary>
        public int Type { get; private set; }
        

        /// <summary>
        /// The description about the skill.
        /// </summary>
        public string Description { get; private set; }
       
    }
}