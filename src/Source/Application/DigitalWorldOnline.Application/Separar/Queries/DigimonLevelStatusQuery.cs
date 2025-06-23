using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonLevelStatusQuery : IRequest<DigimonLevelStatusAssetDTO>
    {
        public int Type { get; private set; }

        public byte Level { get; private set; }

        public DigimonLevelStatusQuery(int type, byte level)
        {
            Type = type;
            Level = level;
        }
    }
}