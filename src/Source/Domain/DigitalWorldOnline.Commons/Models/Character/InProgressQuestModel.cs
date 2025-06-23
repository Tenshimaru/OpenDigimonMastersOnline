namespace DigitalWorldOnline.Commons.Models.Character
{
    public class InProgressQuestModel
    {
        public Guid Id { get; private set; }
        public short QuestId { get; private set; }
        public byte FirstCondition { get; private set; }
        public byte SecondCondition { get; private set; }
        public byte ThirdCondition { get; private set; }
        public byte FourthCondition { get; private set; }
        public byte FifthCondition { get; private set; }

        public InProgressQuestModel() { }

        public InProgressQuestModel(short questId)
        {
            Id = Guid.NewGuid();
            QuestId = questId;
        }

        public byte[] ToArray()
        {
            byte[] buffer;

            using (var m = new MemoryStream(7))
            {
                m.Write(BitConverter.GetBytes(QuestId), 0, 2);
                m.WriteByte(FirstCondition);
                m.WriteByte(SecondCondition);
                m.WriteByte(ThirdCondition);
                m.WriteByte(FourthCondition);
                m.WriteByte(FifthCondition);

                buffer = m.ToArray();
            }

            return buffer;
        }

        public void UpdateCondition(int condIDX, byte condValue)
        {
            switch (condIDX)
            {
                case 0:
                    FirstCondition = condValue;
                    break;
                case 1:
                    SecondCondition = condValue;
                    break;
                case 2:
                    ThirdCondition = condValue;
                    break;
                case 3:
                    FourthCondition = condValue;
                    break;
                case 4:
                    FifthCondition = condValue;
                    break;
                default:
                    break;
            }
        }

        public byte GetGoalValue(int goalIndex)
        {
            switch (goalIndex)
            {
                case 0:
                    return FirstCondition;
                case 1:
                    return SecondCondition;
                case 2:
                    return ThirdCondition;
                case 3:
                    return FourthCondition;
                case 4:
                    return FifthCondition;
                default:
                    return 255;
            }
        }
    }
}
