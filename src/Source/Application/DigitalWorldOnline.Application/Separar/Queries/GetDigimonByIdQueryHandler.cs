using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetDigimonByIdQueryHandler : IRequestHandler<GetDigimonByIdQuery, DigimonDTO?>
    {
        private readonly ICharacterQueriesRepository _repository;

        public GetDigimonByIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<DigimonDTO?> Handle(GetDigimonByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonByIdAsync(request.DigimonId);
        }
    }
}