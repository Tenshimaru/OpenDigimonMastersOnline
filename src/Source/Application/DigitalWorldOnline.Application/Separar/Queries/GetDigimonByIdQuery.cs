using MediatR;
using DigitalWorldOnline.Commons.DTOs.Digimon;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetDigimonByIdQuery : IRequest<DigimonDTO?>
    {
        public long DigimonId { get; }

        public GetDigimonByIdQuery(long digimonId)
        {
            DigimonId = digimonId;
        }
    }
}