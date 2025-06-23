
namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterArenaPointsModel
    {
        public void UpdatePoints(int amount = 0, int itemId = 6642, int currentStage = 0)
        {
            SetItemId(6642);
            SetCurrentStage(currentStage);
        }

        public void ReductionAmount(int amount)
        {
            if (Amount - amount >= 0)
            {
                Amount -= amount;             
            }
            else
            {
                Amount = 0;
            }
        }

        public void IncreaseAmount(int amount) => Amount += amount;
        public void SetAmount(int amount) => Amount = amount;
        public void SetItemId(int itemId) => ItemId = itemId;
        public void SetCurrentStage(int currentStage) => CurrentStage = currentStage;
    }
}
