

using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class DeckBuffOptionModel
    {
        /// <summary>
        /// Unique identifier for the deck buff option.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Identifier for the group related to the deck buff option.
        /// </summary>
        public required int GroupIdX { get; set; }

        /// <summary>
        /// Condition under which the deck buff option applies.
        /// </summary>
        public required DeckBuffConditionsEnum Condition { get; set; }

        /// <summary>
        /// Type of the deck buff option, specifying its category or role within the deck.
        /// </summary>
        public required DeckBuffAtTypesEnum AtType { get; set; }

        /// <summary>
        /// Defines the specific option associated with the deck buff.
        /// </summary>
        public required int OptionId { get; set; }

        /// <summary>
        /// Represents the value associated with the deck buff option.
        /// </summary>
        public required int Value { get; set; }

        /// <summary>
        /// Probability value used for the deck buff option.
        /// </summary>
        public required int Prob { get; set; }

        /// <summary>
        /// Duration or timestamp associated with the deck buff option in seconds.
        /// </summary>
        public required int Time { get; set; }

        /// <summary>
        /// Represents the deck buff associated with this option.
        /// </summary>
        public DeckBuffModel? DeckBuff { get; set; }

        /// <summary>
        /// Contains information about the book associated with the deck.
        /// </summary>
        public DeckBookInfoModel? DeckBookInfo { get; set; }
    }
}
