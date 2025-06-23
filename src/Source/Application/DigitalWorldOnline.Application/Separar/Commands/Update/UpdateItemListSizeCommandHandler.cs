using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemListSizeCommandHandler : IRequestHandler<UpdateItemListSizeCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateItemListSizeCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateItemListSizeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateItemListSizeAsync(request.ItemListId, request.NewSize);

            return Unit.Value;
        }
    }
}