using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ItemCraftAssetsByFilterQuery : IRequest<ItemCraftAssetDTO?>
    {
        public int NpcId { get; private set; }

        public int SeqId { get; private set; }

        public ItemCraftAssetsByFilterQuery(int npcId, int seqId)
        {
            NpcId = npcId;
            SeqId = seqId;
        }
    }
}