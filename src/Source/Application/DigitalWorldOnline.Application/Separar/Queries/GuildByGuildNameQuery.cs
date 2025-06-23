using DigitalWorldOnline.Commons.DTOs.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByGuildNameQuery : IRequest<GuildDTO?>
    {
        public string GuildName { get; private set; }

        public GuildByGuildNameQuery(string guildName)
        {
            GuildName = guildName;
        }
    }
}