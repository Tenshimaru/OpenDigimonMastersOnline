using DigitalWorldOnline.Commons.DTOs.Routine;
using DigitalWorldOnline.Commons.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure.Repositories.Routine
{
    public class RoutineRepository : IRoutineRepository
    {
        private readonly DatabaseContext _context;

        public RoutineRepository(DatabaseContext context)
        {
            _context = context;
        }
        public class BitwiseOperations
        {
            public static int GetBitValue(int[] array, int x)
            {
                int arrIDX = x / 32;
                int bitPosition = x % 32;

                if (arrIDX >= array.Length)
                    throw new ArgumentOutOfRangeException("Invalid array index");

                int value = array[arrIDX];
                return (value >> bitPosition) & 1;
            }

            public static void SetBitValue(ref int[] array, int x, int bitValue)
            {
                int arrIDX = x / 32;
                int bitPosition = x % 32;

                if (arrIDX >= array.Length)
                    throw new ArgumentOutOfRangeException("Invalid array index");

                if (bitValue != 0 && bitValue != 1)
                    throw new ArgumentException("Invalid bit value. Only 0 or 1 are allowed.");

                int value = array[arrIDX];
                int mask = 1 << bitPosition;

                if (bitValue == 1)
                    array[arrIDX] = value | mask;
                else
                    array[arrIDX] = value & ~mask;
            }

        }

        public async Task ExecuteDailyQuestsAsync(List<short> questIdList)
        {
            var progressList = await _context.CharacterProgress
                .AsNoTracking()
                .ToListAsync();

            progressList.ForEach(progress =>
            {
                questIdList.ForEach(questId =>
                {
                    progress.CompletedDataValue = MarkQuestIncomplete(questId, progress.CompletedDataValue);
                    
                });

                _context.Update(progress);
            });

           
            _context.SaveChanges();
        }
        public int[] MarkQuestIncomplete(int qIDX, int[] CompleteDataInt)
        {
            int bitValue = BitwiseOperations.GetBitValue(CompleteDataInt, qIDX - 1);

            if (bitValue == 1)
            {
                BitwiseOperations.SetBitValue(ref CompleteDataInt, qIDX - 1, 0);
            }

            return CompleteDataInt;
        }

        public async Task<List<RoutineDTO>> GetActiveRoutinesAsync()
        {
            return await _context.Routine
                .AsNoTracking()
                .Where(x => x.Active)
                .ToListAsync();
        }

        public async Task UpdateRoutineExecutionTimeAsync(long routineId)
        {
            var dto = await _context.Routine
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == routineId);

            if(dto != null)
            {
                dto.NextRunTime = dto.NextRunTime.AddDays(dto.Interval);

                _context.Update(dto);
                _context.SaveChanges();
            }
        }
    }
}