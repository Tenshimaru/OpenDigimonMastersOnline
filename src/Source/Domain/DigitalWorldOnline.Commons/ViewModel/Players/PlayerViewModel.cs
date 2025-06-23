using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;

namespace DigitalWorldOnline.Commons.ViewModel.Players
{
    public class PlayerViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Account identifier.
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// Tamer name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Current experience.
        /// </summary>
        public long CurrentExperience { get; set; }

        /// <summary>
        /// Current map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Connection state.
        /// </summary>
        public CharacterStateEnum State { get; set; }

        /// <summary>
        /// Event state.
        /// </summary>
        public CharacterEventStateEnum EventState { get; set; }

        /// <summary>
        /// Current channel.
        /// </summary>
        public byte Channel { get; set; }

        /// <summary>
        /// Character model.
        /// </summary>
        public CharacterModelEnum Model { get; set; }

        /// <summary>
        /// Character size.
        /// </summary>
        public short Size { get; set; }

        /// <summary>
        /// Current HP.
        /// </summary>
        public int CurrentHp { get; set; }

        /// <summary>
        /// Current DS.
        /// </summary>
        public int CurrentDs { get; set; }

        /// <summary>
        /// X Gauge.
        /// </summary>
        public int XGauge { get; set; }

        /// <summary>
        /// X Crystals.
        /// </summary>
        public short XCrystals { get; set; }

        /// <summary>
        /// Current title.
        /// </summary>
        public short CurrentTitle { get; set; }

        /// <summary>
        /// Digimon slots.
        /// </summary>
        public byte DigimonSlots { get; set; }

        /// <summary>
        /// Character position in account.
        /// </summary>
        public byte Position { get; set; }

        /// <summary>
        /// Creation date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
