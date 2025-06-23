using DigitalWorldOnline.Commons.Enums.Server;

namespace DigitalWorldOnline.Commons.ViewModel.Server
{
    public class ServerCreationViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
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
        /// Server port address.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Name);

        /// <summary>
        /// Server type enumeration.
        /// </summary>
        public ServerTypeEnum Type { get; set; }

        public ServerCreationViewModel()
        {
            Experience = 100;
            Type = ServerTypeEnum.Default;
            Port = 7041;
        }
    }
}