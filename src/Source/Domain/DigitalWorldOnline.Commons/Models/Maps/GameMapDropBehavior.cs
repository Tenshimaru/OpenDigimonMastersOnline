using DigitalWorldOnline.Commons.Utils;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public sealed partial class GameMap
    {
        public List<Drop> DropsToRemove = new();

        public List<Drop> DropsToAdd = new();

        public List<long> NearestTamers(Drop drop)
        {
            var targetTamers = new List<long>();

            foreach (var tamer in ConnectedTamers)
            {
                var diff = UtilitiesFunctions.CalculateDistance(
                    drop.Location.X,
                    tamer.Partner.Location.X,
                    drop.Location.Y,
                    tamer.Partner.Location.Y);

                if (diff <= _startToSee)
                    targetTamers.Add(tamer.Id);
            }

            return targetTamers;
        }

        public List<long> FarawayTamers(Drop drop)
        {
            var targetTamers = new List<long>();

            foreach (var tamer in ConnectedTamers)
            {
                var diff = UtilitiesFunctions.CalculateDistance(
                    drop.Location.X,
                    tamer.Partner.Location.X,
                    drop.Location.Y,
                    tamer.Partner.Location.Y);

                if (diff >= _stopSeeing)
                    targetTamers.Add(tamer.Id);
            }

            return targetTamers;
        }

        #region Handler
        private bool NeedNewHandler(Drop drop)
        {
            return DropHandlers.Values.FirstOrDefault(x => x == drop.Id) == 0;
        }

        public void SetDropHandler(Drop drop)
        {
            short handler = 0;

            lock (DropHandlers)
            {
                FreeDropHandler(drop.Id);

                handler = DropHandlers.First(x => x.Value == 0).Key;

                DropHandlers[handler] = drop.Id;
            }

            drop.SetHandlerValue(handler);
        }

        public void FreeDropHandler(long dropId)
        {
            lock (DropHandlers)
            {
                var handler = DropHandlers.FirstOrDefault(x => x.Value == dropId).Key;

                if (handler > 0)
                    DropHandlers[handler] = 0;
            }
        }
        #endregion

        #region View
        private void ManageDropView(long dropId)
        {
            RemoveDropView(dropId);
            AddDropView(dropId);
        }

        public IList<long> GetDropViews(long dropKey)
        {
            return DropsView
                .FirstOrDefault(x => x.Key == dropKey).Value ??
                new List<long>();
        }

        private void AddDropView(long dropId)
        {
            if (!DropsView.ContainsKey(dropId))
                DropsView.Add(dropId, new List<long>());
        }

        public bool ViewingDrop(long dropKey, long tamerTarget)
        {
            return DropsView
                .FirstOrDefault(x => x.Key == dropKey).Value?
                .Contains(tamerTarget) ??
                false;
        }

        public void ShowDrop(long dropKey, long tamerTarget)
        {
            DropsView
                .FirstOrDefault(x => x.Key == dropKey).Value?
                .Add(tamerTarget);
        }

        public void HideDrop(long dropKey, long tamerTarget)
        {
            DropsView
                .FirstOrDefault(x => x.Key == dropKey).Value?
                .Remove(tamerTarget);
        }

        public void RemoveDropView(long dropId)
        {
            if (DropsView.ContainsKey(dropId))
                DropsView.Remove(dropId);
        }
        #endregion

        public void AddDrop(Drop drop)
        {
            if (!Drops.Any(x => x.Id == drop.Id))
            {
                ManageDropView(drop.Id);
                SetDropHandler(drop);
                Drops.Add(drop);
            }
        }

        public void AddMapDrop(Drop drop)
        {
            lock (DropsLock)
            {
                DropsToAdd.Add(drop);
            }
        }

        public Drop? GetDrop(int dropHandler)
        {
            return Drops.FirstOrDefault(x => x.GeneralHandler == dropHandler);
        }

        public void RemoveMapDrop(Drop drop)
        {
            lock (DropsLock)
            {
                DropsToRemove.Add(drop);
            }
        }

        public void RemoveDrop(Drop drop)
        {
            if (Drops.Any(x => x.Id == drop.Id))
            {
                RemoveDropView(drop.Id);
                FreeDropHandler(drop.Id);
                Drops.Remove(drop);
            }
        }
    }
}
