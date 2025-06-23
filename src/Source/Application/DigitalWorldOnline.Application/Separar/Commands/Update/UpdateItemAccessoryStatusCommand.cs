using DigitalWorldOnline.Commons.Models.Base;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemAccessoryStatusCommand : IRequest
    {
        public ItemModel Item { get; }

        public UpdateItemAccessoryStatusCommand(ItemModel item)
        {
            Item = item;
        }
    }
}