using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.Enums.Server;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateServerCommand : IRequest<ServerDTO>
    {
        public string Name { get; }
        public int Experience { get; }
        public ServerTypeEnum Type { get; }
        public int Port { get; }

        public CreateServerCommand(
            string name,
            int experience,
            ServerTypeEnum type,
            int port)
        {
            Name = name;
            Experience = experience;
            Type = type;
            Port = port;
        }
    }
}