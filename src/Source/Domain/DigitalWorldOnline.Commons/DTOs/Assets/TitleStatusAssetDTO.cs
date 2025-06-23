using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class TitleStatusAssetDTO : StatusDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int AchievementId { get; set; }

        public double SCD { get; set; }
        public double LASCD { get; set; }
        public double FISCD { get; set; }
        public double ICSCD { get; set; }
        public double LISCD { get; set; }
        public double STSCD { get; set; }
        public double NESCD { get; set; }
        public double DASCD { get; set; }
        public double THSCD { get; set; }
        public double WASCD { get; set; }
        public double WISCD { get; set; }
        public double WOSCD { get; set; }
    }
}