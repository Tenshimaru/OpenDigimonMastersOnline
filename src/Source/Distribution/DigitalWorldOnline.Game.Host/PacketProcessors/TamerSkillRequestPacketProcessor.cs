using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TamerSkillRequestPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerSkillRequest;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly AssetsLoader _assets;
        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly SemaphoreSlim _threadSemaphore = new SemaphoreSlim(1, 1);

        public TamerSkillRequestPacketProcessor(ILogger logger, ISender sender, AssetsLoader assets,
            PartyManager partyManager, MapServer mapserver, DungeonsServer dungeonServer, EventServer eventServer,
            PvpServer pvpServer)
        {
            _logger = logger;
            _sender = sender;
            _assets = assets;
            _partyManager = partyManager;
            _mapServer = mapserver;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            int SkillId = packet.ReadInt();

            _logger.Debug($"SkillId: {SkillId}");

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            var tamerSkill = _assets.TamerSkills.FirstOrDefault(x => x.SkillId == SkillId);

            if (tamerSkill == null)
            {
                _logger.Error($"SkillId: {SkillId} not found in TamerSkill !!");
                return;
            }
            else
            {
                if (tamerSkill.BuffId == 0)
                {
                    var skillInfo = _assets.SkillInfo.FirstOrDefault(x => x.SkillId == tamerSkill.SkillCode);

                    if (skillInfo == null)
                    {
                        _logger.Error($"skillInfo: {SkillId} not found in Asset.SkillInfo !!");
                        return;
                    }
                    else
                    {
                        var TargetType = (SkillTargetTypeEnum)skillInfo.Target;

                        _logger.Debug($"TamerSkill Type: {TargetType}");

                        switch (TargetType)
                        {
                            case SkillTargetTypeEnum.Tamer:
                                break;
                            case SkillTargetTypeEnum.Digimon:
                                {
                                    //await TamerSkillUniqueTarget(client, SkillId, tamerSkill, buffinfo, skillInfo, mapConfig);
                                }
                                break;
                            case SkillTargetTypeEnum.Both:
                                break;
                            case SkillTargetTypeEnum.Party:
                                {
                                    //await PartySkillSwitch(client, SkillId, tamerSkill, buffinfo, skillInfo, mapConfig);
                                }
                                break;
                            case SkillTargetTypeEnum.Mob:
                                {
                                    await TamerSkillOnMob(client, SkillId, tamerSkill, skillInfo, mapConfig);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
                else
                {
                    var buffinfo = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == tamerSkill?.BuffId);

                    if (buffinfo == null)
                    {
                        _logger.Error($"buffinfo: {tamerSkill?.BuffId} not found in Asset.Buff !!");
                        return;
                    }
                    else
                    {
                        var skillInfo = _assets.SkillInfo.FirstOrDefault(x => x.SkillId == tamerSkill.SkillCode);

                        if (skillInfo == null)
                        {
                            _logger.Error($"skillInfo: {SkillId} not found in Asset.SkillInfo !!");
                            return;
                        }
                        else
                        {
                            var TargetType = (SkillTargetTypeEnum)skillInfo.Target;

                            switch (TargetType)
                            {
                                case SkillTargetTypeEnum.Tamer:
                                    break;
                                case SkillTargetTypeEnum.Digimon:
                                    {
                                        await TamerSkillUniqueTarget(client, SkillId, tamerSkill, buffinfo, skillInfo,
                                            mapConfig);
                                    }
                                    break;
                                case SkillTargetTypeEnum.Both:
                                    break;
                                case SkillTargetTypeEnum.Party:
                                    {
                                        await PartySkillSwitch(client, SkillId, tamerSkill, buffinfo, skillInfo, mapConfig);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                }
            }
        }

        private async Task TamerSkillUniqueTarget(GameClient client, int SkillId, TamerSkillAssetModel? targetSkill,
            BuffInfoAssetModel? targetBuffInfo, SkillInfoAssetModel? TargetSkillInfo, MapConfigDTO? mapConfig)
        {
            var duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration);

            client.Send(new TamerSkillRequestPacket(SkillId, targetBuffInfo.BuffId, duration));

            var newDigimonSkillBuff = DigimonBuffModel.Create(targetBuffInfo.BuffId, targetSkill.SkillCode, 0,
                targetSkill.Duration, (int)(TargetSkillInfo.Cooldown / 1000.0));
            newDigimonSkillBuff.SetBuffInfo(targetBuffInfo);

            var activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

            if (activeSkill != null)
            {
                activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
            }
            else
            {
                activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0), TamerSkillTypeEnum.Normal);
            }

            if (!targetBuffInfo.Pray && !targetBuffInfo.Cheer)
            {
                var buffToRemove =
                    client.Tamer.Partner.BuffList.ActiveBuffs.FirstOrDefault(x =>
                        x.SkillId == newDigimonSkillBuff.SkillId);

                if (buffToRemove != null)
                {
                    duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration);

                    client.Tamer.Partner.BuffList.Buffs.Remove(buffToRemove);
                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler, newDigimonSkillBuff.BuffId)
                                    .Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler, newDigimonSkillBuff.BuffId)
                                    .Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler, newDigimonSkillBuff.BuffId)
                                    .Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler, newDigimonSkillBuff.BuffId)
                                    .Serialize());
                            break;
                    }
                }

                client.Tamer.Partner.BuffList.Add(newDigimonSkillBuff);

                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0, duration)
                                .Serialize());
                        _dungeonServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0, duration)
                                .Serialize());
                        _eventServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0, duration)
                                .Serialize());
                        _pvpServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0, duration)
                                .Serialize());
                        _mapServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;
                }
            }
            else
            {
                if (targetBuffInfo.Pray)
                {
                    var value = 40;

                    client.Tamer.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Tamer.HP));
                    client.Partner.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Partner.HP));

                    client.Tamer.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Tamer.DS));
                    client.Partner.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Partner.DS));
                }
                else if (targetBuffInfo.Cheer)
                {
                    var value = 100;

                    client.Tamer.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Tamer.HP));
                    client.Partner.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Partner.HP));

                    client.Tamer.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Tamer.DS));
                    client.Partner.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Partner.DS));
                }


                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTargetTamers(client.TamerId,
                            new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                client.Tamer.Partner.HpRate).Serialize());
                        break;
                }
            }

            client.Send(new UpdateStatusPacket(client.Tamer));

            await _sender.Send(new UpdateDigimonBuffListCommand(client.Tamer.Partner.BuffList));
            await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
        }

        private async Task PartySkillSwitch(GameClient client, int SkillId, TamerSkillAssetModel? targetSkill,
            BuffInfoAssetModel? targetBuffInfo, SkillInfoAssetModel? TargetSkillInfo, MapConfigDTO? mapConfig)
        {
            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                var targetClients = new List<CharacterModel>(party.Members.Values);


                foreach (var target in targetClients)
                {
                    var diff = UtilitiesFunctions.CalculateDistance(
                        client.Tamer.Location.X,
                        target.Location.X,
                        client.Tamer.Location.Y,
                        target.Location.Y);

                    //if (diff <= TargetSkillInfo.Range && target.Channel == client.Tamer.Channel && target.Location.MapId == client.Tamer.Location.MapId)
                    if (diff <= TargetSkillInfo.Range && target.Location.MapId == client.Tamer.Location.MapId)
                    {
                        if (client.DungeonMap)
                        {
                            var targetClient = _dungeonServer.FindClientByTamerHandle(target.GeneralHandler);

                            if (targetClient != null)
                            {
                                var duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration);

                                if (targetClient.Tamer.Id == client.Tamer.Id)
                                {
                                    client.Send(new TamerSkillRequestPacket(SkillId, targetBuffInfo.BuffId, duration)
                                        .Serialize());

                                    var activeSkill =
                                        client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

                                    if (activeSkill != null)
                                    {
                                        activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
                                    }
                                    else
                                    {
                                        activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                                        activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0),
                                            TamerSkillTypeEnum.Normal);
                                    }

                                    await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
                                }

                                if (targetBuffInfo.Type != 1)
                                {
                                    var newDigimonSkillBuff = DigimonBuffModel.Create(targetBuffInfo.BuffId,
                                        targetSkill.SkillCode, 0, targetSkill.Duration,
                                        (int)(TargetSkillInfo.Cooldown / 1000.0));
                                    newDigimonSkillBuff.SetBuffInfo(targetBuffInfo);

                                    if (targetClient.Tamer.Partner.BuffList.ActiveBuffs.FirstOrDefault(x =>
                                            x.SkillId == newDigimonSkillBuff.SkillId) != null)
                                    {
                                        targetClient.Tamer.Partner.BuffList.Buffs.Remove(newDigimonSkillBuff);
                                    }

                                    targetClient.Tamer.Partner.BuffList.Add(newDigimonSkillBuff);

                                    _dungeonServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                        new AddBuffPacket(targetClient.Tamer.Partner.GeneralHandler, targetBuffInfo,
                                            (short)0, duration).Serialize());
                                    _dungeonServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                                    await _sender.Send(
                                        new UpdateDigimonBuffListCommand(targetClient.Tamer.Partner.BuffList));
                                }
                                else
                                {
                                    if (targetBuffInfo.Pray)
                                    {
                                        var value = 40;

                                        targetClient.Tamer.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.HP));
                                        targetClient.Partner.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.HP));

                                        targetClient.Tamer.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.DS));
                                        targetClient.Partner.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.DS));
                                    }
                                    else if (targetBuffInfo.Cheer)
                                    {
                                        var value = 100;

                                        targetClient.Tamer.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.HP));
                                        targetClient.Partner.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.HP));

                                        targetClient.Tamer.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.DS));
                                        targetClient.Partner.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.DS));
                                    }

                                    _dungeonServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));
                                }
                            }
                        }
                        else
                        {
                            var targetClient = (_mapServer.FindClientByTamerHandle(target.GeneralHandler) ??
                                                _eventServer.FindClientByTamerHandle(target.GeneralHandler)) ??
                                               _pvpServer.FindClientByTamerHandle(target.GeneralHandler);

                            if (targetClient != null)
                            {
                                var duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration);

                                if (targetClient.Tamer.Id == client.Tamer.Id)
                                {
                                    client.Send(new TamerSkillRequestPacket(SkillId, targetBuffInfo.BuffId, duration)
                                        .Serialize());

                                    var activeSkill =
                                        client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

                                    if (activeSkill != null)
                                    {
                                        activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
                                    }
                                    else
                                    {
                                        activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                                        activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0),
                                            TamerSkillTypeEnum.Normal);
                                    }

                                    await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
                                }

                                if (targetBuffInfo is { Pray: false, Cheer: false })
                                {
                                    var newDigimonSkillBuff = DigimonBuffModel.Create(targetBuffInfo.BuffId,
                                        targetSkill.SkillCode, 0, targetSkill.Duration,
                                        (int)(TargetSkillInfo.Cooldown / 1000.0));
                                    newDigimonSkillBuff.SetBuffInfo(targetBuffInfo);

                                    var buffToRemove =
                                        targetClient.Tamer.Partner.BuffList.ActiveBuffs.FirstOrDefault(x =>
                                            x.SkillId == newDigimonSkillBuff.SkillId);

                                    if (buffToRemove != null)
                                    {
                                        duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration +
                                            buffToRemove.RemainingSeconds);

                                        targetClient.Tamer.Partner.BuffList.Buffs.Remove(buffToRemove);

                                        if (targetClient.DungeonMap)
                                        {
                                            _dungeonServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                                new RemoveBuffPacket(targetClient.Tamer.Partner.GeneralHandler,
                                                    newDigimonSkillBuff.BuffId).Serialize());
                                        }
                                        else
                                        {
                                            _mapServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                                new RemoveBuffPacket(targetClient.Tamer.Partner.GeneralHandler,
                                                    newDigimonSkillBuff.BuffId).Serialize());
                                            _eventServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                                new RemoveBuffPacket(targetClient.Tamer.Partner.GeneralHandler,
                                                    newDigimonSkillBuff.BuffId).Serialize());
                                            _pvpServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                                new RemoveBuffPacket(targetClient.Tamer.Partner.GeneralHandler,
                                                    newDigimonSkillBuff.BuffId).Serialize());
                                        }
                                    }

                                    targetClient.Tamer.Partner.BuffList.Add(newDigimonSkillBuff);

                                    _mapServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                        new AddBuffPacket(targetClient.Tamer.Partner.GeneralHandler, targetBuffInfo,
                                            (short)0, duration).Serialize());
                                    _mapServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    _eventServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                        new AddBuffPacket(targetClient.Tamer.Partner.GeneralHandler, targetBuffInfo,
                                            (short)0, duration).Serialize());
                                    _eventServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    _pvpServer.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                                        new AddBuffPacket(targetClient.Tamer.Partner.GeneralHandler, targetBuffInfo,
                                            (short)0, duration).Serialize());
                                    _pvpServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                                    await _sender.Send(
                                        new UpdateDigimonBuffListCommand(targetClient.Tamer.Partner.BuffList));
                                }
                                else
                                {
                                    if (targetBuffInfo != null && targetBuffInfo.Pray)
                                    {
                                        var value = 40;

                                        targetClient.Tamer.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.HP));
                                        targetClient.Partner.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.HP));

                                        targetClient.Tamer.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.DS));
                                        targetClient.Partner.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.DS));
                                    }
                                    else if (targetBuffInfo != null && targetBuffInfo.Cheer)
                                    {
                                        var value = 100;

                                        targetClient.Tamer.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.HP));
                                        targetClient.Partner.RecoverHp(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.HP));

                                        targetClient.Tamer.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Tamer.DS));
                                        targetClient.Partner.RecoverDs(
                                            (int)Math.Ceiling((double)(value) / 100 * targetClient.Partner.DS));
                                    }


                                    _mapServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());


                                    _eventServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());


                                    _pvpServer.BroadcastForTargetTamers(targetClient.TamerId,
                                        new UpdateCurrentHPRatePacket(targetClient.Tamer.Partner.GeneralHandler,
                                            targetClient.Tamer.Partner.HpRate).Serialize());

                                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration);

                client.Send(new TamerSkillRequestPacket(SkillId, targetBuffInfo.BuffId, duration));

                if (!targetBuffInfo.Pray && !targetBuffInfo.Cheer)
                {
                    var newDigimonSkillBuff = DigimonBuffModel.Create(targetBuffInfo.BuffId, targetSkill.SkillCode, 0,
                        targetSkill.Duration, (int)(TargetSkillInfo.Cooldown / 1000.0));
                    newDigimonSkillBuff.SetBuffInfo(targetBuffInfo);

                    var buffToRemove =
                        client.Tamer.Partner.BuffList.ActiveBuffs.FirstOrDefault(x =>
                            x.SkillId == newDigimonSkillBuff.SkillId);

                    if (buffToRemove != null)
                    {
                        duration = UtilitiesFunctions.RemainingTimeSeconds(targetSkill.Duration +
                                                                           buffToRemove.RemainingSeconds);

                        client.Tamer.Partner.BuffList.Buffs.Remove(buffToRemove);
                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,
                                            newDigimonSkillBuff.BuffId)
                                        .Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,
                                            newDigimonSkillBuff.BuffId)
                                        .Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,
                                            newDigimonSkillBuff.BuffId)
                                        .Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,
                                            newDigimonSkillBuff.BuffId)
                                        .Serialize());
                                break;
                        }
                    }


                    client.Tamer.Partner.BuffList.Add(newDigimonSkillBuff);

                    var activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

                    if (activeSkill != null)
                    {
                        activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
                    }
                    else
                    {
                        activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                        activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0),
                            TamerSkillTypeEnum.Normal);
                    }

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0,
                                        duration)
                                    .Serialize());
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                    client.Tamer.Partner.HpRate).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0,
                                        duration)
                                    .Serialize());
                            _eventServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                    client.Tamer.Partner.HpRate).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0,
                                        duration)
                                    .Serialize());
                            _pvpServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                    client.Tamer.Partner.HpRate).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AddBuffPacket(client.Tamer.Partner.GeneralHandler, targetBuffInfo, (short)0,
                                        duration)
                                    .Serialize());
                            _mapServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                    client.Tamer.Partner.HpRate).Serialize());
                            break;
                    }

                    client.Send(new UpdateStatusPacket(client.Tamer));

                    await _sender.Send(new UpdateDigimonBuffListCommand(client.Tamer.Partner.BuffList));
                    await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
                }
                else
                {
                    if (targetBuffInfo.Pray)
                    {
                        var value = 40;

                        client.Tamer.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Tamer.HP));
                        client.Partner.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Partner.HP));

                        client.Tamer.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Tamer.DS));
                        client.Partner.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Partner.DS));
                    }
                    else if (targetBuffInfo.Cheer)
                    {
                        var value = 100;

                        client.Tamer.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Tamer.HP));
                        client.Partner.RecoverHp((int)Math.Ceiling((double)(value) / 100 * client.Partner.HP));

                        client.Tamer.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Tamer.DS));
                        client.Partner.RecoverDs((int)Math.Ceiling((double)(value) / 100 * client.Partner.DS));
                    }


                    var activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

                    if (activeSkill != null)
                    {
                        activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
                    }
                    else
                    {
                        activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                        activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0),
                            TamerSkillTypeEnum.Normal);
                    }

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                        client.Tamer.Partner.HpRate)
                                    .Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                        client.Tamer.Partner.HpRate)
                                    .Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                        client.Tamer.Partner.HpRate)
                                    .Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTargetTamers(client.TamerId,
                                new UpdateCurrentHPRatePacket(client.Tamer.Partner.GeneralHandler,
                                        client.Tamer.Partner.HpRate)
                                    .Serialize());
                            break;
                    }

                    client.Send(new UpdateStatusPacket(client.Tamer));
                    await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
                }
            }
        }

        private async Task TamerSkillOnMob(GameClient client, int SkillId, TamerSkillAssetModel? targetSkill, SkillInfoAssetModel? TargetSkillInfo, MapConfigDTO? mapConfig)
        {
            var activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == SkillId);

            if (activeSkill != null)
            {
                activeSkill.SetCooldown((int)(TargetSkillInfo.Cooldown / 1000.0));
            }
            else
            {
                activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == 0);
                activeSkill.SetTamerSkill(SkillId, (int)(TargetSkillInfo.Cooldown / 1000.0), TamerSkillTypeEnum.Normal);
            }

            client.Send(new UpdateStatusPacket(client.Tamer));

            await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
        }
    }
}