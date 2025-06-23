using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateCloneConfigCommand : IRequest<CloneConfigDTO>
    {
        public CloneConfigDTO Clone { get; }

        public CreateCloneConfigCommand(CloneConfigDTO clone)
        {
            Clone = clone;
        }
    }
}