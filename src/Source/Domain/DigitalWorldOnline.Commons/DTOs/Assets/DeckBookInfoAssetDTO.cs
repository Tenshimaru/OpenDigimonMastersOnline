using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public class DeckBookInfoAssetDTO
    {

        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// OptionID
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// Reference to the option name
        /// </summary>
        public DeckBookInfoTypesEnum Type { get; set; }

        /// <summary>
        /// Reference to the option name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Option Explain
        /// </summary>
        public required string Explain { get; set; }

        public List<DeckBuffOptionAssetDTO> Options { get; set; }
        
        public DeckBookInfoAssetDTO()
        {
            Options = new List<DeckBuffOptionAssetDTO>(3);
        }
    }
}