using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemListBitsCommandHandler : IRequestHandler<UpdateItemListBitsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateItemListBitsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateItemListBitsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateItemListBitsAsync(request.ItemListId, request.Bits);

            return Unit.Value;
        }
    }
}