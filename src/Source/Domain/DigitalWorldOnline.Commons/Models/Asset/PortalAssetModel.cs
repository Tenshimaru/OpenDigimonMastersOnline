using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class PortalAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public int Id { get; private set; }

        public PortalTypeEnum Type { get; set; }

        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public int NpcId { get;  set; }

        /// <summary>
        /// Reference to the target map.
        /// </summary>
        public int DestinationMapId { get; private set; }

        /// <summary>
        /// Destination X position.
        /// </summary>
        public int DestinationX { get; private set; }

        /// <summary>
        /// Destiantion Y position.
        /// </summary>
        public int DestinationY { get; private set; }

        public int PortalIndex { get; set; }
        public bool IsLocal { get; set; }
    }
}