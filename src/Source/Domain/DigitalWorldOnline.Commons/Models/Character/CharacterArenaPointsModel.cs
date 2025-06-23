
namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterArenaPointsModel
    {
        public long Id { get; private set; }

        public int ItemId { get; private set; }

        public int Amount { get; private set; }

        public int CurrentStage { get; private set; }

        public CharacterArenaPointsModel()
        {
            ItemId = 6642;
            Amount = 0;
        }

    }
}