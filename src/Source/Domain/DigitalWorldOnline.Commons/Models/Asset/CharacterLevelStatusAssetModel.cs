using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public class CharacterLevelStatusAssetModel : StatusAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference tamer type for the atributes.
        /// </summary>
        public CharacterModelEnum Type { get; private set; }

        /// <summary>
        /// Level reference for the status.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Exp required for the next level.
        /// </summary>
        public long ExpValue { get; private set; }
    }
}