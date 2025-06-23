using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemAccessoryStatusCommandHandler : IRequestHandler<UpdateItemAccessoryStatusCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateItemAccessoryStatusCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateItemAccessoryStatusCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateItemAccessoryStatusAsync(request.Item);

            return Unit.Value;
        }
    }
}