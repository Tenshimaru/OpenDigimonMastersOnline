namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonDebuffModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long BuffListId { get; set; }

        public DigimonDebuffModel()
        {
            Id = Guid.NewGuid();
        }
    }
}