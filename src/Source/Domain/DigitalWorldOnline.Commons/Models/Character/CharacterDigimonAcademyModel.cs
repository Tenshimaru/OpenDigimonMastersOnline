using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonAcademyModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Digimon academys.
        /// </summary>
        public List<CharacterDigimonAcademyItemModel> DigimonAcademys { get; private set; }

        /// <summary>
        /// Available academy slots.
        /// </summary>
        public int Slots { get; private set; }

        /// <summary>
        /// Reference to character.
        /// </summary>
        public long CharacterId { get; private set; }

        public CharacterDigimonAcademyModel()
        {
            Id = Guid.NewGuid();
            Slots = GeneralSizeEnum.InitialArchive.GetHashCode();
            DigimonAcademys = new List<CharacterDigimonAcademyItemModel>();
            for (int i = 0; i < GeneralSizeEnum.InitialAcademy.GetHashCode(); i++)
            {
                DigimonAcademys.Add(new CharacterDigimonAcademyItemModel(i));
            }
        }
    }
}
