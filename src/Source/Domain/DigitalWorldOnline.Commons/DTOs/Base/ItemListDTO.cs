using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public sealed class ItemListDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Item list enumeration.
        /// </summary>
        public ItemListEnum Type { get; set; }

        /// <summary>
        /// Current item list slots amount.
        /// </summary>
        public byte Size { get; set; }

        /// <summary>
        /// Item list bits.
        /// </summary>
        public long Bits { get; set; }

        /// <summary>
        /// Items inside the list.
        /// </summary>
        public List<ItemDTO> Items { get; set; }
    }
}