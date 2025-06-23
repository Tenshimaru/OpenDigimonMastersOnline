using MediatR;
using DigitalWorldOnline.Commons.DTOs.Digimon;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAllCharactersDigimonQuery : IRequest<List<DigimonDTO>>
    {
        public GetAllCharactersDigimonQuery()
        {
            
        }
    }
}