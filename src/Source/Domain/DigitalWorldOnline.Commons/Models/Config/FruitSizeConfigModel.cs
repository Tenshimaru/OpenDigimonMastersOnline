using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Config
{
    public class FruitSizeConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Hatch grade.
        /// </summary>
        public DigimonHatchGradeEnum HatchGrade { get; private set; }

        /// <summary>
        /// Target size.
        /// </summary>
        public double Size { get; private set; }

        /// <summary>
        /// Target size.
        /// </summary>
        public double MinSize { get; private set; }

        /// <summary>
        /// Target size.
        /// </summary>
        public double MaxSize { get; private set; }

        /// <summary>
        /// Target chance.
        /// </summary>
        public double Chance { get; private set; }
    }
}