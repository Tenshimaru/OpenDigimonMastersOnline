using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateCloneConfigCommand : IRequest
    {
        public CloneConfigDTO Clone { get; }

        public UpdateCloneConfigCommand(CloneConfigDTO clone)
        {
            Clone = clone;
        }
    }
}