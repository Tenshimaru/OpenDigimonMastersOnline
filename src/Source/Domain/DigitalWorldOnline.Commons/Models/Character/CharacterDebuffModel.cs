namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterDebuffModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long BuffListId { get; set; }

        public CharacterDebuffModel()
        {
            Id = Guid.NewGuid();
        }
    }
}