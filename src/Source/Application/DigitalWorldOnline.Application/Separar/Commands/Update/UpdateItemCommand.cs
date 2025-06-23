using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemCommand : IRequest
    {
        public ItemModel Item { get; }

        public UpdateItemCommand(ItemModel item)
        {
            Item = item;
        }
    }
}