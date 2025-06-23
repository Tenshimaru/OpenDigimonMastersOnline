namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterProgressModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Byte array for completed quests (through bitwise operation).
        /// </summary>
        public byte[] CompletedData { get; set; } = new byte[768];
        
        /// <summary>
        /// Byte array for completed achievements (through bitwise operation).
        /// </summary>
        //public byte[] CompletedAchievmentData { get; private set; } = new byte[340];
        
        /// <summary>
        /// Client id references for achievements and quests.
        /// </summary>
        public int[] CompletedDataValue { get;  set; } = new int[192];

        /// <summary>
        /// In progress quest list.
        /// </summary>
        public List<InProgressQuestModel> InProgressQuestData { get;  set; } = new();
    }
}
