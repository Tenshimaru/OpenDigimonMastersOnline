namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public sealed class CharacterSealDTO
    {
        public Guid Id { get; set; }
        public int SealId { get; set; }
        public short Amount { get; set; }
        public bool Favorite { get; set; }
        public short SequentialId { get; set; }

        //References
        public long SealListId { get; set; }
        public CharacterSealListDTO SealList { get; set; }
    }
}
