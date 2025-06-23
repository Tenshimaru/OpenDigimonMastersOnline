namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class DeckBuffModel
    {
        /// <summary>
        /// Unique identifier for the deck buff option.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the group identifier associated with the deck buff asset.
        /// </summary>
        public int GroupIdX { get; set; }

        ///
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the explanation or description for the deck buff asset.
        /// </summary>
        public string Explain { get; set; }

        /// <summary>
        /// Gets or sets the list of deck buff options associated with the deck buff asset.
        /// </summary>
        public List<DeckBuffOptionModel> Options { get; set; }

        /// <summary>
        /// DTO representing a Deck Buff Asset with various options.
        /// </summary>
        public DeckBuffModel()
        {
            Options = new List<DeckBuffOptionModel>();
        }
    }
}