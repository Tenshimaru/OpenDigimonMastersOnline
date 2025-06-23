using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Game.Models.Configuration
{
    public class GameConfigurationModel
    {
        public int? BaseCriticalDamage { get; set; }
        public AttributeConfig Attribute { get; set; }
        public ElementConfig Element { get; set; }
        public ItemDropCountConfig ItemDropCount { get; set; }

        public BitDropCountConfig BitDropCount { get; set; }

        public EvolutionChampion evolutionChampion { get; set; }
        public EvolutionUltimate evolutionUltimate { get; set; }
        public EvolutionMega evolutionMega { get; set; }

        public EvolutionBurstMode evolutionBurstMode { get; set; }

        public EvolutionCapsule evolutionCapsule { get; set; }
        public EvolutionJogress evolutionJogress { get; set; }


        public class AttributeConfig
        {
            public bool ApplyDamage { get; set; }
            public double AdvantageMultiplier { get; set; }
            public double DisAdvantageMultiplier { get; set; }
        }

        public class ElementConfig
        {
            public bool ApplyDamage { get; set; }
            public double AdvantageMultiplier { get; set; }
            public double DisAdvantageMultiplier { get; set; }
        }

        public class ItemDropCountConfig
        {
            public bool ApplyDropAddition { get; set; }
            public int MultiplyDropCount { get; set; }
        }

        public class BitDropCountConfig
        {
            public bool ApplyDropAddition { get; set; }
            public int MultiplyDropCount { get; set; }
        }

        public class EvolutionChampion
        {
            public bool Apply { get; set; }
            public int Level { get; set; }

        }
        public class EvolutionUltimate
        {
            public bool Apply { get; set; }
            public int Level { get; set; }
        }
        public class EvolutionMega
        {
            public bool Apply { get; set; }
            public int Level { get; set; }
        }
        public class EvolutionCapsule
        {
            public bool Apply { get; set; }
            public int Level { get; set; }
        }
        public class EvolutionBurstMode
        {
            public bool Apply { get; set; }
            public int Level { get; set; }
        }
        public class EvolutionJogress
        {
            public bool Apply { get; set; }
            public int Level { get; set; }
        }
    }
}