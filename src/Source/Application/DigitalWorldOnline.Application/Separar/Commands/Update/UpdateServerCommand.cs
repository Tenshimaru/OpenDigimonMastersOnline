using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateServerCommand : IRequest
    {
        public long ServerId { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public bool Maintenance { get; set; }

        public UpdateServerCommand(long serverId,
            string name, int experience, bool maintenance)
        {
            ServerId = serverId;
            Name = name;
            Experience = experience;
            Maintenance = maintenance;
        }
    }
}
