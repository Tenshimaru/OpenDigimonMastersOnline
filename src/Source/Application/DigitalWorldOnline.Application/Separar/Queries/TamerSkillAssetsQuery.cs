using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerSkillAssetsQuery : IRequest<List<TamerSkillAssetDTO>>
    {
    }
}