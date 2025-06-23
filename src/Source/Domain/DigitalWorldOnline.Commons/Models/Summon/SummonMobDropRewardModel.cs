using DigitalWorldOnline.Commons.ViewModel.Summons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Commons.Models.Summon
{
    public class SummonMobDropRewardModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Min. amount of drops.
        /// </summary>
        public byte MinAmount { get; set; }

        /// <summary>
        /// Max. amount of drops.
        /// </summary>
        public byte MaxAmount { get; set; }

        /// <summary>
        /// Item drop list
        /// </summary>
        public List<SummonMobItemDropModel> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public SummonMobBitDropModel BitsDrop { get; set; }

        public SummonMobDropRewardModel()
        {
            MinAmount = 0;
            MaxAmount = 1;
            Drops = new List<SummonMobItemDropModel>();
            BitsDrop = new SummonMobBitDropModel();
        }
    }
}
