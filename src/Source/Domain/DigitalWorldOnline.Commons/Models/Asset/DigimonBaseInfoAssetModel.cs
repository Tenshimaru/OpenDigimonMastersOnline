using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public partial class DigimonBaseInfoAssetModel : StatusAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client reference for digimon type.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Client reference for digimon model.
        /// </summary>
        public int Model { get; private set; }

        /// <summary>
        /// Digimon name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Base digimon level.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Digimon scaling type.
        /// </summary>
        public byte ScaleType { get; private set; }

        /// <summary>
        /// Digimon evolution type.
        /// </summary>
        public int EvolutionType { get; private set; }

        /// <summary>
        /// Digimon attribute.
        /// </summary>
        public DigimonAttributeEnum Attribute { get; private set; }

        /// <summary>
        /// Digimon element.
        /// </summary>
        public DigimonElementEnum Element { get; private set; }

        /// <summary>
        /// Digimon main family.
        /// </summary>
        public DigimonFamilyEnum Family1 { get; private set; }

        /// <summary>
        /// Digimon second family.
        /// </summary>
        public DigimonFamilyEnum Family2 { get; private set; }

        /// <summary>
        /// Digimon third family.
        /// </summary>
        public DigimonFamilyEnum Family3 { get; private set; }
    }
}