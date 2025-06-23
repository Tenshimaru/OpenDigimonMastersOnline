namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class MobLocationConfigModel
    {
        /// <summary>
        /// Creates a new location object.
        /// </summary>
        /// <param name="mapId">Map identifier.</param>
        /// <param name="x">X (vertical) position.</param>
        /// <param name="y">Y (horizontal) position</param>
        public static MobLocationConfigModel Create(short mapId, int x, int y)
        {
            var location = new MobLocationConfigModel();
            location.SetX(x);
            location.SetY(y);
            location.SetMapId(mapId);

            return location;
        }
    }
}
