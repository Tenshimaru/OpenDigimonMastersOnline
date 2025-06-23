namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterSealModel
    {
        /// <summary>
        /// Increase unlocked amount.
        /// </summary>
        /// <param name="amount">Amount to increase.</param>
        public void IncreaseAmount(short amount)
        {
            if (Amount + amount > 3000)
                Amount = 3000;
            else
                Amount += amount;
        }

        /// <summary>
        /// Decrease unlocked amount.
        /// </summary>
        /// <param name="amount">Amount to decrease.</param>
        public void DecreaseAmount(short amount) => Amount -= amount;

        /// <summary>
        /// Set the seal as favorited.
        /// </summary>
        /// <param name="favorited">Favorited.</param>
        public void SetFavorite(bool favorited) => Favorite = favorited;

        /// <summary>
        /// Creates a new seal.
        /// </summary>
        /// <param name="sealId">Seal id.</param>
        /// <param name="sequential">Seal sequential id.</param>
        /// <param name="favorited">Favorited.</param>
        public static CharacterSealModel Create(int sealId, short sequential, bool favorited)
        {
            return new CharacterSealModel()
            {
                SealId = sealId,
                Favorite = favorited,
                SequentialId = sequential
            };
        }

        /// <summary>
        /// Creates a new seal.
        /// </summary>
        /// <param name="sealId">Seal id.</param>
        /// <param name="amount">Seal unlocked amount.</param>
        /// <param name="sequential">Seal sequential id.</param>
        public static CharacterSealModel Create(int sealId, short amount, short sequential)
        {
            return new CharacterSealModel()
            {
                SealId = sealId,
                Amount = amount,
                SequentialId = sequential
            };
        }
    }
}