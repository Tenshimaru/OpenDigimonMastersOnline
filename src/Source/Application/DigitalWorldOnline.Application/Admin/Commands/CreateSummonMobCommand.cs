using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateSummonMobCommand : IRequest<SummonMobDTO>
    {
        public SummonMobDTO Mob { get; }

        public CreateSummonMobCommand(SummonMobDTO mob)
        {
            Mob = mob;
        }
    }
}