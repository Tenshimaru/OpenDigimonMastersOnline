using DigitalWorldOnline.Commons.DTOs.Routine;
using MediatR;

namespace DigitalWorldOnline.Application.Routines.Queries
{
    public class GetActiveRoutinesQuery : IRequest<List<RoutineDTO>>
    {
    }
}