using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonArchiveItemModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Archive slot.
        /// </summary>
        public int Slot { get; private set; }

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

        public CharacterDigimonArchiveItemModel(int slot)
        {
            Id = Guid.NewGuid();
            Slot = slot;
        }

        public void SetSlot(int archiveSlot)
        {
            Slot = archiveSlot;
        }
    }
}
