namespace DigitalWorldOnline.Commons.Models
{
    public partial class Location
    {
        /// <summary>
        /// Updates the current value of X.
        /// </summary>
        /// <param name="x">The new value</param>
        public void SetX(int x) => X = x;

        /// <summary>
        /// Updates the current value of Y.
        /// </summary>
        /// <param name="y">The new value</param>
        public void SetY(int y) => Y = y;
        
        /// <summary>
        /// Updates the current value of Z.
        /// </summary>
        /// <param name="z">The new value</param>
        public void SetZ(float z) => Z = z;

        public void SetTicksCount(int ticksCount) => TicksCount = ticksCount;
        /// <summary>
        /// Updates the current value of MapId.
        /// </summary>
        /// <param name="mapId">The new value</param>
        public void SetMapId(short mapId) => MapId = mapId;

        public Location()
        {
            
        }

        public Location(short mapId, int x, int y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }
    }
}
