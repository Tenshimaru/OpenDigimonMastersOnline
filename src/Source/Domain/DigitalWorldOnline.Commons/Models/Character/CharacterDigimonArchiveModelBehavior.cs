namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonArchiveModel
    {
        public void AddSlot()
        {
            DigimonArchives.Add(new CharacterDigimonArchiveItemModel(DigimonArchives.Max(x => x.Slot) + 1));
            Slots++;
        }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public int ArchivePrice(byte? digimonLevel)
        {
            if (digimonLevel == null)
                return 0;

            var priceMultiplier = 2.2f;
            switch (digimonLevel)
            {
                case byte n when (n <= 40):
                    priceMultiplier = 1;
                    break;

                case byte n when (n <= 65):
                    priceMultiplier = 1.3f;
                    break;

                case byte n when (n <= 75):
                    priceMultiplier = 1.5f;
                    break;

                case byte n when (n <= 99):
                    priceMultiplier = 1.8f;
                    break;
            }

            return (int)(priceMultiplier * digimonLevel * digimonLevel);
        }
    }
}
