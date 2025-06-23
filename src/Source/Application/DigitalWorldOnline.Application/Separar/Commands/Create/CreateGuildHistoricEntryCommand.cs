using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildHistoricEntryCommand : IRequest
    {
        public GuildHistoricModel? HistoricEntry { get; private set; }
        public long GuildId { get; private set; }

        public CreateGuildHistoricEntryCommand(GuildHistoricModel? historicEntry, long guildId)
        {
            HistoricEntry = historicEntry;
            GuildId = guildId;
        }
    }
}