using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Server;

namespace DigitalWorldOnline.Commons.ViewModel.Servers
{
    public class ServerViewModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Server name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Server experience multiplier.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Server maintenance status.
        /// </summary>
        public bool Maintenance { get; set; }

        /// <summary>
        /// Server new status.
        /// </summary>
        public bool New { get; set; }

        /// <summary>
        /// Server overload status.
        /// </summary>
        public ServerOverloadEnum Overload { get; set; }
        
        /// <summary>
        /// Server type enumeration.
        /// </summary>
        public ServerTypeEnum Type { get; set; }

        /// <summary>
        /// Server port address.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Server create date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
