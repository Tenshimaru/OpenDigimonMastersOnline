using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateTamerAttendanceRewardCommand: IRequest
    {
    
        public AttendanceRewardModel AttendanceRewardModel { get; set; }

        public UpdateTamerAttendanceRewardCommand(AttendanceRewardModel attendanceReward)
        {
            AttendanceRewardModel = attendanceReward;
        }
    }
}
