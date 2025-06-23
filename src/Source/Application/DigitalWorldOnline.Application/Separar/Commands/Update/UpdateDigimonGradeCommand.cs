using DigitalWorldOnline.Commons.Enums;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonGradeCommand : IRequest
    {
        public long DigimonId { get; }
        public DigimonHatchGradeEnum Grade { get; }

        public UpdateDigimonGradeCommand(long digimonId, DigimonHatchGradeEnum grade)
        {
            DigimonId = digimonId;
            Grade = grade;
        }
    }
}
