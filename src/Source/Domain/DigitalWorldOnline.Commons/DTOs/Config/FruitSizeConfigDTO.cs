using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class FruitSizeConfigDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Hatch grade.
        /// </summary>
        public DigimonHatchGradeEnum HatchGrade { get; set; }

        /// <summary>
        /// Target size.
        /// </summary>
        public double Size { get; set; }

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
        public double Chance { get; set; }

        /// <summary>
        /// Owner reference.
        /// </summary>
        public long FruitConfigId { get; set; }
        public FruitConfigDTO FruitConfig { get; set; }
    }
}