namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class MobDebuffListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public List<MobDebuffModel> Buffs { get; set; }

        public MobDebuffListModel()
        {
            Buffs = new List<MobDebuffModel>();
        }
    }
}