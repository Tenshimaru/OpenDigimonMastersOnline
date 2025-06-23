using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAdminUsersQuery : IRequest<List<UserDTO>>
    {
    }
}