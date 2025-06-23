namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class InProgressQuestDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Client reference to the target quest.
        /// </summary>
        public short QuestId { get; set; }

        /// <summary>
        /// Quest first condition.
        /// </summary>
        public byte FirstCondition { get; set; }

        /// <summary>
        /// Quest second condition.
        /// </summary>
        public byte SecondCondition { get; set; }

        /// <summary>
        /// Quest third condition.
        /// </summary>
        public byte ThirdCondition { get; set; }

        /// <summary>
        /// Quest fourth condition.
        /// </summary>
        public byte FourthCondition { get; set; }

        /// <summary>
        /// Quest fifth condition.
        /// </summary>
        public byte FifthCondition { get; set; }

        //FK
        public CharacterProgressDTO CharacterProgress { get; set; }
        public long CharacterProgressId { get; set; }
    }
}
