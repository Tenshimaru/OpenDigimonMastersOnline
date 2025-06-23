namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterFoeDTO
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Annotation { get; private set; }
        public long FoeId { get; private set; }

        //References
        public long CharacterId { get; private set; }
        public CharacterDTO Character { get; private set; }
        public CharacterDTO Foe { get; private set; }
    }
}