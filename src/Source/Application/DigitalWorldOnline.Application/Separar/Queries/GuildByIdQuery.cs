using DigitalWorldOnline.Commons.DTOs.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByIdQuery : IRequest<GuildDTO?>
    {
        public long GuildId { get; private set; }

        public GuildByIdQuery(long guildId)
        {
            GuildId = guildId;
        }
    }
}