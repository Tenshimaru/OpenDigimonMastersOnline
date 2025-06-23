using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerLevelStatusQuery : IRequest<CharacterLevelStatusAssetDTO>
    {
        public CharacterModelEnum Type { get; private set; }

        public byte Level { get; private set; }

        public TamerLevelStatusQuery(CharacterModelEnum type, byte level)
        {
            Type = type;
            Level = level;
        }
    }
}