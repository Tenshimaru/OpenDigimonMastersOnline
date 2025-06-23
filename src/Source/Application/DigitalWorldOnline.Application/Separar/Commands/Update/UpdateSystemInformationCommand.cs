using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateSystemInformationCommand : IRequest
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public string? Cpu { get; set; }

        public string? Gpu { get; set; }

        public string? Ip { get; set; }

        public UpdateSystemInformationCommand(
            long id,
            long accountId,
            string? cpu = null,
            string? gpu = null,
            string? ip = null)
        {
            Id = id;
            AccountId = accountId;
            Cpu = cpu;
            Gpu = gpu;
            Ip = ip;
        }
    }
}