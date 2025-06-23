using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdatePremiumAndSilkCommand : IRequest
    {
        public int Premium { get; set; }

        public int Silk { get; set; }

        public long AccountId { get; set; }

        public UpdatePremiumAndSilkCommand(int premium, int silk, long accountId)
        {
            AccountId = accountId;
            Premium = premium;
            Silk = silk;
        }
    }
}
