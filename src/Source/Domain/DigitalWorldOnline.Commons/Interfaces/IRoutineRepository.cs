using DigitalWorldOnline.Commons.DTOs.Routine;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IRoutineRepository
    {
        Task ExecuteDailyQuestsAsync(List<short> questIdList);

        Task<List<RoutineDTO>> GetActiveRoutinesAsync();

        Task UpdateRoutineExecutionTimeAsync(long routineId);
    }
}
