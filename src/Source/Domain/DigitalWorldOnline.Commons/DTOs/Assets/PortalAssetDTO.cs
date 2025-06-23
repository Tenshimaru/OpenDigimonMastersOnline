using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class PortalAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public int Id { get; set; }

        public PortalTypeEnum Type { get; set; }

        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public int NpcId { get; set; }

        /// <summary>
        /// Reference to the target map.
        /// </summary>
        public int DestinationMapId { get; set; }

        /// <summary>
        /// Destination X position.
        /// </summary>
        public int DestinationX { get; set; }
        
        /// <summary>
        /// Destiantion Y position.
        /// </summary>
        public int DestinationY { get; set; }

        public int PortalIndex { get; set; }

        public bool IsLocal { get; set; }

    }
}