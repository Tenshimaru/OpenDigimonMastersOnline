using DigitalWorldOnline.Commons.Enums;
using System.Text.Json.Serialization;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public partial  class ItemSocketStatusModel
    { /// <summary>
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
        [JsonIgnore]
        public ItemModel Item { get; set; }
        public Guid ItemId { get; set; }
        public ItemSocketStatusModel(byte slot)
        {
            Id = Guid.NewGuid();
            Slot = slot;
        }
    }
}

