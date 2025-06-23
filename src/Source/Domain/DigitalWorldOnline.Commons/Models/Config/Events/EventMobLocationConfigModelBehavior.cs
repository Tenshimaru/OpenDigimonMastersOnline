namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed partial class EventMobLocationConfigModel
    {
        /// <summary>
        /// Creates a new location object.
        /// </summary>
        /// <param name="mapId">Map identifier.</param>
        /// <param name="x">X (vertical) position.</param>
        /// <param name="y">Y (horizontal) position</param>
        public static EventMobLocationConfigModel Create(short mapId, int x, int y)
        {
            var location = new EventMobLocationConfigModel();
            location.SetX(x);
            location.SetY(y);
            location.SetMapId(mapId);

            return location;
        }
    }
}