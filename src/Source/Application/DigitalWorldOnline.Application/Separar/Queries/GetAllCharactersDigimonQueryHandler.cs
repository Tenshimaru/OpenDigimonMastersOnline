using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAllCharactersDigimonQueryHandler : IRequestHandler<GetAllCharactersDigimonQuery, List<DigimonDTO>>
    {
        private readonly ICharacterQueriesRepository _repository;

        public GetAllCharactersDigimonQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DigimonDTO>> Handle(GetAllCharactersDigimonQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllDigimonsAsync();
        }
    }
}