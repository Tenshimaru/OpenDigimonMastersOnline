using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerBaseStatusQuery : IRequest<CharacterBaseStatusAssetDTO>
    {
        public CharacterModelEnum Type { get; private set; }

        public TamerBaseStatusQuery(CharacterModelEnum type)
        {
            Type = type;
        }
    }
}