using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartnerEvolutionPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerEvolution;

        private readonly PartyManager _partyManager;
        private readonly StatusManager _statusManager;
        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public PartnerEvolutionPacketProcessor(PartyManager partyManager, StatusManager statusManager,
            AssetsLoader assets,
            MapServer mapServer, DungeonsServer dungeonServer, EventServer eventServer, PvpServer pvpServer,
            ISender sender, ILogger logger)
        {
            _partyManager = partyManager;
            _statusManager = statusManager;
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var digimonHandle = packet.ReadInt();
            var evoStage = packet.ReadByte();
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            if (client.Partner == null)
            {
                client.Send(new DigimonEvolutionFailPacket());
                return;
            }

            var evoLine = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?
                .Lines.FirstOrDefault(x => x.Type == client.Partner.CurrentType)?.Stages;

            var evoInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?
                .Lines.FirstOrDefault(x => x.Type == client.Partner.CurrentType);

            var targetInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?
                .Lines.FirstOrDefault(x => x.Type == evoLine[evoStage].Type);

            if (evoLine == null || !evoLine.Any())
            {
                _logger.Error($"evoLine not found !! Evolution Failed");
                client.Send(new DigimonEvolutionFailPacket());
                return;
            }

            if (targetInfo == null)
            {
                _logger.Error($"targetInfo not found !! Evolution Failed");
                client.Send(new DigimonEvolutionFailPacket());
                return;
            }

            //_logger.Information($"evoStage Index: {evoStage} | evoStage Type: {evoLine[evoStage].Type}");
            //_logger.Information($"Evoline ID: {evoInfo.Id} | Digimon Type Atual: {evoInfo.Type}");

            var starterPartners = new List<int>() { 31001, 31002, 31003, 31004 };

            if (!client.Partner.BaseType.IsBetween(starterPartners.ToArray()))
            {
                var targetEvo = client.Partner.Evolutions.FirstOrDefault(x => x.Type == evoLine[evoStage].Type);

                if (targetEvo == null || targetEvo.Unlocked == 0)
                {
                    _logger.Verbose(
                        $"Tamer {client.Tamer.Name} tryied to evolve {client.Partner.Id}:{client.Partner.BaseInfo.Name} into type {targetEvo?.Type} without unlocking the evo.");
                    client.Send(new DigimonEvolutionFailPacket());
                    return;
                }
            }
            else
            {
                var targetEvo = client.Partner.Evolutions.FirstOrDefault(x => x.Type == evoLine[evoStage].Type);

                if (targetInfo.SlotLevel > 4 && targetEvo.Unlocked == 0)
                {
                    _logger.Verbose(
                        $"Tamer {client.Tamer.Name} tryied to evolve {client.Partner.Id}:{client.Partner.BaseInfo.Name} into type {targetEvo?.Type} without unlocking the evo.");
                    client.Send(new DigimonEvolutionFailPacket());
                    return;
                }
            }

            // -- BUFF --------------------------------

            var buffToRemove = client.Tamer.Partner.BuffList.TamerBaseSkill();

            if (buffToRemove != null)
            {
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RemoveBuffPacket(client.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RemoveBuffPacket(client.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RemoveBuffPacket(client.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RemoveBuffPacket(client.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                        break;
                }
            }

            client.Tamer.RemovePartnerPassiveBuff();

            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));

            // ---------------------------------------

            DigimonEvolutionEffectEnum evoEffect;

            if (evoStage == 8)
            {
                evoEffect = DigimonEvolutionEffectEnum.Back;

                client.Tamer.ActiveEvolution.SetDs(0);
                client.Tamer.ActiveEvolution.SetXg(0);
            }
            else
            {
                var evolutionType = _assets.DigimonBaseInfo.First(x => x.Type == evoLine[evoStage].Type).EvolutionType;

                //_logger.Information($"EvolutionRankEnum: {(EvolutionRankEnum)evolutionType}");

                switch ((EvolutionRankEnum)evolutionType)
                {
                    case EvolutionRankEnum.Rookie:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;
                        client.Tamer.ActiveEvolution.SetDs(0);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Champion:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(20))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(8);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Ultimate:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(50))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(10);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Mega:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(152))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(12);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.BurstMode:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.BurstMode;

                        //_logger.Information($"evoInfo.RequiredItem: {targetInfo.RequiredItem}");

                        if (targetInfo.RequiredItem > 0)
                        {
                            var itemToConsume = client.Tamer.Inventory.FindItemById(41002);

                            if (itemToConsume == null)
                            {
                                itemToConsume = client.Tamer.Inventory.FindItemById(9400);

                                if (itemToConsume == null)
                                {
                                    _logger.Verbose($"Accelerator not found");
                                    client.Send(new DigimonEvolutionFailPacket());
                                    return;
                                }
                            }
                            else
                            {
                                if (client.Partner.Level < targetInfo.UnlockLevel && !client.Tamer.ConsumeDs(148) &&
                                    itemToConsume.Amount < targetInfo.RequiredAmount)
                                {
                                    client.Send(new DigimonEvolutionFailPacket());
                                    return;
                                }
                                else
                                {
                                    client.Tamer.Inventory.RemoveOrReduceItem(itemToConsume, targetInfo.RequiredAmount);
                                    _logger.Verbose(
                                        $"{targetInfo.RequiredAmount} {itemToConsume.ItemInfo.Name} was consumed !!");
                                }
                            }
                        }
                        else
                        {
                            if (client.Partner.Level < targetInfo.UnlockLevel && !client.Tamer.ConsumeDs(148))
                            {
                                client.Send(new DigimonEvolutionFailPacket());
                                return;
                            }
                        }

                        client.Tamer.ActiveEvolution.SetDs(40);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Jogress:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        //_logger.Information($"evoInfo.RequiredItem: {targetInfo.RequiredItem}");

                        if (targetInfo.RequiredItem > 0)
                        {
                            var itemToConsume = client.Tamer.Inventory.FindItemBySection(targetInfo.RequiredItem);

                            if (itemToConsume == null)
                            {
                                itemToConsume = client.Tamer.Inventory.FindItemById(targetInfo.RequiredItem);

                                if (itemToConsume == null)
                                {
                                    _logger.Verbose(
                                        $"Item {targetInfo.RequiredItem} not found on Section and ItemId !!");
                                    client.Send(new DigimonEvolutionFailPacket());
                                    return;
                                }
                            }

                            if (client.Partner.Level < targetInfo.UnlockLevel && !client.Tamer.ConsumeDs(180) &&
                                !client.Tamer.Inventory.RemoveOrReduceItem(itemToConsume, targetInfo.RequiredAmount))
                            {
                                client.Send(new DigimonEvolutionFailPacket());
                                return;
                            }
                        }
                        else
                        {
                            if (client.Partner.Level < targetInfo.UnlockLevel && !client.Tamer.ConsumeDs(180))
                            {
                                client.Send(new DigimonEvolutionFailPacket());
                                return;
                            }
                        }

                        client.Tamer.ActiveEvolution.SetDs(80);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Capsule:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Unknown;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(75))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(3);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.Spirit:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(0))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(20);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    case EvolutionRankEnum.RookieX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(68);

                        client.Tamer.ActiveEvolution.SetXg(2);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.ChampionX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(92);

                        client.Tamer.ActiveEvolution.SetXg(4);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.UltimateX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(130);

                        client.Tamer.ActiveEvolution.SetXg(6);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.MegaX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(174);

                        client.Tamer.ActiveEvolution.SetXg(8);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.BurstModeX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.BurstMode;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(280);

                        client.Tamer.ActiveEvolution.SetXg(10);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.JogressX:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.BurstMode;

                        if (client.Partner.Level < targetInfo.UnlockLevel)
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ConsumeXg(320);

                        client.Tamer.ActiveEvolution.SetXg(12);
                        client.Tamer.ActiveEvolution.SetDs(0);
                    }
                        break;

                    case EvolutionRankEnum.Extra:
                    {
                        evoEffect = DigimonEvolutionEffectEnum.Default;

                        if (client.Partner.Level < targetInfo.UnlockLevel || !client.Tamer.ConsumeDs(0))
                        {
                            client.Send(new DigimonEvolutionFailPacket());
                            return;
                        }

                        client.Tamer.ActiveEvolution.SetDs(20);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                        break;

                    default:
                    {
                        // _logger.Error($"EvolutionRankEnum not registered: {(EvolutionRankEnum)evolutionType}");
                        client.Send(new DigimonEvolutionFailPacket());
                        return;
                    }
                }

                if (client.Tamer.HasXai)
                {
                    client.Send(new XaiInfoPacket(client.Tamer.Xai));
                    client.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));
                }
            }

            // _logger.Information($"Evo ds set...");
            if (evoStage == 8)
                _logger.Verbose(
                    $"Tamer {client.Tamer.Name} devolved partner ({client.Partner.Id}:{client.Partner.Name}) " +
                    $"from {client.Partner.CurrentType} to {evoLine[evoStage]?.Type}.");
            else
                _logger.Verbose(
                    $"Tamer {client.Tamer.Name} evolved partner ({client.Partner.Id}:{client.Partner.Name}) " +
                    $"from {client.Partner.CurrentType}:{client.Partner.BaseInfo.Name} to {evoLine[evoStage]?.Type}");

            // _logger.Information($"update current type...");
            client.Partner.UpdateCurrentType(evoLine[evoStage].Type);

            // ------------------------------------------------------------

            // _logger.Information($"Check if dungeon");
            if (client.Tamer.Riding)
            {
                client.Tamer.StopRideMode();
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UpdateMovementSpeedPacket(client.Tamer).Serialize());

                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler)
                                .Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UpdateMovementSpeedPacket(client.Tamer).Serialize());

                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler)
                                .Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UpdateMovementSpeedPacket(client.Tamer).Serialize());

                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler)
                                .Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new UpdateMovementSpeedPacket(client.Tamer).Serialize());

                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                            new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler)
                                .Serialize());
                        break;
                }
            }

            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new DigimonEvolutionSucessPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler,
                            client.Partner.CurrentType, evoEffect).Serialize());
                    break;

                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new DigimonEvolutionSucessPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler,
                            client.Partner.CurrentType, evoEffect).Serialize());
                    break;

                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new DigimonEvolutionSucessPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler,
                            client.Partner.CurrentType, evoEffect).Serialize());
                    break;

                default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                        new DigimonEvolutionSucessPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler,
                            client.Partner.CurrentType, evoEffect).Serialize());
                    break;
            }

            UpdateSkillCooldown(client);

            var currentHp = client.Partner.CurrentHp;
            var currentMaxHp = client.Partner.HP;
            var currentDs = client.Partner.CurrentDs;
            var currentMaxDs = client.Partner.DS;

            client.Tamer.Partner.SetBaseInfo(_statusManager.GetDigimonBaseInfo(client.Tamer.Partner.CurrentType));
            client.Tamer.Partner.SetBaseStatus(_statusManager.GetDigimonBaseStatus(client.Tamer.Partner.CurrentType,
                client.Tamer.Partner.Level, client.Tamer.Partner.Size));

            client.Partner.SetSealStatus(_assets.SealInfo);
            client.Tamer.SetPartnerPassiveBuff();

            if (evoStage != 8)
                client.Partner.FullHeal();
            else
                client.Partner.AdjustHpAndDs(currentHp, currentMaxHp, currentDs, currentMaxDs);

            var currentTitleBuff =
                _assets.AchievementAssets.FirstOrDefault(x => x.QuestId == client.Tamer.CurrentTitle && x.BuffId > 0);

            if (currentTitleBuff != null)
            {
                foreach (var buff in client.Tamer.Partner.BuffList.ActiveBuffs.Where(x =>
                             x.BuffId != currentTitleBuff.BuffId))
                    buff.SetBuffInfo(_assets.BuffInfo.FirstOrDefault(x =>
                        x.SkillCode == buff.SkillId && buff.BuffInfo == null ||
                        x.DigimonSkillCode == buff.SkillId && buff.BuffInfo == null));

                if (client.Tamer.Partner.BuffList.TamerBaseSkill() != null)
                {
                    var buffToApply = client.Tamer.Partner.BuffList.Buffs
                        .Where(x => x.Duration == 0 && x.BuffId != currentTitleBuff.BuffId).ToList();

                    buffToApply.ForEach(digimonBuffModel =>
                    {
                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;
                        }
                    });
                }
            }
            else
            {
                foreach (var buff in client.Tamer.Partner.BuffList.ActiveBuffs)
                    buff.SetBuffInfo(_assets.BuffInfo.FirstOrDefault(x =>
                        x.SkillCode == buff.SkillId && buff.BuffInfo == null ||
                        x.DigimonSkillCode == buff.SkillId && buff.BuffInfo == null));

                if (client.Tamer.Partner.BuffList.TamerBaseSkill() != null)
                {
                    var buffToApply = client.Tamer.Partner.BuffList.Buffs.Where(x => x.Duration == 0).ToList();

                    buffToApply.ForEach(digimonBuffModel =>
                    {
                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                        digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                                break;
                        }
                    });
                }
            }

            // _logger.Information($"Evolved");
            client.Send(new UpdateStatusPacket(client.Tamer));
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            // -- PARTY -------------------------------------------

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                party.UpdateMember(party[client.TamerId], client.Tamer);

                foreach (var target in party.Members.Values)
                {
                    var targetClient = _mapServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);
                    if (targetClient == null) targetClient = _eventServer.FindClientByTamerId(target.Id);
                    if (targetClient == null) targetClient = _pvpServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) continue;

                    if (target.Id != client.Tamer.Id)
                        targetClient.Send(new PartyMemberInfoPacket(party[client.TamerId]));
                }
            }

            // _logger.Information($"Updating statuses");
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdatePartnerCurrentTypeCommand(client.Partner));
            await _sender.Send(new UpdateCharacterActiveEvolutionCommand(client.Tamer.ActiveEvolution));
            await _sender.Send(new UpdateCharacterBasicInfoCommand(client.Tamer));
            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
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
    }
}