namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterProgressModel
    {
        /// <summary>
        /// Serializes the progress object.
        /// </summary>
        public byte[] ToArray()
        {
            byte[] buffer = Array.Empty<byte>();

            using (MemoryStream m = new(1248))
            {
                Buffer.BlockCopy(CompletedDataValue, 0, CompletedData, 0, CompletedData.Length);

                //var temp = new byte[340];

                //m.Write(temp.ToArray(), 0, 340);
                m.Write(CompletedData.ToArray(), 0, 768);

                foreach (var inProgressQuest in InProgressQuestData)
                    m.Write(inProgressQuest.ToArray(), 0, 7);

                var remaningQuestsToAdd = 20 - InProgressQuestData.Count;
                for (int i = 0; i < remaningQuestsToAdd; i++)
                    m.Write(new InProgressQuestModel().ToArray(), 0, 7);

                buffer = m.ToArray();
            }

            return buffer;
        }

        /// <summary>
        /// Adds a new quest to the current quests list.
        /// </summary>
        /// <param name="questId">Quest identifier</param>
        /// <returns>True when the tamer has free quest slots to add this quest.</returns>
        public bool AcceptQuest(short questId)
        {
            if (InProgressQuestData.Count >= 20)
                return false;

            InProgressQuestData.Add(new InProgressQuestModel(questId));

            return true;
        }

        /// <summary>
        /// Removes the quest from the current quests list.
        /// </summary>
        /// <param name="questId">Quest identifier</param>
        public Guid? RemoveQuest(short questId)
        {
            var progress = InProgressQuestData.FirstOrDefault(x => x.QuestId == questId);
            InProgressQuestData.RemoveAll(x => x.QuestId == questId);

            return progress?.Id;
        }

        /// <summary>
        /// Updates quest progress?
        /// </summary>
        public void UpdateQuestInProgress(short questId, int goalIndex, byte value)
        {
            InProgressQuestData.FirstOrDefault(x => x.QuestId == questId)?.UpdateCondition(goalIndex, value);
        }

        public byte GetQuestGoalProgress(short questId, int goalIndex)
        {
            return InProgressQuestData.First(x => x.QuestId == questId).GetGoalValue(goalIndex);
        }
    }
}