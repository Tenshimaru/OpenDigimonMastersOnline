using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class AddInventorySlotsCommand : IRequest
    {
        public List<ItemModel> Items { get; }

        public AddInventorySlotsCommand(List<ItemModel> items)
        {
            Items = items;
        }
    }
}