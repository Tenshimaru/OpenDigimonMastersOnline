using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class EvolutionArmorAssetsQuery : IRequest<List<EvolutionArmorAssetDTO>>
    {
    }
}

