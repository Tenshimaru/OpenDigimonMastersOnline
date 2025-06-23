using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateItemSocketStatusCommandHandler : IRequestHandler<UpdateItemSocketStatusCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateItemSocketStatusCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateItemSocketStatusCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateItemSocketStatusAsync(request.Item);

            return Unit.Value;
        }
    }
}