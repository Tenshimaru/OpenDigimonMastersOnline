using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public class DeckBuffAssetDTO
    {
        /// <summary>
        /// Unique identifier for the deck buff option.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the group identifier associated with the deck buff asset.
        /// </summary>
        public required int GroupIdX { get; set; }

        ///
        public required string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the explanation or description for the deck buff asset.
        /// </summary>
        public required string Explain { get; set; }

        /// <summary>
        /// Gets or sets the list of deck buff options associated with the deck buff asset.
        /// </summary>
        public List<DeckBuffOptionAssetDTO> Options { get; set; }

        /// <summary>
        /// Gets or sets the list of deck buff options associated with the deck buff asset.
        /// </summary>
        public List<CharacterDTO> Characters { get; set; }

        /// <summary>
        /// DTO representing a Deck Buff Asset with various options.
        /// </summary>
        public DeckBuffAssetDTO()
        {
            Options = new List<DeckBuffOptionAssetDTO>(3);
        }
    }
}