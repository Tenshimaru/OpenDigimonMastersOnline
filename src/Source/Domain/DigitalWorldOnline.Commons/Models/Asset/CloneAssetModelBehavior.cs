using DigitalWorldOnline.Commons.Utils;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class CloneAssetModel
    {
        /// <summary>
        /// Flag for reinforced digiclones.
        /// </summary>
        public bool Reinforced => ItemSection.IsBetween(5511, 5512, 5513, 5514, 5515);

        /// <summary>
        /// Flag for mega reinforced digiclones.
        /// </summary>
        public bool MegaReinforced => ItemSection.IsBetween(5521, 5522, 5523, 5524, 5525);

        /// <summary>
        /// Flag for mega digiclones.
        /// </summary>
        public bool Mega => ItemSection.IsBetween(5536, 5537, 5538, 5539, 5540);

        /// <summary>
        /// Flag for low digiclones.
        /// </summary>
        public bool Low => ItemSection.IsBetween(5531, 5532, 5533, 5534, 5535);

        /// <summary>
        /// Flag for normal digiclones.
        /// </summary>
        public bool Normal => ItemSection.IsBetween(5501, 5502, 5503, 5504, 5505);
    }
}