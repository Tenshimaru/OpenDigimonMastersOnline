using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MonsterSkillInfoAssetModel
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
        /// Damage or Healling value
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Damage or Healling value
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Total casting time
        /// </summary>
        public int CastingTime { get; set; }

        /// <summary>
        /// Cooldown in seconds
        /// </summary>
        public int Cooldown { get; set; }

        /// <summary>
        /// the Target Quantity 
        /// </summary>
        public byte TargetCount { get; set; }

        /// <summary>
        /// the Target Quantity 
        /// </summary>
        public byte TargetMin { get; set; }

        /// <summary>
        /// the Target Quantity 
        /// </summary>
        public byte TargetMax { get; set; }

        /// <summary>
        /// The skill area of effect (AoE). Zero means not AoE skill.
        /// </summary>
        public byte UseTerms { get; set; }

        /// <summary>
        /// Minimal damage of an Area of Effect skill.
        /// </summary>
        public int RangeId { get; set; }

        /// <summary>
        /// Maximum damage of an Area of Effect skill.
        /// </summary>
        public float AnimationDelay { get; set; }

        /// <summary>
        /// Range to start casting.
        /// </summary>
        public byte ActiveType { get; set; }

        /// <summary>
        /// Minimal damage of an Area of Effect skill.
        /// </summary>
        public int SkillType { get; set; }

        /// <summary>
        /// Minimal damage of an Area of Effect skill.
        /// </summary>
        public float NoticeTime { get; set; }

        /// <summary>
        /// The explicit type of the casted skill.
        /// </summary>
        public int Type { get; set; }
    }
}
