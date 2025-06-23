using DigitalWorldOnline.Commons.Enums.Server;

namespace DigitalWorldOnline.Commons.ViewModel.Server
{
    public class ServerUpdateViewModel
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
        /// Server maintenance state.
        /// </summary>
        public bool Maintenance { get; set; }

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

        public ServerUpdateViewModel() { }

        public ServerUpdateViewModel(
            long id,
            string name,
            int experience,
            bool maintenance,
            ServerTypeEnum type,
            int port)
        {
            Id = id;
            Name = name;
            Experience = experience;
            Maintenance = maintenance;
            Type = type;
            Port = port;
        }
    }
}