namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterProgressDTO
    {
        public long Id { get; set; }
        public byte[] CompletedData { get; set; } = new byte[768];
        public int[] CompletedDataValue { get; set; } = new int[192];
        public List<InProgressQuestDTO> InProgressQuestData { get; set; }

        //FK
        public CharacterDTO Character { get; set; }
        public long CharacterId { get; set; }
    }
}
