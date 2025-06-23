namespace DigitalWorldOnline.Models.DTOs.Routine
{
    public sealed partial class RoutineModel
    {
        /// <summary>
        /// Flag for execution time.
        /// </summary>
        public bool ExecutionTime => DateTime.Now >= NextRunTime;
    }
}