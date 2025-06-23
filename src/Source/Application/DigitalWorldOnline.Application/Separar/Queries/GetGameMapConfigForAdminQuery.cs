using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetGameMapConfigForAdminQuery : IRequest<List<GetGameMapConfigForAdminQueryDto>>
    {
    }
}