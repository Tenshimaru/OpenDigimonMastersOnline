using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public partial class Drop
    {
        public long Id { get; private set; }

        public long OwnerId { get; private set; }

        public int OwnerHandler { get; private set; }

        public int GeneralHandler { get; private set; }

        public bool Collected { get; private set; }

        public ItemModel DropInfo { get; private set; }

        public Location Location { get; private set; }

        public DateTime Expiration { get; private set; }

        public DateTime LoseOwnership { get; private set; }

        public bool Thrown { get; private set; }

        public bool Lost { get; private set; }

        public bool Processing { get; set; }

        public Drop()
        {
            DropInfo = new ItemModel();
            Location = new Location();
        }
    }
}
