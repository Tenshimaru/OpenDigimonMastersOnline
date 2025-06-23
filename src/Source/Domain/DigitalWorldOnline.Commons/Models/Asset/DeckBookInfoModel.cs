

using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class DeckBookInfoModel
    {
        /// <summary>
        /// Unique identifier for the deck buff option.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Gets or sets the group identifier associated with the deck buff asset.
        /// </summary>
        public required int OptionId { get; set; }
        
        /// <summary>
        /// Reference to the option name
        /// </summary>
        public required DeckBookInfoTypesEnum Type { get; set; }

        /// <summary>
        /// Reference to the option name
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Option Explain
        /// </summary>
        public required string Explain { get; set; }

        public List<DeckBuffOptionModel> Options { get; set; }

        /// <summary>
        /// DTO representing a Deck Buff Asset with various options.
        /// </summary>
        public DeckBookInfoModel()
        {
            Options = new List<DeckBuffOptionModel>(3);
        }
    }
}
