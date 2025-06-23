using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class NpcAssetsQuery : IRequest<List<NpcAssetDTO>>
    {
    }
}