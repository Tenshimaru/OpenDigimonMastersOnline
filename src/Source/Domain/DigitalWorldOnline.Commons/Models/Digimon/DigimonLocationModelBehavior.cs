namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public partial class DigimonLocationModel : Location
    {
        /// <summary>
        /// Creates a new digimon location object.
        /// </summary>
        /// <param name="mapId">Map id.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public static DigimonLocationModel Create(short mapId, int x, int y)
        {
            var location = new DigimonLocationModel();
            location.SetMapId(mapId);
            location.SetX(x);
            location.SetY(y);

            return location;
        }
    }
}