using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemListBitsCommand : IRequest
    {
        public long ItemListId { get; }
        public long Bits { get; }

        public UpdateItemListBitsCommand(long itemListId, long bits)
        {
            ItemListId = itemListId;
            Bits = bits;
        }

        public UpdateItemListBitsCommand(ItemListModel itemList)
        {
            ItemListId = itemList.Id;
            Bits = itemList.Bits;
        }
    }
}