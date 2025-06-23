using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Character;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonDTO
    {
        public long Id { get; set; }
        public int BaseType { get; set; }
        public int Model { get; set; }
        public byte Level { get; set; }
        [MinLength(6)]
        public string Name { get; set; }
        public short Size { get; set; }
        public long CurrentExperience { get; set; }
        public long CurrentSkillExperience { get; set; }
        public long TranscendenceExperience { get; set; }
        public DateTime CreateDate { get; set; }
        public DigimonHatchGradeEnum HatchGrade { get; set; }
        public int CurrentType { get; set; }
        public byte Friendship { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentDs { get; set; }
        public byte Slot { get; set; }
        
        //Refs
        public DigimonBuffListDTO BuffList { get; set; }
        public List<DigimonEvolutionDTO> Evolutions { get; set; }
        public DigimonDigicloneDTO Digiclone { get; set; }
        public DigimonAttributeExperienceDTO AttributeExperience { get; set; }
        public DigimonLocationDTO Location { get; set; }

        //FK
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}
