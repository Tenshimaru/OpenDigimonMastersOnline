using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateSystemInformationCommand : IRequest<SystemInformationDTO>
    {
        public long AccountId { get; set; }

        public string? Cpu { get; set; }

        public string? Gpu { get; set; }

        public string? Ip { get; set; }

        public CreateSystemInformationCommand(
            long accountId,
            string? cpu = null,
            string? gpu = null,
            string? ip = null)
        {
            AccountId = accountId;
            Cpu = cpu;
            Gpu = gpu;
            Ip = ip;
        }
    }
}
