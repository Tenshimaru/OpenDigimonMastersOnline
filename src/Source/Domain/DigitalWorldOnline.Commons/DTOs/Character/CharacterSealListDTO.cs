namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public sealed class CharacterSealListDTO
    {
        public long Id { get; set; }
        public short SealLeaderId { get; set; }

        public IList<CharacterSealDTO> Seals { get; set; }

        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}
