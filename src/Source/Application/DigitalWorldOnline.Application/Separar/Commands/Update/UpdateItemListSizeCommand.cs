using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemListSizeCommand : IRequest
    {
        public long ItemListId { get; }
        public byte NewSize { get; }

        public UpdateItemListSizeCommand(long itemListId, byte newSize)
        {
            ItemListId = itemListId;
            NewSize = newSize;
        }

        public UpdateItemListSizeCommand(ItemListModel itemList)
        {
            ItemListId = itemList.Id;
            NewSize = itemList.Size;
        }
    }
}