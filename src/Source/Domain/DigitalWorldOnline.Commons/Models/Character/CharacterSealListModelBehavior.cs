namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterSealListModel
    {
        /// <summary>
        /// Returns the first seal with search criteria.
        /// </summary>
        /// <param name="sequentialOrId">Seal id or sequential id.</param>
        public CharacterSealModel? FindSeal(int sequentialOrId) => FindSealById(sequentialOrId) ?? FindSealBySequential((short)sequentialOrId);

        /// <summary>
        /// Set the target seal as favorite.
        /// </summary>
        /// <param name="sealId">Target seal id.</param>
        /// <param name="sequential">Target seal sequential id.</param>
        /// <param name="favorited">Favorite</param>
        public void SetFavorite(int sealId, short sequential, bool favorited)
        {
            var targetSeal = FindSealById(sealId);

            if (targetSeal != null)
                targetSeal.SetFavorite(favorited);
            else
                AddSeal(sealId, sequential, favorited);
        }

        /// <summary>
        /// Set the target seal as leader.
        /// </summary>
        /// <param name="sequential">Target seal sequential id.</param>
        public void SetLeader(short sequential)
        {
            SealLeaderId = sequential;
        }

        /// <summary>
        /// Adds a new seal to the list or updates it if already exists.
        /// </summary>
        /// <param name="sealId">Target seal id.</param>
        /// <param name="amount">Seal amount.</param>
        /// <param name="sequential">Target seal sequential id.</param>
        public void AddOrUpdateSeal(int sealId, short amount, short sequential)
        {
            var targetSeal = FindSealById(sealId);

            if (targetSeal != null)
                targetSeal.IncreaseAmount(amount);
            else
                AddSeal(sealId, amount, sequential);
        }

        private void AddSeal(int sealId, short amount, short sequential) => Seals.Add(CharacterSealModel.Create(sealId, amount, sequential));
        private void AddSeal(int sealId, short sequential, bool favorited) => Seals.Add(CharacterSealModel.Create(sealId, sequential, favorited));
        private CharacterSealModel? FindSealById(int id) => Seals.FirstOrDefault(x => x.SealId == id);
        private CharacterSealModel? FindSealBySequential(short sequential) => Seals.FirstOrDefault(x => x.SequentialId == sequential);
    }
}