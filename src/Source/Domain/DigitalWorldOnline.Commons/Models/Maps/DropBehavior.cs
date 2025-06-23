using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public partial class Drop
    {
        private const int HandlerRange = 49153;

        public bool BitsDrop => DropInfo.ItemId == 90600;

        public bool Expired => DateTime.Now > Expiration;

        public bool NoOwner => OwnerHandler == 0 || DateTime.Now > LoseOwnership;

        /// <summary>
        /// Sets the current drop general handler.
        /// </summary>
        /// <param name="handler">Sequential value to attach into the final value</param>
        public void SetHandlerValue(short handler) => GeneralHandler = HandlerRange + handler;

        /// <summary>
        /// Updates the drop lost status.
        /// </summary>
        /// <param name="lost">Lost status</param>
        public void SetLost(bool lost = true) => Lost = lost;

        public void SetCollected(bool collected) => Collected = collected;

        public Drop(long id, long ownerId, ushort ownerHandler, ItemModel itemInfo, Location location, bool throwAway = false)
        {
            Id = id;
            OwnerId = ownerId;
            OwnerHandler = ownerHandler;
            DropInfo = itemInfo;
            Location = location;
            Expiration = DateTime.Now.AddSeconds(90); //TODO: externalizar
            LoseOwnership = DateTime.Now.AddSeconds(60);
            Thrown = throwAway;
        }
    }
}