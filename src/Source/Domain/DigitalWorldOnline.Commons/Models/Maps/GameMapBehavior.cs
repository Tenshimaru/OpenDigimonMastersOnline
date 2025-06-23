using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Map.Dungeons;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.MapServer;
using System.Text;
using DigitalWorldOnline.Commons.Models.Config.Events;
using DigitalWorldOnline.Commons.Packets.GameServer;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public sealed partial class GameMap : ICloneable
    {
        //TODO: externalizar
        private readonly int _startToSee = 4000;
        private readonly int _stopSeeing = 4001;

        public List<ConsignedShop> ConsignedShopsToRemove = new();

        public void Initialize()
        {
            if (Initialized)
                return;

            Clients = new List<GameClient>();
            Drops = new List<Drop>();
            ConsignedShops = new List<ConsignedShop>();

            TamersView = new Dictionary<long, List<long>>();
            MobsView = new Dictionary<long, List<long>>();
            DropsView = new Dictionary<long, List<long>>();
            ConsignedShopView = new Dictionary<long, List<long>>();

            TamerHandlers = new Dictionary<short, long>();
            DigimonHandlers = new Dictionary<short, long>();
            MobHandlers = new Dictionary<short, long>();
            DropHandlers = new Dictionary<short, long>();
            ColiseumMobs = new List<int>();

            for (short i = 1; i <= byte.MaxValue; i++)
                TamerHandlers.Add(i, 0);

            for (short i = 1; i <= 2000; i++)
                DigimonHandlers.Add(i, 0);

            for (short i = 1; i <= 1000; i++)
                MobHandlers.Add(i, 0);

            for (short i = 1; i <= 2000; i++)
                DropHandlers.Add(i, 0);

            KillSpawns?.ForEach(killSpawn => { killSpawn.ResetCurrentSourceMobAmount(); });

            Mobs?.ForEach(mob =>
            {
                mob.UpdateCurrentHp(mob.HPValue);
                mob.SetInitialLocation();

                var mobKillSpawn = KillSpawns.FirstOrDefault(x => x.TargetMobs.Any(killSpawnTargetMobConfigModel =>
                    killSpawnTargetMobConfigModel.TargetMobType == mob.Type));

                if (mobKillSpawn != null)
                {
                    mob.SetAwaitingKillSpawn();
                }
            });

            EventMobs?.ForEach(mob =>
            {
                mob.UpdateCurrentHp(mob.HPValue);
                mob.SetInitialLocation();

                var mobKillSpawn = KillSpawns.FirstOrDefault(x => x.TargetMobs.Any(killSpawnTargetMobConfigModel =>
                    killSpawnTargetMobConfigModel.TargetMobType == mob.Type));

                if (mobKillSpawn != null)
                {
                    mob.SetAwaitingKillSpawn();
                }
            });

            Initialized = true;
        }

        public bool RequestMobsUpdate(IList<MobConfigModel> mapMobs)
        {
            UpdateMobs = /*NeedToAddMobs(mapMobs) ||*/ NeedToRemoveMobs(mapMobs);

            return UpdateMobs;
        }

        public bool RequestMobsUpdate(IList<EventMobConfigModel> mapMobs)
        {
            UpdateMobs = NeedToRemoveEventMobs(mapMobs);

            return UpdateMobs;
        }

        private bool NeedToAddMobs(IList<MobConfigModel> mapMobs)
        {
            MobsToAdd = new List<MobConfigModel>();

            MobsToAdd.AddRange(mapMobs.Where(x => !Mobs.Select(y => y.Id).Contains(x.Id)));

            return MobsToAdd.Count > 0;
        }

        private bool NeedToRemoveMobs(IList<MobConfigModel> mapMobs)
        {
            MobsToRemove = new List<MobConfigModel>();

            if (Mobs?.Any() ?? false)
            {
                MobsToRemove.AddRange(Mobs.Where(x => !mapMobs.Select(y => y.Id).Contains(x.Id)));
            }

            return MobsToRemove.Count > 0;
        }

        private bool NeedToAddEventMobs(IList<EventMobConfigModel> mapMobs)
        {
            EventMobsToAdd = new List<EventMobConfigModel>();

            if (EventMobs?.Any() ?? false)
            {
                EventMobsToAdd.AddRange(mapMobs.Where(x => !EventMobs.Select(y => y.Id).Contains(x.Id)));
            }

            return EventMobsToAdd.Count > 0;
        }

        private bool NeedToRemoveEventMobs(IList<EventMobConfigModel> mapMobs)
        {
            EventMobsToRemove = new List<EventMobConfigModel>();

            if (EventMobs?.Any() ?? false)
            {
                EventMobsToRemove.AddRange(EventMobs.Where(x => !mapMobs.Select(y => y.Id).Contains(x.Id)));
            }

            return EventMobsToRemove.Count > 0;
        }

        public void FinishMobsUpdate()
        {
            _mobsToAdd.Clear();
            UpdateMobs = false;
        }

        public void setRoyalBaseMap(RoyalBaseMap? map)
        {
            RoyalBaseMap = map;
        }

        public void IsRoyalBaseUpdate(bool value = false)
        {
            IsRoyalBase = value;
        }

        public bool MobsAttacking(long tamerId) =>
            Mobs.Any(x => !x.Dead && x.TargetTamers.Exists(characterModel => characterModel.Id == tamerId));

        public bool MobsAttacking(long tamerId, bool Summon) =>
            SummonMobs.Any(x => !x.Dead && x.TargetTamers.Exists(characterModel => characterModel.Id == tamerId));

        public bool PlayersAttacking(long partnerId) =>
            ConnectedTamers.Any(x => x.Alive && x.TargetPartners.Exists(digimonModel => digimonModel.Id == partnerId));

        public void BroadcastForMap(byte[] packet)
        {
            Clients.ForEach(client => { client.Send(packet); });
        }

        public void BroadcastForUniqueTamer(long tamerId, byte[] packet)
        {
            BroadcastForSelf(tamerId, packet);
        }

        public void BroadcastForTargetTamers(List<long> targetTamers, byte[] packet)
        {
            var clients = Clients.Where(x => targetTamers.Contains(x.TamerId)).ToList();

            clients.ForEach(client => { client.Send(packet); });
        }

        public void BroadcastForTargetTamers(long sourceId, byte[] packet)
        {
            BroadcastForTamerViews(sourceId, packet);
        }

        public void BroadcastForTamerViewsAndSelf(long sourceId, byte[] packet)
        {
            BroadcastForTamerViews(sourceId, packet);
            BroadcastForSelf(sourceId, packet);
        }

        public void BroadcastForTamerViewOnly(long sourceId, byte[] packet)
        {
            BroadcastForTamerViews(sourceId, packet);
        }

        private void BroadcastForTamerViews(long sourceId, byte[] packet)
        {
            if (TamersView.ContainsKey(sourceId))
            {
                var views = new List<long>();
                views.AddRange(TamersView[sourceId]);
                views.ForEach(view => { Clients.FirstOrDefault(x => x.TamerId == view)?.Send(packet); });
            }
        }

        private void BroadcastForSelf(long sourceId, byte[] packet)
        {
            Clients.FirstOrDefault(x => x.TamerId == sourceId)?.Send(packet);
        }

        public void StartOperation() => Operating = true;
        public void EndOperation() => Operating = false;

        public void ManageHandlers()
        {
            if (!Tamers.Any())
                return;

            foreach (var tamer in LoadingTamers)
            {
                if (NeedNewHandler(tamer.Id))
                {
                    SetDigimonHandlers(tamer.Digimons);
                    SetTamerHandler(tamer);
                    ResetView(tamer.Id);
                }

                if (tamer.State != CharacterStateEnum.Ready)
                    tamer.UpdateState(CharacterStateEnum.Connected);
            }

            Mobs?.ForEach(mob =>
            {
                if (NeedNewHandler(mob))
                    SetMobHandler(mob);
            });

            Drops?.ForEach(drop =>
            {
                if (NeedNewHandler(drop))
                    SetDropHandler(drop);
            });
        }

        public void AddClient(GameClient client)
        {
            AddTamerView(client.TamerId);

            lock (ClientsLock)
            {
                if (Clients.Any(x => x.TamerId == client.TamerId))
                    return;

                Clients.Add(client);
            }

            WithoutTamers = DateTime.MaxValue;

            while (NeedNewHandler(client.Tamer.Id))
                Thread.Sleep(500);

            client.SetLoading(false);
        }

        private void ClearViews(long tamerId)
        {
            if (ConsignedShopView != null && ConsignedShopView.Any())
            {
                var consignedShopsView = ConsignedShopView.FirstOrDefault(x => x.Value.Contains(tamerId));
                consignedShopsView.Value?.Remove(tamerId);
            }

            if (TamersView != null && TamersView.Any())
            {
                if (TamersView.ContainsKey(tamerId))
                    TamersView.Remove(tamerId);

                foreach (var tamerView in TamersView.Values.Where(x => x != null && x.Contains(tamerId)))
                {
                    tamerView.Remove(tamerId);
                }
            }

            Mobs.ForEach(mob => { mob.TamersViewing.Remove(tamerId); });

            if (DropsView != null && DropsView.Any())
            {
                foreach (var dropView in DropsView.Values.Where(x => x.Contains(tamerId)))
                {
                    dropView.Remove(tamerId);
                }
            }
        }

        public void RemoveClient(GameClient client)
        {
            BroadcastForTamerViews(client.TamerId, new UnloadTamerPacket(client.Tamer).Serialize());

#if DEBUG
            var serialized = SerializeHideTamer(client.Tamer);
            //File.WriteAllText($"Hides\\Hide{client.TamerId}To{client.TamerId}_Views_{DateTime.Now:dd_MM_yy_HH_mm_ss}.temp", serialized);
#endif
            ClearViews(client.TamerId);
            FreeTamerHandler(client.Tamer.Id);
            FreeDigimonsHandler(client.Tamer.Digimons);

            lock (ClientsLock)
            {
                Clients.RemoveAll(x => x.AccountId == client.AccountId);
            }

            if (!Tamers.Any())
                WithoutTamers = DateTime.Now;
        }

        private static string SerializeHideTamer(CharacterModel tamer)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Tamer{tamer.Id}{tamer.Name}");
            sb.AppendLine($"TamerHandler {tamer.GeneralHandler.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.X.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.Y.ToString()}");

            sb.AppendLine($"Partner{tamer.Partner.Id}{tamer.Partner.Name}");
            sb.AppendLine($"PartnerHandler {tamer.Partner.GeneralHandler.ToString()}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.X.ToString()}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.Y.ToString()}");

            return sb.ToString();
        }

        public object Clone()
        {
            return (GameMap)MemberwiseClone();
        }
    }
}