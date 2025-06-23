using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class FruitConfigsQuery : IRequest<List<FruitConfigDTO>>
    {
    }
}