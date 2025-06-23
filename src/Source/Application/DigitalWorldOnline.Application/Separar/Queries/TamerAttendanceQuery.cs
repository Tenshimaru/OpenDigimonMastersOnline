using MediatR;
using DigitalWorldOnline.Commons.DTOs.Events;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerAttendanceQuery : IRequest<AttendanceRewardDTO>
    {
        public long CharacterId { get; set; }

        public TamerAttendanceQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}