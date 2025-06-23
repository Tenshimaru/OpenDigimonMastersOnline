namespace DigitalWorldOnline.Commons.Models.Config
{
    public partial class MapConfigModel
    {
        /// <summary>
        /// Creates a new map config object.
        /// </summary>
        /// <param name="mapId">Map id.</param>
        /// <param name="name">Name.</param>
        /// <returns></returns>
        public static MapConfigModel Create(int mapId, string name)
        {
            return new MapConfigModel()
            {
                MapId = mapId,
                Name = name
            };
        }

        /// <summary>
        /// Updates the map name.
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string name) => Name = name;
        
        /// <summary>
        /// Updates the map id.
        /// </summary>
        /// <param name="mapId">New id</param>
        public void SetMapId(int mapId) => MapId = mapId;

        /// <summary>
        /// Updates the unique id.
        /// </summary>
        /// <param name="id">New id</param>
        public void SetId(int id) => DungeonId = id;

    }
}
