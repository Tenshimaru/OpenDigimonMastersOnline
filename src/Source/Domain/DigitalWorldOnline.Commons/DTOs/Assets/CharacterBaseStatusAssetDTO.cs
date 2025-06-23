using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class CharacterBaseStatusAssetDTO : StatusDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference tamer type for the atributes.
        /// </summary>
        public CharacterModelEnum Type { get; set; }
    }
}