namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonBuffModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long BuffListId { get; set; }

        public DigimonBuffModel()
        {
            Id = Guid.NewGuid();
        }
    }
}