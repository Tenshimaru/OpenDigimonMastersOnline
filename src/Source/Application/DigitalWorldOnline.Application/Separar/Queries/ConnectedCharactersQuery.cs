using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConnectedCharactersQuery : IRequest<IList<CharacterDTO>>
    {
    }
}