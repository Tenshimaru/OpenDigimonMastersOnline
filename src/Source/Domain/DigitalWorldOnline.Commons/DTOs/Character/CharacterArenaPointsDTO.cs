using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterArenaPointsDTO
    {
        public long Id { get;  set; }

        public int ItemId { get; set; }

        public int Amount { get;  set; }

        public int CurrentStage { get;  set; }

        //References
        public long CharacterId { get; private set; }
        public CharacterDTO Character { get; private set; }
    }
}