using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public sealed class ItemSocketStatusDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Status value.
        /// </summary>
        public short AttributeId { get; set; }

        /// <summary>
        /// Status type enumeration.
        /// </summary>
        public AccessoryStatusTypeEnum Type { get; set; }

        /// <summary>
        /// Status value.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Status slot.
        /// </summary>
        public byte Slot { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public ItemDTO Item { get; set; }
        public Guid ItemId { get; set; }
    }
}
