using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateSummonCommand :IRequest<SummonDTO>
    {
        public SummonDTO Summon { get; }

        public CreateSummonCommand(SummonDTO summon)
        {
            Summon = summon;
        }
    }
}
