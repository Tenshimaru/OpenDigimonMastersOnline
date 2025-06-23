using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemsCommand : IRequest
    {
        public List<ItemModel> Items { get; }

        public UpdateItemsCommand(List<ItemModel> items)
        {
            Items = items;
        }

        public UpdateItemsCommand(ItemListModel itemList)
        {
            Items = itemList.Items;
        }
    }
}