using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Packets.MapServer;
using System.Diagnostics;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class PvpServer
    {
        private DateTime _lastMapsSearch = DateTime.Now;
        private DateTime _lastMobsSearch = DateTime.Now;

        private byte _loadChannel = 0;

        private readonly int _startToSee = 6000;
        private readonly int _stopSeeing = 6001;

        #region CleanMaps

        /// <summary>
        /// Cleans unused running maps.
        /// </summary>
        public Task CleanMaps()
        {
            var mapsToRemove = new List<GameMap>();
            mapsToRemove.AddRange(Maps.Where(x => x.CloseMap));

            foreach (var map in mapsToRemove)
            {
                _logger.Debug($"Removing inactive pvp instance for map {map.Id} : {map.Name}");
                Maps.Remove(map);
            }

            return Task.CompletedTask;
        }

        public Task CleanMap(int ChannelId)
        {
            var mapToClose = Maps.FirstOrDefault(x => x.Channel == ChannelId);

            if (mapToClose != null)
            {
                Maps.Remove(mapToClose);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region SearchNewMaps

        /// <summary>
        /// Search for new maps to instance.
        /// </summary>
        public async Task SearchNewMaps(CancellationToken cancellationToken)
        {
            if (DateTime.Now > _lastMapsSearch)
            {
                var mapsToLoad =
                    _mapper.Map<List<GameMap>>(await _sender.Send(new GameMapsConfigQuery(MapTypeEnum.Pvp),
                        cancellationToken));

                foreach (var newMap in mapsToLoad)
                {
                    if (!Maps.Any(x => x.Id == newMap.Id))
                    {
                        _logger.Debug($"Adding new pvp map {newMap.Id} : {newMap.Name}");
                        Maps.Add(newMap);
                    }
                }

                _lastMapsSearch = DateTime.Now.AddSeconds(10);
            }
        }

        public async Task SearchNewMaps(GameClient client)
        {
            var mapsToLoad = _mapper.Map<List<GameMap>>(await _sender.Send(new GameMapConfigsQuery()));

            foreach (var newMap in mapsToLoad)
            {
                if (newMap.MapId == client.Tamer.Location.MapId)
                {
                    if (!Maps.Any(x => x.MapId == client.Tamer.Location.MapId && x.Channel == client.Tamer.Channel))
                    {
                        if (newMap.Type == MapTypeEnum.Pvp)
                        {
                            newMap.Channel = client.Tamer.Channel;
                            _logger.Information(
                                $"Initializing new Channel for pvp map {newMap.Id} : {newMap.Name} Ch {client.Tamer.Channel}");
                            Maps.Add(newMap);
                        }
                    }
                }
            }

            _lastMapsSearch = DateTime.Now.AddSeconds(10);
        }

        #endregion

        #region Get Mobs / Maps

        /// <summary>
        /// Gets the maps objects.
        /// </summary>
        public async Task GetMapObjects(CancellationToken cancellationToken)
        {
            //await GetMapConsignedShops(cancellationToken);
            await GetMapMobs(cancellationToken);
        }

        /// <summary>
        /// Gets the map latest mobs.
        /// </summary>
        /// <returns>The mobs collection</returns>
        private async Task GetMapMobs(CancellationToken cancellationToken)
        {
            if (DateTime.Now > _lastMobsSearch)
            {
                // Take a snapshot of initialized maps
                var initializedMaps = Maps.Where(x => x.Initialized).ToList();

                foreach (var map in initializedMaps)
                {
                    var mapMobs =
                        _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(map.Id),
                            cancellationToken));

                    if (map.RequestMobsUpdate(mapMobs))
                        map.UpdateMobsList();
                }

                _lastMobsSearch = DateTime.Now.AddSeconds(30);
            }
        }

        #endregion

        #region Start / Run

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
                    await SearchNewMaps(cancellationToken);
                    await GetMapObjects(cancellationToken);

                    var tasks = new List<Task>();

                    Maps.ForEach(map => { tasks.Add(RunMap(map)); });

                    await Task.WhenAll(tasks);

                    await Task.Delay(500, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Unexpected PvP map exception: {ex.Message} {ex.StackTrace}");
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
                if (map.Initialized == false)
                    map.Initialize();

                map.ManageHandlers();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var tasks = new List<Task>
                {
                    Task.Run(() => TamerOperation(map)),
                    Task.Run(() => MonsterOperation(map)),
                    //Task.Run(() => DropsOperation(map))
                };

                await Task.WhenAll(tasks);

                stopwatch.Stop();
                var totalTime = stopwatch.Elapsed.TotalMilliseconds;

                if (totalTime >= 1000)
                    Console.WriteLine($"Run pvp Map ({map.MapId}): {totalTime}.");

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error at map running: {ex.Message} {ex.StackTrace}.");
            }
        }

        #endregion

        public Drop? GetDrop(short mapId, int dropHandler, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.GetDrop(dropHandler);
        }

        public void RemoveDrop(Drop drop, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.RemoveMapDrop(drop);
        }

        #region Add / Remove Client

        /// <summary>
        /// Adds a new gameclient to the target map.
        /// </summary>
        /// <param name="client">The game client to be added.</param>
        public async Task AddClient(GameClient client)
        {
            var map = Maps.FirstOrDefault(x =>
                x.Initialized && x.MapId == client.Tamer.Location.MapId && x.Channel == client.Tamer.Channel);

            client.SetLoading();

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
                    var stopWatch = Stopwatch.StartNew();
                    var timeLimit = 15000;

                    while (map == null)
                    {
                        await Task.Delay(2500);

                        map = Maps.FirstOrDefault(x =>
                            x.Initialized && x.MapId == client.Tamer.Location.MapId &&
                            x.Channel == client.Tamer.Channel);

                        _loadChannel = client.Tamer.Channel;
                        _logger.Information(
                            $"Waiting pvp map {client.Tamer.Location.MapId} CH {_loadChannel} initialization.");

                        if (map == null)
                            await SearchNewMaps(client);

                        if (stopWatch.ElapsedMilliseconds >= timeLimit)
                        {
                            _logger.Error(
                                $"The map {client.Tamer.Location.MapId} CH {_loadChannel} was not found, aborting process...");
                            //stopWatch.Stop();
                            break;
                        }
                    }

                    if (map == null)
                    {
                        _loadChannel = client.Tamer.Channel;
                        client.Disconnect();
                    }
                    else
                    {
                        client.Tamer.MobsInView.Clear();
                        map.AddClient(client);
                        client.Tamer.Revive();
                    }
                });
            }
        }

        /// <summary>
        /// Removes the gameclient from the target map.
        /// </summary>
        /// <param name="client">The gameclient to be removed.</param>
        public void RemoveClient(GameClient client)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == client.TamerId));

            map?.BroadcastForTargetTamers(client.TamerId,
                new LocalMapSwapPacket(
                    client.Tamer.GeneralHandler,
                    client.Tamer.Partner.GeneralHandler,
                    client.Tamer.Location.X,
                    client.Tamer.Location.Y,
                    client.Tamer.Partner.Location.X,
                    client.Tamer.Partner.Location.Y
                ).Serialize()
            );

            map?.RemoveClient(client);
        }

        #endregion

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

        public void BroadcastForMap(short mapId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.MapId == mapId);

            map?.BroadcastForMap(packet);
        }

        public void BroadcastForUniqueTamer(long tamerId, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            map?.BroadcastForUniqueTamer(tamerId, packet);
        }

        public void BroadcastForMapAllChannels(short mapId, byte[] packet)
        {
            var maps = Maps.Where(x => x.Clients.Exists(gameClient => gameClient.Tamer.Location.MapId == mapId))
                .SelectMany(map => map.Clients);
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

        public void BroadcastForTargetTamers(List<long> targetTamers, byte[] packet)
        {
            var map = Maps.FirstOrDefault(
                x => x.Clients.Exists(gameClient => targetTamers.Contains(gameClient.TamerId)));

            map?.BroadcastForTargetTamers(targetTamers, packet);
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

        public void BroadcastForTamerViews(GameClient client, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient =>
                gameClient.TamerId == client.TamerId && gameClient.Tamer.Channel == client.Tamer.Channel));

            map?.BroadcastForTamerViewOnly(client.TamerId, packet);
        }

        public void BroadcastForTamerViewsAndSelf(GameClient client, byte[] packet)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient =>
                gameClient.TamerId == client.TamerId && gameClient.Tamer.Channel == client.Tamer.Channel));

            map?.BroadcastForTamerViewsAndSelf(client.TamerId, packet);
        }

        public bool MobsAttacking(short mapId, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.MobsAttacking(tamerId) ?? false;
        }

        public bool EnemiesAttacking(short mapId, long partnerId, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            return map?.PlayersAttacking(partnerId) ?? false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------

        public DigimonModel? GetEnemyByHandler(short mapId, int handler)
        {
            return Maps.FirstOrDefault(x => x.MapId == mapId)?.ConnectedTamers
                .Select(x => x.Partner).FirstOrDefault(x => x.GeneralHandler == handler);
        }

        public DigimonModel? GetEnemyByHandler(short mapId, int handler, long tamerId)
        {
            return Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId))
                ?.ConnectedTamers
                .Select(x => x.Partner).FirstOrDefault(x => x.GeneralHandler == handler);
        }

        // ----------------------------------------------------------------------------------------------------------------------------

        public MobConfigModel? GetMobByHandler(short mapId, int handler, long tamerId)
        {
            var map = Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == tamerId));

            if (map == null)
                return null;

            return map.Mobs.FirstOrDefault(x => x.GeneralHandler == handler);
        }

        // ----------------------------------------------------------------------------------------------------------------------------

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

        public List<MobConfigModel> GetMobsNearbyPartner(Location location, int range)
        {
            var targetMap = Maps.FirstOrDefault(x => x.MapId == location.MapId);
            if (targetMap == null)
                return default;

            var originX = location.X;
            var originY = location.Y;

            return GetTargetMobs(targetMap.Mobs.Where(x => x.Alive).ToList(), originX, originY, range)
                .DistinctBy(x => x.Id).ToList();
        }

        public List<MobConfigModel> GetMobsNearbyTargetMob(short mapId, int handler, int range)
        {
            var targetMap = Maps.FirstOrDefault(x => x.MapId == mapId);
            if (targetMap == null)
                return default;

            var originMob = targetMap.Mobs.FirstOrDefault(x => x.GeneralHandler == handler);

            if (originMob == null)
                return default;

            var originX = originMob.CurrentLocation.X;
            var originY = originMob.CurrentLocation.Y;

            var targetMobs = new List<MobConfigModel>();
            targetMobs.Add(originMob);

            targetMobs.AddRange(GetTargetMobs(targetMap.Mobs.Where(x => x.Alive).ToList(), originX, originY, range));

            return targetMobs.DistinctBy(x => x.Id).ToList();
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

        private static double CalculateDistance(int x1, int y1, int x2, int y2)
        {
            var deltaX = x2 - x1;
            var deltaY = y2 - y1;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}