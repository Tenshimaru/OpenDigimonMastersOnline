using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.Map.Dungeons;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Models.TamerShop;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class DungeonsServer
    {
        private DateTime _lastMapsSearch = DateTime.Now;
        private DateTime _lastMobsSearch = DateTime.Now;
        private DateTime _lastConsignedShopsSearch = DateTime.Now;

        //TODO: externalizar
        private readonly int _startToSee = 18000;
        private readonly int _stopSeeing = 18001;

        /// <summary>
        /// Cleans unused running maps.
        /// </summary>
        public Task CleanMaps()
        {
            var mapsToRemove = new List<GameMap>();
            mapsToRemove.AddRange(Maps.Where(x => x.CloseMap));

            foreach (var map in mapsToRemove)
            {
                // _logger.Information($"Removing inactive instance for {map.Type} map {map.Id} - {map.Name}...");
                Maps.Remove(map);
            }

            return Task.CompletedTask;
        }

        public Task CleanMap(int DungeonId)
        {
            var mapToClose = Maps.FirstOrDefault(x => x.DungeonId == DungeonId);

            if (mapToClose != null)
            {
                // _logger.Information($"Removing inactive instance for {mapToClose.Type} mapID: {mapToClose.MapId} - {mapToClose.Name}");
                Maps.Remove(mapToClose);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Search for new maps to instance.
        /// </summary>
        public async Task SearchNewMaps(CancellationToken cancellationToken)
        {
            if (DateTime.Now > _lastMapsSearch)
            {
                var mapsToLoad =
                    _mapper.Map<List<GameMap>>(await _sender.Send(new GameMapsConfigQuery(MapTypeEnum.Dungeon),
                        cancellationToken));

                var party = _partyManager.Parties;

                foreach (var newMap in mapsToLoad)
                {
                    foreach (var partymap in party)
                    {
                        if (Maps.All(x => x.Id == partymap.Id) && newMap.MapId ==
                            partymap.Members.ElementAt((byte)partymap.LeaderId).Value.Location.MapId)
                        {
                            _logger.Debug(
                                $"Initializing new instance for {newMap.Type} map {newMap.Id} - {newMap.Name}...");

                            int[] RoyalBaseMaps = { 1701, 1702, 1703 };
                            if (Array.Exists(RoyalBaseMaps, element => element == newMap.MapId))
                            {
                                var royalBaseMap = new RoyalBaseMap((short)newMap.MapId, newMap.Mobs);
                                newMap.IsRoyalBaseUpdate(true);
                                newMap.setRoyalBaseMap(royalBaseMap);
                            }
                            else
                            {
                                newMap.IsRoyalBaseUpdate();
                                newMap.setRoyalBaseMap(null);
                            }

                            Maps.Add(newMap);
                        }
                    }
                }

                _lastMapsSearch = DateTime.Now.AddSeconds(10);
            }
        }

        public async Task SearchNewMaps(bool IsParty, GameClient client)
        {
            var mapsToLoad =
                _mapper.Map<List<GameMap>>(await _sender.Send(new GameMapsConfigQuery(MapTypeEnum.Dungeon)));

            if (IsParty)
            {
                var party = _partyManager.FindParty(client.TamerId);

                if (party != null)
                {
                    foreach (var newMap in mapsToLoad)
                    {
                        if (!Maps.Exists(x => x.DungeonId == party.Id) && newMap.MapId == client.Tamer.Location.MapId)
                        {
                            var newDungeon = (GameMap)newMap.Clone();

                            var mobsToRemove = newDungeon.Mobs.Where(x => x.Coliseum && x.Round > 0).ToList();

                            if (mobsToRemove.Any())
                            {
                                foreach (var mob in mobsToRemove)
                                {
                                    newDungeon.Mobs.Remove(mob);
                                }
                            }

                            if (newMap.MapId == 2001 || newMap.MapId == 2002)
                            {
                                mobsToRemove = newDungeon.Mobs
                                    .Where(x => x.WeekDay != (DungeonDayOfWeekEnum)DateTime.Now.DayOfWeek).ToList();

                                if (mobsToRemove.Any())
                                {
                                    foreach (var mob in mobsToRemove)
                                    {
                                        newDungeon.Mobs.Remove(mob);
                                    }
                                }
                            }

                            newDungeon.SetId(party.Id);
                            _logger.Debug(
                                $"Initializing new instance for {newMap.Type} party {party.Id} - {newMap.Name}...");

                            int[] RoyalBaseMaps = { 1701, 1702, 1703 };
                            if (Array.Exists(RoyalBaseMaps, element => element == newDungeon.MapId))
                            {
                                var royalBaseMap = new RoyalBaseMap((short)newDungeon.MapId, newDungeon.Mobs);
                                newDungeon.IsRoyalBaseUpdate(true);
                                newDungeon.setRoyalBaseMap(royalBaseMap);
                            }
                            else
                            {
                                newDungeon.IsRoyalBaseUpdate();
                                newDungeon.setRoyalBaseMap(null);
                            }

                            Maps.Add(newDungeon);
                        }
                    }
                }
            }
            else
            {
                foreach (var newMap in mapsToLoad)
                {
                    if (!Maps.Exists(x => x.DungeonId == client.TamerId) && newMap.MapId == client.Tamer.Location.MapId)
                    {
                        var newDungeon = (GameMap)newMap.Clone();

                        var mobsToRemove = newDungeon.Mobs.Where(x => x.Coliseum && x.Round > 0).ToList();

                        if (mobsToRemove.Any())
                        {
                            foreach (var mob in mobsToRemove)
                            {
                                newDungeon.Mobs.Remove(mob);
                            }
                        }

                        if (newMap.MapId == 2001 || newMap.MapId == 2002)
                        {
                            mobsToRemove = newDungeon.Mobs
                                .Where(x => x.WeekDay != (DungeonDayOfWeekEnum)DateTime.Now.DayOfWeek).ToList();

                            if (mobsToRemove.Any())
                            {
                                foreach (var mob in mobsToRemove)
                                {
                                    newDungeon.Mobs.Remove(mob);
                                }
                            }
                        }

                        newDungeon.SetId((int)client.TamerId);


                        int[] RoyalBaseMaps = { 1701, 1702, 1703 };
                        if (Array.Exists(RoyalBaseMaps, element => element == newDungeon.MapId))
                        {
                            var royalBaseMap = new RoyalBaseMap((short)newDungeon.MapId, newDungeon.Mobs);
                            newDungeon.IsRoyalBaseUpdate(true);
                            newDungeon.setRoyalBaseMap(royalBaseMap);
                        }
                        else
                        {
                            newDungeon.IsRoyalBaseUpdate();
                            newDungeon.setRoyalBaseMap(null);
                        }

                        _logger.Debug(
                            $"Initializing new instance for {newMap.Type} tamer {client.TamerId} - {newMap.Name}...");
                        Maps.Add(newDungeon);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maps objects.
        /// </summary>
        public async Task GetMapObjects(CancellationToken cancellationToken)
        {
            await GetMapMobs(cancellationToken);
        }

        public async Task GetMapObjects()
        {
            await GetMapMobs();
        }

        /// <summary>
        /// Gets the map latest mobs.
        /// </summary>
        /// <returns>The mobs collection</returns>
        private async Task GetMapMobs(CancellationToken cancellationToken)
        {
            // Take a snapshot of initialized maps
            var initializedMaps = Maps.Where(x => x.Initialized).ToList();

            foreach (var map in initializedMaps)
            {
                var mapMobs =
                    _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(map.Id),
                        cancellationToken));

                if (mapMobs != null)
                {
                    var mobsToRemove = mapMobs.Where(x => x.Coliseum && x.Round > 0).ToList();

                    if (mobsToRemove.Any())
                    {
                        foreach (var mob in mobsToRemove)
                        {
                            mapMobs.Remove(mob);
                        }
                    }
                }

                if (map.RequestMobsUpdate(mapMobs))
                    map.UpdateMobsList();
            }
        }

        private async Task GetMapMobs()
        {
            // Take a snapshot of initialized maps
            var initializedMaps = Maps.Where(x => x.Initialized).ToList();

            foreach (var map in initializedMaps)
            {
                var mapMobs = _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(map.Id)));

                if (mapMobs != null)
                {
                    var mobsToRemove = mapMobs.Where(x => x.Coliseum && x.Round > 0).ToList();

                    if (mobsToRemove.Any())
                    {
                        foreach (var mob in mobsToRemove)
                        {
                            mapMobs.Remove(mob);
                        }
                    }
                }

                if (map.RequestMobsUpdate(mapMobs))
                    map.UpdateMobsList();
            }
        }

        /// <summary>
        /// Gets the consigned shops latest list.
        /// </summary>
        /// <returns>The consigned shops collection</returns>
        private async Task GetMapConsignedShops(CancellationToken cancellationToken)
        {
            if (DateTime.Now > _lastConsignedShopsSearch)
            {
                // Take a snapshot of initialized maps
                var initializedMaps = Maps.Where(x => x.Initialized).ToList();
                foreach (var map in initializedMaps)
                {
                    if (map.Operating)
                        continue;

                    var consignedShops =
                        _mapper.Map<List<ConsignedShop>>(await _sender.Send(new ConsignedShopsQuery((int)map.Id),
                            cancellationToken));

                    map.UpdateConsignedShops(consignedShops);
                }

                _lastConsignedShopsSearch = DateTime.Now.AddSeconds(15);
            }
        }

        /// <summary>
        /// The default hosted service "starting" method.
        /// </summary>
        /// <param name="cancellationToken">Control token for the operation</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await CleanMaps();
                    await GetMapObjects(cancellationToken);

                    var tasks = new List<Task>();

                    Maps.ForEach(map => { tasks.Add(RunMap(map)); });

                    await Task.WhenAll(tasks);

                    await Task.Delay(500, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Unexpected map exception: {ex.Message} {ex.StackTrace}");
                    await Task.Delay(3000, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Runs the target map operations.
        /// </summary>
        /// <param name="map">the target map</param>
        private async Task RunMap(GameMap map)
        {
            try
            {
                map.Initialize();
                map.ManageHandlers();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var tasks = new List<Task>
                {
                    Task.Run(() => TamerOperation(map)),
                    Task.Run(() => MonsterOperation(map)),
                    Task.Run(() => DropsOperation(map))
                };

                await Task.WhenAll(tasks);

                stopwatch.Stop();
                var totalTime = stopwatch.Elapsed.TotalMilliseconds;

                if (totalTime >= 1000)
                    // _logger.Information($"RunMap ({map.MapId}): {totalTime}.");
                    await Task.Delay(500);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error at map running: {ex.Message} {ex.StackTrace}.");
            }
        }

        /// <summary>
        /// Adds a new gameclient to the target map.
        /// </summary>
        /// <param name="client">The game client to be added.</param>
        public async Task AddClient(GameClient client)
        {
            if (client.Tamer.TargetTamerIdTP > 0)
            {
                var map = Maps.FirstOrDefault(x => x.Clients.Exists(y => y.TamerId == client.Tamer.TargetTamerIdTP));
                client.SetLoading();

                client.Tamer.MobsInView.Clear();
                map?.AddClient(client);
                client.Tamer.Revive();
            }
            else
            {
                var party = _partyManager.FindParty(client.TamerId);

                if (party != null)
                {
                    var partyMap = Maps.FirstOrDefault(x => x.Initialized &&
                                                            x.DungeonId == party.LeaderId &&
                                                            x.MapId == client.Tamer.Location.MapId ||
                                                            x.DungeonId == party.Id &&
                                                            x.MapId == client.Tamer.Location.MapId);

                    if (partyMap != null)
                    {
                        client.SetLoading();

                        client.Tamer.MobsInView.Clear();
                        partyMap.AddClient(client);
                        client.Tamer.Revive();
                    }
                    else
                    {
                        Maps.RemoveAll(x => x.DungeonId == party.LeaderId || x.DungeonId == party.Id);

                        await SearchNewMaps(true, client);

                        while (partyMap == null)
                        {
                            partyMap = Maps.FirstOrDefault(x => x.Initialized &&
                                                                (x.DungeonId == party.LeaderId ||
                                                                 x.DungeonId == party.Id) &&
                                                                x.MapId == client.Tamer.Location.MapId);

                            if (partyMap != null)
                            {
                                client.Tamer.MobsInView.Clear();
                                partyMap.AddClient(client);
                                client.Tamer.Revive();
                                return;
                            }

                            _logger.Warning($"Waiting Dungeon map {client.Tamer.Location.MapId} initialization.");
                            await Task.Delay(1000);
                        }
                    }
                }
                else
                {
                    await SearchNewMaps(false, client);

                    var map = Maps.FirstOrDefault(x => x.Initialized && x.DungeonId == client.Tamer.Id);

                    if (map != null)
                    {
                        client.Tamer.MobsInView.Clear();
                        map.AddClient(client);
                        client.Tamer.Revive();
                    }
                    else
                    {
                        await Task.Run(async () =>
                        {
                            while (map == null)
                            {
                                await Task.Delay(1000);

                                map = Maps.FirstOrDefault(x => x.Initialized && x.DungeonId == client.Tamer.Id);

                                _logger.Warning($"Waiting Dungeon map {client.Tamer.Location.MapId} initialization.");
                            }

                            client.Tamer.MobsInView.Clear();
                            map.AddClient(client);
                            client.Tamer.Revive();
                        });
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Removes the gameclient from the target map.
        /// </summary>
        /// <param name="client">The gameclient to be removed.</param>
        public void RemoveClient(GameClient client)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == client.TamerId));

            map?.RemoveClient(client);

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                if (map?.Clients.Count == 0)
                {
                    CleanMap(party.Id);
                    CleanMap((int)party.LeaderId);
                }
            }
            else
            {
                if (map?.Clients.Count == 0)
                {
                    CleanMap((int)client.TamerId);
                }
            }
        }

        public void BroadcastForChannel(byte channel, byte[] packet)
        {
            var maps = Maps.Where(x => x.Channel == channel).ToList();

            maps?.ForEach(map => { map.BroadcastForMap(packet); });
        }

        public void BroadcastGlobal(byte[] packet)
        {
            var maps = Maps.Where(x => x.Clients.Any()).ToList();

            maps?.ForEach(map => { map.BroadcastForMap(packet); });
        }

        public void BroadcastForMap(short mapId, byte[] packet, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.MapId == mapId && x.Clients.Exists(y => y.TamerId == tamerId));

            map?.BroadcastForMap(packet);
        }

        public void BroadcastForUniqueTamer(long tamerId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.BroadcastForUniqueTamer(tamerId, packet);
        }

        public void BroadcastForMapAllChannels(short mapId, byte[] packet)
        {
            var maps = Maps.Where(x => x.Clients.Exists(gameClient => gameClient.Tamer.Location.MapId == mapId)).SelectMany(map => map.Clients);
            maps.ToList().ForEach(client => { client.Send(packet); });
        }

        public GameClient? FindClientByTamerId(long tamerId)
        {
            return Maps.SelectMany(map => map.Clients).FirstOrDefault(client => client.TamerId == tamerId);
        }

        public GameClient? FindClientByTamerName(string tamerName)
        {
            return Maps.SelectMany(map => map.Clients).FirstOrDefault(client => client.Tamer.Name == tamerName);
        }

        public GameClient? FindClientByTamerHandle(int handle)
        {
            return Maps.SelectMany(map => map.Clients).FirstOrDefault(client => client.Tamer?.GeneralHandler == handle);
        }

        public GameClient? FindClientByTamerHandleAndChannel(int handle, long TamerId)
        {
            return Maps.Where(x => x.Clients.Exists(gameClient => gameClient.TamerId == TamerId))
                .SelectMany(map => map.Clients)
                .FirstOrDefault(client => client.Tamer?.GeneralHandler == handle);
        }

        public void BroadcastForTargetTamers(List<long> targetTamers,byte[] packet)
        {
            Maps
                .Where(x => x.Clients.Any(gameClient => targetTamers.Contains(gameClient.TamerId)))
                .ToList()
                .ForEach(map => map.BroadcastForTargetTamers(targetTamers,packet));
        }

        public void BroadcastForTargetTamers(long sourceId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == sourceId));

            map?.BroadcastForTargetTamers(map.TamersView[sourceId], packet);
        }

        public void BroadcastForTamerViewsAndSelf(long sourceId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == sourceId));

            map?.BroadcastForTamerViewsAndSelf(sourceId, packet);
        }

        public void BroadcastForTamerViewsAndSelf(GameClient client, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient =>
                gameClient.TamerId == client.TamerId && gameClient.Tamer.Channel == client.Tamer.Channel));

            map?.BroadcastForTamerViewsAndSelf(client.TamerId, packet);
        }

        public void BroadcastForTamerViews(long sourceId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == sourceId));

            map?.BroadcastForTamerViewOnly(sourceId, packet);
        }

        public void AddMapDrop(Drop drop, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.DropsToAdd.Add(drop);
        }

        public void RemoveDrop(Drop drop, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.RemoveMapDrop(drop);
        }

        public Drop? GetDrop(short mapId, int dropHandler, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.GetDrop(dropHandler);
        }

        //Mobs
        public bool MobsAttacking(short mapId, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.MobsAttacking(tamerId) ?? false;
        }

        public bool MobsAttacking(short mapId, long tamerId, bool Summon)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.MobsAttacking(tamerId) ?? false;
        }

        public List<CharacterModel> GetNearbyTamers(short mapId, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.NearbyTamers(tamerId);
        }

        public void AddSummonMobs(short mapId, SummonMobModel summon, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(x => x.TamerId == tamerId));

            map?.AddMob(summon);
        }

        public void AddMobs(short mapId, MobConfigModel mob, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.AddMob(mob);
        }

        public MobConfigModel? GetMobByHandler(short mapId, int handler, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (map == null)
                return null;

            return map.Mobs.FirstOrDefault(x => x.GeneralHandler == handler);
        }

        public SummonMobModel? GetMobByHandler(short mapId, int handler, bool summon, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.SummonMobs.FirstOrDefault(x => x.GeneralHandler == handler);
        }

        public List<MobConfigModel> GetMobsNearbyPartner(Location location, int range, long tamerId)
        {
            var targetMap = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (targetMap == null)
                return new List<MobConfigModel>();

            var originX = location.X;
            var originY = location.Y;

            return GetTargetMobs(targetMap.Mobs.Where(x => x.Alive).ToList(), originX, originY, range)
                .DistinctBy(x => x.Id).ToList();
        }

        public List<MobConfigModel> GetMobsNearbyTargetMob(short mapId, int handler, int range, long tamerId)
        {
            var targetMap = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (targetMap == null)
                return new List<MobConfigModel>();

            var originMob = targetMap.Mobs.FirstOrDefault(x => x.GeneralHandler == handler);

            if (originMob == null)
                return new List<MobConfigModel>();

            var originX = originMob.CurrentLocation.X;
            var originY = originMob.CurrentLocation.Y;

            var targetMobs = new List<MobConfigModel>();
            targetMobs.Add(originMob);

            targetMobs.AddRange(GetTargetMobs(targetMap.Mobs.Where(x => x.Alive).ToList(), originX, originY,
                range / 5));

            return targetMobs.DistinctBy(x => x.Id).ToList();
        }

        public static List<MobConfigModel> GetTargetMobs(List<MobConfigModel> mobs, int originX, int originY, int range)
        {
            var targetMobs = new List<MobConfigModel>();

            foreach (var mob in mobs)
            {
                var mobX = mob.CurrentLocation.X;
                var mobY = mob.CurrentLocation.Y;

                var distance = CalculateDistance(originX, originY, mobX, mobY);

                if (distance <= range)
                {
                    targetMobs.Add(mob);
                }
            }

            return targetMobs;
        }

        // --------------------------------------------------------------------------------------------------------------

        public List<SummonMobModel> GetMobsNearbyPartner(Location location, int range, bool Summon, long tamerId)
        {
            var targetMap = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (targetMap == null)
                return new List<SummonMobModel>();

            var originX = location.X;
            var originY = location.Y;

            return GetTargetMobs(targetMap.SummonMobs.Where(x => x.Alive).ToList(), originX, originY, range)
                .DistinctBy(x => x.Id).ToList();
        }

        public List<SummonMobModel> GetMobsNearbyTargetMob(short mapId, int handler, int range, bool Summon,
            long tamerId)
        {
            var targetMap = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (targetMap == null)
                return new List<SummonMobModel>();

            var originMob = targetMap.SummonMobs.FirstOrDefault(x => x.GeneralHandler == handler);

            if (originMob == null)
                return new List<SummonMobModel>();

            var originX = originMob.CurrentLocation.X;
            var originY = originMob.CurrentLocation.Y;

            var targetMobs = new List<SummonMobModel>();
            targetMobs.Add(originMob);

            targetMobs.AddRange(GetTargetMobs(targetMap.SummonMobs.Where(x => x.Alive).ToList(), originX, originY,
                range / 5));

            return targetMobs.DistinctBy(x => x.Id).ToList();
        }

        public static List<SummonMobModel> GetTargetMobs(List<SummonMobModel> mobs, int originX, int originY, int range)
        {
            var targetMobs = new List<SummonMobModel>();

            foreach (var mob in mobs)
            {
                var mobX = mob.CurrentLocation.X;
                var mobY = mob.CurrentLocation.Y;

                var distance = CalculateDistance(originX, originY, mobX, mobY);

                if (distance <= range)
                {
                    targetMobs.Add(mob);
                }
            }

            return targetMobs;
        }

        private static double CalculateDistance(int x1, int y1, int x2, int y2)
        {
            var deltaX = x2 - x1;
            var deltaY = y2 - y1;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}