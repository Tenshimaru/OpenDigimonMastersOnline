using DigitalWorldOnline.Commons.Models.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateMobConfigCommand : IRequest
    {
        public MobConfigModel MobConfig { get; set; }

        public UpdateMobConfigCommand(MobConfigModel mobConfig)
        {
            MobConfig = mobConfig;
        }
    }
}
