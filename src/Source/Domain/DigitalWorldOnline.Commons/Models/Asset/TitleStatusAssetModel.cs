namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class TitleStatusAssetModel : StatusAssetModel
    {
        public long Id { get; private set; }
        public int ItemId { get; private set; }
        public string Name { get; private set; }
        public int AchievementId { get; private set; }

        public double SCD { get; private set; }
        public double LASCD { get; private set; }
        public double FISCD { get; private set; }
        public double ICSCD { get; private set; }
        public double LISCD { get; private set; }
        public double STSCD { get; private set; }
        public double NESCD { get; private set; }
        public double DASCD { get; private set; }
        public double THSCD { get; private set; }
        public double WASCD { get; private set; }
        public double WISCD { get; private set; }
        public double WOSCD { get; private set; }
    }
}