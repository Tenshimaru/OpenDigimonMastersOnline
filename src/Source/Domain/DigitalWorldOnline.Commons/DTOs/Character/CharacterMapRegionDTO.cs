namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterMapRegionDTO
    {
        public long Id { get; set; }

        public byte Unlocked { get; set; }

        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}