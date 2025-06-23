using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public class CharacterBaseStatusAssetModel : StatusAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference tamer type for the atributes.
        /// </summary>
        public CharacterModelEnum Type { get; private set; }
    }
}