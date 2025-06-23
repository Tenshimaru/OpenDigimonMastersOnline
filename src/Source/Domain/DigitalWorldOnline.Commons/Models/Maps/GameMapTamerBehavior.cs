using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public sealed partial class GameMap
    {
        public bool CloseMap => (DateTime.Now - WithoutTamers).TotalHours >= 2; //TODO: externalizar

        public List<CharacterModel> Tamers =>
            Clients
                .Where(x => x.IsConnected)
                .Select(x => x.Tamer)
                .ToList();

        public List<CharacterModel> LoadingTamers =>
            Clients
                .Where(x => x.Tamer.State == CharacterStateEnum.Loading)
                .Select(x => x.Tamer)
                .ToList();

        public List<CharacterModel> NearbyTamers(long tamerKey)
        {
            return ConnectedTamers
                .Where(x => TamersView[tamerKey].Contains(x.Id))
                .ToList();
        }

        public List<DigimonModel> ConnectedPartners => ConnectedTamers.Select(x => x.Partner).ToList();

        public List<CharacterModel> ConnectedTamers =>
            Clients
                .Where(x => x.IsConnected && x.Tamer.State == CharacterStateEnum.Ready)
                .Select(x => x.Tamer)
                .ToList();

        public void SetNoTamers()
        {
            if (WithoutTamers == DateTime.MinValue)
                WithoutTamers = DateTime.Now;
        }

        private bool NeedNewHandler(long tamerId)
        {
            return !TamerHandlers.ContainsValue(tamerId);
        }

        public void SetDigimonHandlers(List<DigimonModel> digimons)
        {
            foreach (var digimon in digimons)
            {
                SetDigimonHandler(digimon);
            }
        }

        public void SwapDigimonHandlers(DigimonModel oldPartner, DigimonModel newPartner)
        {
            lock (DigimonHandlersLock)
            {
                var oldPartnerHandler = DigimonHandlers.FirstOrDefault(x => x.Value == oldPartner.Id).Key;
                newPartner.SetHandlerValue(oldPartnerHandler);

                var newPartnerHandler = DigimonHandlers.FirstOrDefault(x => x.Value == newPartner.Id).Key;
                oldPartner.SetHandlerValue(newPartnerHandler);

                DigimonHandlers[newPartnerHandler] = oldPartner.Id;
                DigimonHandlers[oldPartnerHandler] = newPartner.Id;
            }
        }

        private void SetDigimonHandler(DigimonModel digimon)
        {
            lock (DigimonHandlersLock)
            {
                FreeDigimonHandler(digimon.Id);

                var handler = DigimonHandlers.First(x => x.Value == 0).Key;
                DigimonHandlers[handler] = digimon.Id;

                digimon.SetHandlerValue(handler);
            }
        }

        public void FreeDigimonHandler(long digimonId)
        {
            if (DigimonHandlers.ContainsValue(digimonId))
            {
                var handler = DigimonHandlers.FirstOrDefault(x => x.Value == digimonId).Key;
                DigimonHandlers[handler] = 0;
            }
        }

        public void FreeDigimonsHandler(IList<DigimonModel> digimons)
        {
            lock (DigimonHandlersLock)
            {
                foreach (var digimon in digimons)
                    FreeDigimonHandler(digimon.Id);
            }
        }

        public void SetTamerHandler(CharacterModel character)
        {
            lock (TamerHandlersLock)
            {
                FreeTamerHandler(character.Id);

                var handler = TamerHandlers.First(x => x.Value == 0).Key;
                TamerHandlers[handler] = character.Id;

                character.SetHandlerValue(handler);
            }
        }

        public void FreeTamerHandler(long tamerId)
        {
            if (TamerHandlers.ContainsValue(tamerId))
            {
                var handler = TamerHandlers.FirstOrDefault(x => x.Value == tamerId).Key;
                TamerHandlers[handler] = 0;
            }
        }

        private void AddTamerView(long tamerId)
        {
            try
            {
                if (!TamersView.ContainsKey(tamerId))
                {
                    TamersView.Add(tamerId, new List<long>());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message}");
                throw;
            }
        }

        public bool ViewingTamer(long tamerKey, long tamerTarget)
        {
            if (TamersView.ContainsKey(tamerKey))
                return TamersView[tamerKey].Contains(tamerTarget);
            else
                return false;
        }

        public void ShowTamer(long tamerKey, long tamerTarget)
        {
            TamersView
                .FirstOrDefault(x => x.Key == tamerKey).Value?
                .Add(tamerTarget);
        }

        public void HideTamer(long tamerKey, long tamerTarget)
        {
            if (TamersView.ContainsKey(tamerKey))
                TamersView[tamerKey].Remove(tamerTarget);
        }
    }
}