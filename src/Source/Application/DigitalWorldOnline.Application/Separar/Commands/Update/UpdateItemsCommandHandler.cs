using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemsCommandHandler : IRequestHandler<UpdateItemsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateItemsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateItemsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateItemsAsync(request.Items);

            return Unit.Value;
        }
    }
}