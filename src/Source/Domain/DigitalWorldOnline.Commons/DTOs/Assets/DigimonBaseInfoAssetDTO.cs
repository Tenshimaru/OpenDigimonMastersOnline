using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public class DigimonBaseInfoAssetDTO : StatusDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference for digimon type.
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// Client reference for digimon model.
        /// </summary>
        public int Model { get; set; }
        
        /// <summary>
        /// Digimon name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Base digimon level.
        /// </summary>
        public byte Level { get; set; }
        
        /// <summary>
        /// Digimon scaling type.
        /// </summary>
        public byte ScaleType { get; set; }
        
        /// <summary>
        /// Digimon evolution type.
        /// </summary>
        public int EvolutionType { get; set; }

        /// <summary>
        /// Mob attribute.
        /// </summary>
        public DigimonAttributeEnum Attribute { get; set; }

        /// <summary>
        /// Mob element.
        /// </summary>
        public DigimonElementEnum Element { get; set; }

        /// <summary>
        /// Mob main family.
        /// </summary>
        public DigimonFamilyEnum Family1 { get; set; }

        /// <summary>
        /// Mob second family.
        /// </summary>
        public DigimonFamilyEnum Family2 { get; set; }

        /// <summary>
        /// Mob third family.
        /// </summary>
        public DigimonFamilyEnum Family3 { get; set; }
    }
}