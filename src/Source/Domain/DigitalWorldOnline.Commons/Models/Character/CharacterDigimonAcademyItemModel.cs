using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonAcademyItemModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Academy slot.
        /// </summary>
        public int Slot { get; private set; }

        /// <summary>
        /// Academy itemId.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Digimon reference.
        /// </summary>
        public long DigimonId { get; private set; }

        /// <summary>
        /// Digimon info.
        /// </summary>
        public DigimonModel? Digimon { get; set; }

        /// <summary>
        /// Reference to character.
        /// </summary>
        public Guid DigimonArchiveId { get; private set; }

        public CharacterDigimonAcademyItemModel(int slot)
        {
            Id = Guid.NewGuid();
            Slot = slot;
        }

        public void SetSlot(int academySlot)
        {
            Slot = academySlot;
        }
    }
}
