using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Game.Managers;
using Microsoft.IdentityModel.Tokens;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost.EventsServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ComplementarInformationPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ComplementarInformation;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ComplementarInformationPacketProcessor(PartyManager partyManager, MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer, PvpServer pvpServer, AssetsLoader assets,
            ILogger logger, ISender sender, IMapper mapper)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            //_logger.Information($"Runing ComplementarInformationPacketProcessor ... **************************");

            _logger.Debug($"Sending seal info packet for character {client.TamerId}...");
            client.Send(new SealsPacket(client.Tamer.SealList));
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            if (client.Tamer.TamerShop?.Count > 0)
            {
                _logger.Debug($"Recovering tamer shop items for character {client.TamerId}...");
                client.Tamer.Inventory.AddItems(client.Tamer.TamerShop.Items);
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

                client.Tamer.TamerShop.Clear();
                await _sender.Send(new UpdateItemsCommand(client.Tamer.TamerShop));
            }

            UpdateSkillCooldown(client);

            try
            {
                _logger.Debug($"Sending inventory packet for character {client.TamerId}...");
                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro loading inventory for Tamer {client.TamerId}.\n{ex.Message}\n");
            }

            _logger.Debug($"Sending warehouse packet for character {client.TamerId}...");
            client.Send(new LoadInventoryPacket(client.Tamer.Warehouse, InventoryTypeEnum.Warehouse));

            if (client.Tamer.AccountWarehouse != null)
            {
                _logger.Debug($"Sending account warehouse packet for character {client.TamerId}...");
                client.Send(new LoadInventoryPacket(client.Tamer.AccountWarehouse, InventoryTypeEnum.AccountWarehouse));
            }

            _logger.Debug($"Getting server exp information for character {client.TamerId}...");
            var serverInfo = _mapper.Map<ServerObject>(await _sender.Send(new ServerByIdQuery(client.ServerId)));

            client.SetServerExperience(serverInfo.Experience);

            if (!client.DungeonMap)
            {
                _logger.Debug($"Sending server experience packet for character {client.TamerId}...");
                client.Send(new ServerExperiencePacket(serverInfo));
            }

            if (client.MembershipExpirationDate != null)
            {
                _logger.Debug($"Sending account membership duration packet for character {client.TamerId}...");
                client.Send(new MembershipPacket(client.MembershipExpirationDate.Value, client.MembershipUtcSeconds));

                var secondsUTC = (client.MembershipExpirationDate.Value - DateTime.UtcNow).TotalSeconds;

                if (secondsUTC <= 0)
                {
                    //_logger.Information($"Verifying if tamer have buffs without membership");

                    var buff = _assets.BuffInfo.Where(x => x.BuffId == 50121 || x.BuffId == 50122 || x.BuffId == 50123)
                        .ToList();

                    buff.ForEach(buffAsset =>
                    {
                        if (client.Tamer.BuffList.Buffs.Any(x => x.BuffId == buffAsset.BuffId))
                        {
                            var buffData = client.Tamer.BuffList.Buffs.First(x => x.BuffId == buffAsset.BuffId);

                            if (buffData != null)
                            {
                                buffData.SetDuration(0, true);
                                switch (mapConfig?.Type)
                                {
                                    case MapTypeEnum.Dungeon:
                                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, 0)
                                                .Serialize());
                                        break;

                                    case MapTypeEnum.Event:
                                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, 0)
                                                .Serialize());
                                        break;

                                    case MapTypeEnum.Pvp:
                                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, 0)
                                                .Serialize());
                                        break;

                                    default:
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, 0)
                                                .Serialize());
                                        break;
                                }
                            }
                        }
                    });

                    await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
                }
            }
            else
            {
                client.RemoveMembership();

                client.Send(new MembershipPacket());

                await _sender.Send(
                    new UpdateAccountMembershipCommand(client.AccountId, client.MembershipExpirationDate));
            }

            _logger.Debug($"Sending account cash coins packet for character {client.TamerId}...");
            client.Send(new CashShopCoinsPacket(client.Premium, client.Silk));

            _logger.Debug($"Sending time reward packet for character {client.TamerId}...");
            client.Send(new TimeRewardPacket(client.Tamer.TimeReward));

            if (client.ReceiveWelcome)
            {
                var welcomeMessages = await _sender.Send(new ActiveWelcomeMessagesAssetsQuery());

                _logger.Debug($"Sending welcome message packet for account {client.AccountId}...");
                client.Send(new WelcomeMessagePacket(welcomeMessages.PickRandom().Message));
            }

            if (client.Tamer.HasXai)
            {
                _logger.Debug($"Sending XAI info packet for character {client.TamerId}...");
                client.Send(new XaiInfoPacket(client.Tamer.Xai));

                _logger.Debug($"Sending tamer XAI resources packet for character {client.TamerId}...");
                client.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));
            }

            if (!client.SentOnceDataSent)
            {
                _logger.Debug($"Sending tamer relations packet for character {client.TamerId}...");
                client.Send(new TamerRelationsPacket(client.Tamer.Friends, client.Tamer.Foes));
                await _sender.Send(new UpdateCharacterInitialPacketSentOnceSentCommand(client.TamerId, true));

                if (!client.DungeonMap)
                {
                    var channels = new Dictionary<byte, byte>();

                    var mapChannels = await _sender.Send(new ChannelsByMapIdQuery(client.Tamer.Location.MapId));

                    foreach (var channel in mapChannels.OrderBy(x => x.Key))
                    {
                        channels.Add(channel.Key, channel.Value);
                    }

                    if (!channels.IsNullOrEmpty())
                    {
                        client.Send(new AvailableChannelsPacket(channels).Serialize());
                    }
                }
            }

            _logger.Debug($"Sending attendance event packet for character {client.TamerId}...");
            client.Send(new TamerAttendancePacket(client.Tamer.AttendanceReward));

            _logger.Debug($"Sending update status packet for character {client.TamerId}...");
            client.Send(new UpdateStatusPacket(client.Tamer));

            _logger.Debug($"Sending update movement speed packet for character {client.TamerId}...");
            client.Send(new UpdateMovementSpeedPacket(client.Tamer));

            _logger.Debug($"Searching guild information for character {client.TamerId}...");
            client.Tamer.SetGuild(
                _mapper.Map<GuildModel>(await _sender.Send(new GuildByCharacterIdQuery(client.TamerId))));

            if (client.Tamer.Guild != null)
            {
                foreach (var guildMember in client.Tamer.Guild.Members)
                {
                    if (guildMember.CharacterInfo == null)
                    {
                        GameClient? guildMemberClient;
                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                guildMemberClient = _dungeonServer.FindClientByTamerId(guildMember.CharacterId);
                                break;
                            case MapTypeEnum.Event:
                                guildMemberClient = _eventServer.FindClientByTamerId(guildMember.CharacterId);
                                break;

                            case MapTypeEnum.Pvp:
                                guildMemberClient = _pvpServer.FindClientByTamerId(guildMember.CharacterId);
                                break;

                            default:
                                guildMemberClient = _mapServer.FindClientByTamerId(guildMember.CharacterId);
                                break;
                        }

                        if (guildMemberClient != null)
                        {
                            guildMember.SetCharacterInfo(guildMemberClient.Tamer);
                        }
                        else
                        {
                            guildMember.SetCharacterInfo(
                                _mapper.Map<CharacterModel>(
                                    await _sender.Send(new CharacterByIdQuery(guildMember.CharacterId))));
                        }
                    }
                }

                foreach (var guildMember in client.Tamer.Guild.Members)
                {
                    if (client.ReceiveWelcome)
                    {
                        _logger.Debug($"Sending guild information packet for character {client.TamerId}...");
                        _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                            new GuildInformationPacket(client.Tamer.Guild).Serialize());
                        _dungeonServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                            new GuildInformationPacket(client.Tamer.Guild).Serialize());
                        _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                            new GuildInformationPacket(client.Tamer.Guild).Serialize());
                        _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                            new GuildInformationPacket(client.Tamer.Guild).Serialize());
                    }
                }

                _logger.Debug($"Sending guild historic packet for character {client.TamerId}...");
                client.Send(new GuildInformationPacket(client.Tamer.Guild));

                _logger.Debug($"Sending guild historic packet for character {client.TamerId}...");
                client.Send(new GuildHistoricPacket(client.Tamer.Guild.Historic));
            }

            /*if (client.ReceiveWelcome)
            {*/
            await _sender.Send(new UpdateCharacterFriendsCommand(client.Tamer, true));
            client.Tamer.Friended.ToList().ForEach(friend =>
            {
                // _logger.Information($"Sending friend connection packet for character {friend.CharacterId}...");
                _mapServer.BroadcastForUniqueTamer(friend.CharacterId,
                    new FriendConnectPacket(client.Tamer.Name).Serialize());
                _eventServer.BroadcastForUniqueTamer(friend.CharacterId,
                    new FriendConnectPacket(client.Tamer.Name).Serialize());
                _dungeonServer.BroadcastForUniqueTamer(friend.CharacterId,
                    new FriendConnectPacket(client.Tamer.Name).Serialize());
                _pvpServer.BroadcastForUniqueTamer(friend.CharacterId,
                    new FriendConnectPacket(client.Tamer.Name).Serialize());
            });

            if (client.Tamer.Guild != null)
            {
                _logger.Debug($"Getting guild rank position for guild {client.Tamer.Guild.Id}...");
                var guildRank = await _sender.Send(new GuildCurrentRankByGuildIdQuery(client.Tamer.Guild.Id));

                if (guildRank > 0 && guildRank <= 100)
                {
                    _logger.Debug($"Sending guild rank packet for character {client.TamerId}...");
                    client.Send(new GuildRankPacket(guildRank));
                }
            }
            /*}*/

            _logger.Debug($"Updating tamer state for character {client.TamerId}...");

            client.Tamer.UpdateSlots();
            client.Tamer.UpdateState(CharacterStateEnum.Ready);
            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Ready));

            _logger.Debug($"Updating account welcome flag for account {client.AccountId}...");
            await _sender.Send(new UpdateAccountWelcomeFlagCommand(client.AccountId, false));

            var mapTypeConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapTypeConfig!.Type)
            {
                case MapTypeEnum.Dungeon:
                {
                }
                    break;

                case MapTypeEnum.Event:
                {
                    var map = _eventServer.Maps.FirstOrDefault(x => x.MapId == client.Tamer.Location.MapId);

                    if (map != null)
                        NotifyTamerKillSpawnEnteringMap(client, map);
                }
                    break;

                case MapTypeEnum.Pvp:
                {
                }
                    break;

                case MapTypeEnum.Default:
                {
                    var map = _mapServer.Maps.FirstOrDefault(x => x.MapId == client.Tamer.Location.MapId);

                    if (map != null)
                        NotifyTamerKillSpawnEnteringMap(client, map);
                }
                    break;
            }

            /*if (!client.DungeonMap)
            {
                var map = _mapServer.Maps.FirstOrDefault(x => x.MapId == client.Tamer.Location.MapId);

                if (map != null)
                {
                    NotifyTamerKillSpawnEnteringMap(client, map);
                }
            }*/

            var currentMap = _assets.Maps.FirstOrDefault(x => x.MapId == client.Tamer.Location.MapId);

            if (currentMap != null)
            {
                var characterRegion = client.Tamer.MapRegions[currentMap.RegionIndex];

                if (characterRegion != null)
                {
                    if (characterRegion.Unlocked == 0)
                    {
                        characterRegion.Unlock();

                        await _sender.Send(new UpdateCharacterMapRegionCommand(characterRegion));
                        _logger.Verbose(
                            $"Character {client.TamerId} unlocked region {currentMap.RegionIndex} at {client.TamerLocation}.");
                    }
                }
                else
                {
                    client.Send(new SystemMessagePacket($"Unknown region index {currentMap.RegionIndex}."));
                    _logger.Warning(
                        $"Unknown region index {currentMap.RegionIndex} for character {client.TamerId} at {client.TamerLocation}.");
                }
            }
            else
            {
                client.Send(new SystemMessagePacket($"Unknown map info for map id {client.Tamer.Location.MapId}."));
                _logger.Warning($"Unknown map info for map id {client.Tamer.Location.MapId}.");
            }

            //_logger.Information($"***********************************************************************");
        }

        private void UpdateSkillCooldown(GameClient client)
        {
            if (client.Tamer.Partner.HasActiveSkills())
            {
                foreach (var evolution in client.Tamer.Partner.Evolutions)
                {
                    foreach (var skill in evolution.Skills)
                    {
                        if (skill.Duration > 0 && skill.Expired)
                        {
                            skill.ResetCooldown();
                        }
                    }

                    _sender.Send(new UpdateEvolutionCommand(evolution));
                }

                List<int> SkillIds = new List<int>(5);
                var packetEvolution =
                    client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                if (packetEvolution != null)
                {
                    var slot = -1;

                    foreach (var item in packetEvolution.Skills)
                    {
                        slot++;

                        var skillInfo = _assets.DigimonSkillInfo.FirstOrDefault(x =>
                            x.Type == client.Partner.CurrentType && x.Slot == slot);

                        if (skillInfo != null)
                        {
                            SkillIds.Add(skillInfo.SkillId);
                        }
                    }

                    client?.Send(new SkillUpdateCooldownPacket(client.Tamer.Partner.GeneralHandler,
                        client.Tamer.Partner.CurrentType, packetEvolution, SkillIds));
                }
            }
        }

        public void NotifyTamerKillSpawnEnteringMap(GameClient client, GameMap map)
        {
            foreach (var sourceKillSpawn in map.KillSpawns)
            {
                foreach (var mob in sourceKillSpawn.SourceMobs.Where(x => x.CurrentSourceMobRequiredAmount <= 10))
                {
                    NotifyMinimap(client, mob);
                }

                if (sourceKillSpawn.Spawn())
                {
                    NotifyMapChat(client, map, sourceKillSpawn);
                }
            }
        }

        private void NotifyMinimap(GameClient client, KillSpawnSourceMobConfigModel mob)
        {
            client.Send(new KillSpawnMinimapNotifyPacket(mob.SourceMobType, mob.CurrentSourceMobRequiredAmount)
                .Serialize());
        }

        private void NotifyMapChat(GameClient client, GameMap map, KillSpawnConfigModel sourceKillSpawn)
        {
            foreach (var targetMob in sourceKillSpawn.TargetMobs)
            {
                client.Send(new KillSpawnChatNotifyPacket(map.MapId, map.Channel, targetMob.TargetMobType).Serialize());
            }
        }
    }
}