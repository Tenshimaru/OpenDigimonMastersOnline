namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterLocationModel : Location
    {
        /// <summary>
        /// Creates a new location object.
        /// </summary>
        /// <param name="mapId">Map identifier.</param>
        /// <param name="x">X (vertical) position.</param>
        /// <param name="y">Y (horizontal) position</param>
        /// <returns></returns>
        public static CharacterLocationModel Create(short mapId, int x, int y, long id = 0)
        {
            var location = new CharacterLocationModel();
            location.Id = id;
            location.SetX(x);
            location.SetY(y);
            location.SetMapId(mapId);

            return location;
        }
    }
}
