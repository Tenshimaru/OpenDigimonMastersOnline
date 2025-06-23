using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Arena;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DungeonArenaNextStagePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DungeonArenaStageNext;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly DungeonsServer _dungeonServer;
        private readonly AssetsLoader _assetsLoader;
        private readonly PartyManager _partyManager;
        public DungeonArenaNextStagePacketProcessor(
            ILogger logger,
            ISender sender,
            AssetsLoader assetsLoader,
            DungeonsServer dungeonsServer,
            PartyManager partyManager,
            IMapper mapper)
        {
            _logger = logger;
            _sender = sender;
            _dungeonServer = dungeonsServer;
            _assetsLoader = assetsLoader;
            _partyManager = partyManager;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            int NpcId = packet.ReadInt();

            var dungeonInfo = _assetsLoader.NpcColiseum.FirstOrDefault(x => x.NpcId == NpcId);
            if (dungeonInfo != null)
            {
                if (NpcId == 92137)
                {
                    var party = _partyManager.FindParty(client.TamerId);

                    if (party != null)
                    {
                        var targetClients = _dungeonServer.Maps.FirstOrDefault(x => x.Clients.Any() && x.DungeonId == party.Id);

                        if (targetClients != null)
                        {

                            foreach (var target in targetClients.Clients)
                            {
                                target.Tamer.Points.UpdatePoints(0, 0, target.Tamer.Points.CurrentStage + 1);
                            }

                            var time = 600 * 5;

                            _dungeonServer.BroadcastForMap(client.Tamer.Location.MapId, new DungeonArenaNextStagePacket(client.Tamer.Points.CurrentStage, NpcId, time).Serialize(), client.TamerId);

                            await Task.Delay(time);

                            var mapMobs = _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(targetClients.Id)));

                            if (mapMobs != null)
                            {
                                var mobsToAdd = mapMobs.Where(x => x.ColiseumMobType == ColiseumMobTypeEnum.Normal && x.Coliseum && x.Round == client.Tamer.Points.CurrentStage).ToList();

                                if (mobsToAdd.Any())
                                {
                                    var mobInfo = dungeonInfo.MobInfo.FirstOrDefault(x => x.Round == client.Tamer.Points.CurrentStage);

                                    if (mobInfo != null)
                                    {
                                        if (!targetClients.ColiseumMobs.Contains(NpcId))
                                            targetClients.ColiseumMobs.Add(NpcId);

                                        if (mobInfo.SummonType == 0)
                                        {
                                            var chance = 80;

                                            var random = new Random();

                                            if (random.Next(1, 100) >= chance)
                                            {
                                                foreach (var mob in mobsToAdd)
                                                {
                                                    var MobClone = (MobConfigModel)mob.Clone();

                                                    int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                    // Gerando valores aleatórios para deslocamento em X e Y
                                                    int xOffset = random.Next(-radius, radius + 1);
                                                    int yOffset = random.Next(-radius, radius + 1);

                                                    // Calculando as novas coordenadas do chefe de raid
                                                    int bossX = MobClone.Location.X + xOffset;
                                                    int bossY = MobClone.Location.Y + yOffset;

                                                    MobClone.SetId(targetClients.Mobs.Count + 1);
                                                    MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                    targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                    _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                                }
                                            }
                                        }
                                        else if (mobInfo.SummonType == 1)
                                        {
                                            var count = 0;

                                            foreach (var mob in mobsToAdd)
                                            {
                                                count++;

                                                if (count == 2)
                                                    return;

                                                var random = new Random();

                                                var MobClone = (MobConfigModel)mob.Clone();

                                                int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                // Gerando valores aleatórios para deslocamento em X e Y
                                                int xOffset = random.Next(-radius, radius + 1);
                                                int yOffset = random.Next(-radius, radius + 1);

                                                // Calculando as novas coordenadas do chefe de raid
                                                int bossX = MobClone.Location.X + xOffset;
                                                int bossY = MobClone.Location.Y + yOffset;

                                                MobClone.SetId(targetClients.Mobs.Count + 1);
                                                MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                            }
                                        }
                                        else if (mobInfo.SummonType == 2)
                                        {
                                            var count = 0;

                                            foreach (var mob in mobsToAdd)
                                            {
                                                count++;

                                                if (count == 3)
                                                    return;

                                                var random = new Random();

                                                var MobClone = (MobConfigModel)mob.Clone();

                                                int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                // Gerando valores aleatórios para deslocamento em X e Y
                                                int xOffset = random.Next(-radius, radius + 1);
                                                int yOffset = random.Next(-radius, radius + 1);

                                                // Calculando as novas coordenadas do chefe de raid
                                                int bossX = MobClone.Location.X + xOffset;
                                                int bossY = MobClone.Location.Y + yOffset;

                                                MobClone.SetId(targetClients.Mobs.Count + 1);
                                                MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        var targetClients = _dungeonServer.Maps.FirstOrDefault(x => x.Clients.Any() && x.DungeonId == client.TamerId);

                        if (targetClients == null)
                            return;

                        if (targetClients.Clients == null)
                            return;

                        foreach (var target in targetClients.Clients)
                        {

                            target.Tamer.Points.UpdatePoints(0, 0, target.Tamer.Points.CurrentStage + 1);
                            await _sender.Send(new UpdateCharacterArenaPointsCommand(target.Tamer.Points));
                        }

                        var time = 600 * 10;

                        _dungeonServer.BroadcastForMap(client.Tamer.Location.MapId, new DungeonArenaNextStagePacket(client.Tamer.Points.CurrentStage, NpcId, time).Serialize(), client.TamerId);

                        await Task.Delay(time);

                        var mapMobs = _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(targetClients.Id)));

                        if (mapMobs != null)
                        {
                            var mobsToAdd = mapMobs.Where(x => x.ColiseumMobType == Commons.Enums.ClientEnums.ColiseumMobTypeEnum.Normal && x.Coliseum && x.Round == client.Tamer.Points.CurrentStage).ToList();

                            if (mobsToAdd.Any())
                            {
                                var mobInfo = dungeonInfo.MobInfo.FirstOrDefault(x => x.Round == client.Tamer.Points.CurrentStage);

                                if (mobInfo != null)
                                {
                                    if (!targetClients.ColiseumMobs.Contains(NpcId))
                                        targetClients.ColiseumMobs.Add(NpcId);

                                    if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.All)
                                    {

                                        foreach (var mob in mobsToAdd)
                                        {
                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }

                                    }
                                    else if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.One)
                                    {
                                        var count = 0;

                                        foreach (var mob in mobsToAdd)
                                        {
                                            count++;

                                            if (count == 2)
                                                return;

                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }
                                    }
                                    else if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.Two)
                                    {
                                        var count = 0;

                                        foreach (var mob in mobsToAdd)
                                        {
                                            count++;

                                            if (count == 3)
                                                return;

                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);
                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }
                                    }
                                }

                            }
                        }


                    }
                }
                else if (NpcId == 92138)
                {
                    var party = _partyManager.FindParty(client.TamerId);

                    if (party != null)
                    {
                        var targetClients = _dungeonServer.Maps.FirstOrDefault(x => x.Clients.Any() && x.DungeonId == party.Id);

                        if (targetClients != null)
                        {

                            foreach (var target in targetClients.Clients)
                            {
                                target.Tamer.Points.UpdatePoints(0, 0, target.Tamer.Points.CurrentStage + 1);
                            }

                            var time = 600 * 5;

                            _dungeonServer.BroadcastForMap(client.Tamer.Location.MapId, new DungeonArenaNextStagePacket(client.Tamer.Points.CurrentStage, NpcId, time).Serialize(), client.TamerId);

                            await Task.Delay(time);

                            var mapMobs = _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(targetClients.Id)));

                            if (mapMobs != null)
                            {

                                var roundAtual = (client.Tamer.Points.CurrentStage + 100);
                                var mobsToAdd = mapMobs.Where(x => x.ColiseumMobType == ColiseumMobTypeEnum.Hard && x.Coliseum && x.Round == (client.Tamer.Points.CurrentStage + 100)).ToList();

                                if (mobsToAdd.Any())
                                {
                                    var mobInfo = dungeonInfo.MobInfo.FirstOrDefault(x => x.Round == client.Tamer.Points.CurrentStage);

                                    if (mobInfo != null)
                                    {
                                        if (!targetClients.ColiseumMobs.Contains(NpcId))
                                            targetClients.ColiseumMobs.Add(NpcId);

                                        if (mobInfo.SummonType == 0)
                                        {
                                            var chance = 80;

                                            var random = new Random();

                                            if (random.Next(1, 100) >= chance)
                                            {
                                                foreach (var mob in mobsToAdd)
                                                {
                                                    var MobClone = (MobConfigModel)mob.Clone();

                                                    int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                    // Gerando valores aleatórios para deslocamento em X e Y
                                                    int xOffset = random.Next(-radius, radius + 1);
                                                    int yOffset = random.Next(-radius, radius + 1);

                                                    // Calculando as novas coordenadas do chefe de raid
                                                    int bossX = MobClone.Location.X + xOffset;
                                                    int bossY = MobClone.Location.Y + yOffset;

                                                    MobClone.SetId(targetClients.Mobs.Count + 1);
                                                    MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                    targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                    _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                                }
                                            }
                                        }
                                        else if (mobInfo.SummonType == 1)
                                        {
                                            var count = 0;

                                            foreach (var mob in mobsToAdd)
                                            {
                                                count++;

                                                if (count == 2)
                                                    return;

                                                var random = new Random();

                                                var MobClone = (MobConfigModel)mob.Clone();

                                                int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                // Gerando valores aleatórios para deslocamento em X e Y
                                                int xOffset = random.Next(-radius, radius + 1);
                                                int yOffset = random.Next(-radius, radius + 1);

                                                // Calculando as novas coordenadas do chefe de raid
                                                int bossX = MobClone.Location.X + xOffset;
                                                int bossY = MobClone.Location.Y + yOffset;

                                                MobClone.SetId(targetClients.Mobs.Count + 1);
                                                MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                            }
                                        }
                                        else if (mobInfo.SummonType == 2)
                                        {
                                            var count = 0;

                                            foreach (var mob in mobsToAdd)
                                            {
                                                count++;

                                                if (count == 3)
                                                    return;

                                                var random = new Random();

                                                var MobClone = (MobConfigModel)mob.Clone();

                                                int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                                // Gerando valores aleatórios para deslocamento em X e Y
                                                int xOffset = random.Next(-radius, radius + 1);
                                                int yOffset = random.Next(-radius, radius + 1);

                                                // Calculando as novas coordenadas do chefe de raid
                                                int bossX = MobClone.Location.X + xOffset;
                                                int bossY = MobClone.Location.Y + yOffset;

                                                MobClone.SetId(targetClients.Mobs.Count + 1);
                                                MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                                targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                                _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        var targetClients = _dungeonServer.Maps.FirstOrDefault(x => x.Clients.Any() && x.DungeonId == client.TamerId);

                        if (targetClients == null)
                            return;

                        if (targetClients.Clients == null)
                            return;

                        foreach (var target in targetClients.Clients)
                        {

                            target.Tamer.Points.UpdatePoints(0, 0, target.Tamer.Points.CurrentStage + 1);
                            await _sender.Send(new UpdateCharacterArenaPointsCommand(target.Tamer.Points));
                        }

                        var time = 600 * 10;

                        _dungeonServer.BroadcastForMap(client.Tamer.Location.MapId, new DungeonArenaNextStagePacket(client.Tamer.Points.CurrentStage, NpcId, time).Serialize(), client.TamerId);

                        await Task.Delay(time);

                        var mapMobs = _mapper.Map<IList<MobConfigModel>>(await _sender.Send(new MapMobConfigsQuery(targetClients.Id)));

                        if (mapMobs != null)
                        {
                            var roundAtual = (client.Tamer.Points.CurrentStage + 100);
                            var mobsToAdd = mapMobs.Where(x => x.ColiseumMobType == Commons.Enums.ClientEnums.ColiseumMobTypeEnum.Hard && x.Round == roundAtual).ToList();

                            if (mobsToAdd.Any())
                            {
                                var mobInfo = dungeonInfo.MobInfo.FirstOrDefault(x => x.Round == client.Tamer.Points.CurrentStage);

                                if (mobInfo != null)
                                {
                                    if (!targetClients.ColiseumMobs.Contains(NpcId))
                                        targetClients.ColiseumMobs.Add(NpcId);

                                    if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.All)
                                    {

                                        foreach (var mob in mobsToAdd)
                                        {
                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }

                                    }
                                    else if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.One)
                                    {
                                        var count = 0;

                                        foreach (var mob in mobsToAdd)
                                        {
                                            count++;

                                            if (count == 2)
                                                return;

                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);

                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }
                                    }
                                    else if (mobInfo.SummonType == (int)ColiseumSummonTypeEnum.Two)
                                    {
                                        var count = 0;

                                        foreach (var mob in mobsToAdd)
                                        {
                                            count++;

                                            if (count == 3)
                                                return;

                                            var random = new Random();

                                            var MobClone = (MobConfigModel)mob.Clone();

                                            int radius = 500; // Ajuste este valor para controlar a dispersão dos chefes

                                            // Gerando valores aleatórios para deslocamento em X e Y
                                            int xOffset = random.Next(-radius, radius + 1);
                                            int yOffset = random.Next(-radius, radius + 1);

                                            // Calculando as novas coordenadas do chefe de raid
                                            int bossX = MobClone.Location.X + xOffset;
                                            int bossY = MobClone.Location.Y + yOffset;

                                            MobClone.SetId(targetClients.Mobs.Count + 1);
                                            MobClone.SetLocation(client.Tamer.Location.MapId, bossX, bossY);

                                            targetClients.ColiseumMobs.Add((int)MobClone.Id);
                                            _dungeonServer.AddMobs(client.Tamer.Location.MapId, MobClone, client.TamerId);
                                        }
                                    }
                                }

                            }
                        }


                    }
                }
                else
                {
                    //var targetClients = _dungeonServer.Maps.Where(x => x.Clients.Any() && x.DungeonId == client.TamerId);
                }
            }
        }
    }
}
