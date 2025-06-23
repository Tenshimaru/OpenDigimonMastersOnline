namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterFoeModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Foe name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Detailed annotation.
        /// </summary>
        public string Annotation { get; private set; }

        /// <summary>
        /// Foe character id
        /// </summary>
        public long FoeId { get; private set; }

        /// <summary>
        /// Foe character id
        /// </summary>
        public long CharacterId { get; private set; }

        /// <summary>
        /// Foe character id
        /// </summary>
        public CharacterModel Character { get; private set; }

        /// <summary>
        /// Foe character id
        /// </summary>
        public CharacterModel Foe { get; private set; }
    }
}