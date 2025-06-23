namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterBuffModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; private set; }

        public CharacterBuffModel()
        {
            Id = Guid.NewGuid();
        }
    }
}