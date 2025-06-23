using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteConsignedShopCommand : IRequest
    {
        public long GeneralHandler { get; private set; }

        public DeleteConsignedShopCommand(long generalHandler)
        {
            GeneralHandler = generalHandler;
        }
    }
}