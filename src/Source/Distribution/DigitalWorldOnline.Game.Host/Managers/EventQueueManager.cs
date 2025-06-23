using DigitalWorldOnline.Commons.Models.Character;
using Serilog;

namespace DigitalWorldOnline.Game.Managers
{
    public class EventQueueManager
    {
        public List<CharacterModel> Tamers { get; private set; }
        public DateTime TeleportTime { get; private set; } = DateTime.MaxValue;

        public bool EventStart => DateTime.Now >= TeleportTime;
        public bool TamersReady => !Tamers.Exists(x => x.Location.MapId != 1700 || x.State == Commons.Enums.Character.CharacterStateEnum.Ready);

        private bool LockedQueue => Tamers.Count >= 4 || EventStart;

        private readonly ILogger _logger;
        private int _queueDuration = 120;
        private List<long> _tamersInMap = new List<long>(4);

        public EventQueueManager(ILogger logger)
        {
            _logger = logger;
            Tamers = new List<CharacterModel>();
        }

        public bool InMap(long tamerId) => _tamersInMap.Contains(tamerId);

        public void SetInMap(long tamerId) => _tamersInMap.Add(tamerId);

        public bool JoinQueue(CharacterModel tamer)
        {
            if (LockedQueue)
                return false;

            _logger.Verbose($"Tamer {tamer.Id} - {tamer.Name} joinned the queue.");

            if (!Tamers.Any())
                TeleportTime = DateTime.Now.AddSeconds(_queueDuration);

            Tamers.Add(tamer);

            tamer.UpdateEventQueueInfoTime(5);

            return true;
        }

        public bool LeaveQueue(CharacterModel tamer)
        {
            if (LockedQueue)
                return false;

            _logger.Verbose($"Tamer {tamer.Id} - {tamer.Name} left the queue.");

            Tamers.RemoveAll(x => x.Id == tamer.Id);

            return true;
        }

        public bool InQueue(long tamerId)
        {
            return Tamers.Exists(x => x.Id == tamerId);
        }
    }
}
