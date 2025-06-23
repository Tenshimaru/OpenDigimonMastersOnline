using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateMobCommand : IRequest<MobConfigDTO>
    {
        public MobConfigDTO Mob { get; }

        public CreateMobCommand(MobConfigDTO mob)
        {
            Mob = mob;
        }
    }
}