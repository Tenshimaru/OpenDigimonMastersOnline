using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonBaseInfoQuery : IRequest<DigimonBaseInfoAssetDTO?>
    {
        public int Type { get; private set; }

        public DigimonBaseInfoQuery(int type)
        {
            Type = type;
        }
    }
}

