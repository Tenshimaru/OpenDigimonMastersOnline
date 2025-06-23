using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonLevelingAssetsQuery : IRequest<List<DigimonLevelStatusAssetDTO>>
    {
    }
}