namespace DigitalWorldOnline.Commons.Models.Config
{ 
    public sealed partial class MobDebuffModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long BuffListId { get; set; }

        public MobDebuffModel()
        {
            Id = Guid.NewGuid();
        }
    }
}