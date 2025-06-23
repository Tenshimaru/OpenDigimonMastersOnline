using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartnerSwitchPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerSwitch;

        private readonly PartyManager _partyManager;
        private readonly StatusManager _statusManager;
        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public PartnerSwitchPacketProcessor(PartyManager partyManager, StatusManager statusManager, AssetsLoader assets,
            MapServer mapServer, DungeonsServer dungeonServer, EventServer eventServer, PvpServer pvpServer,
            ILogger logger, ISender sender)
        {
            _partyManager = partyManager;
            _statusManager = statusManager;
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var slot = packet.ReadByte();

            var previousId = client.Partner.Id;
            var previousType = client.Partner.CurrentType;

            var newPartner = client.Tamer.Digimons.First(x => x.Slot == slot);

            client.Tamer.RemovePartnerPassiveBuff();
            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapConfig!.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonServer.SwapDigimonHandlers(client.Tamer.Location.MapId, client.Tamer.Channel,
                        client.Partner, newPartner);
                    break;

                case MapTypeEnum.Event:
                    _eventServer.SwapDigimonHandlers(client.Tamer.Location.MapId, client.Tamer.Channel, client.Partner,
                        newPartner);
                    break;

                case MapTypeEnum.Pvp:
                    _pvpServer.SwapDigimonHandlers(client.Tamer.Location.MapId, client.Tamer.Channel, client.Partner,
                        newPartner);
                    break;

                default:
                    _mapServer.SwapDigimonHandlers(client.Tamer.Location.MapId, client.Tamer.Channel, client.Partner,
                        newPartner);
                    break;
            }

            client.Tamer.SwitchPartner(slot);

            client.Partner.UpdateCurrentType(client.Partner.BaseType);
            client.Partner.SetTamer(client.Tamer);
            client.Partner.NewLocation(client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y);

            client.Partner.SetBaseInfo(_statusManager.GetDigimonBaseInfo(client.Tamer.Partner.CurrentType));
            client.Partner.SetBaseStatus(_statusManager.GetDigimonBaseStatus(client.Tamer.Partner.CurrentType,
                client.Tamer.Partner.Level, client.Tamer.Partner.Size));
            client.Partner.SetSealStatus(_assets.SealInfo);

            // Battle Tag
            if (client.Tamer.InBattle)
            {
                var battleTagItem = client.Tamer.Inventory.FindItemBySection(16400);

                if (client.Tamer.Inventory.RemoveOrReduceItem(battleTagItem, 1))
                {
                    _logger.Verbose($"Character {client.TamerId} consumed battle tag {battleTagItem.ItemId}.");
                    client.Send(new PartnerSwitchInBattlePacket(slot, client.Tamer.Model.GetHashCode()));
                }
            }

            client.Tamer.SetPartnerPassiveBuff();

            foreach (var buff in client.Tamer.Partner.BuffList.ActiveBuffs)
                buff.SetBuffInfo(_assets.BuffInfo.FirstOrDefault(x =>
                    x.SkillCode == buff.SkillId && buff.BuffInfo == null ||
                    x.DigimonSkillCode == buff.SkillId && buff.BuffInfo == null));

            switch (mapConfig.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonServer.BroadcastForTamerViewsAndSelf(client,
                        new PartnerSwitchPacket(client.Tamer.GenericHandler, previousType, client.Partner, slot)
                            .Serialize());
                    break;
                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client,
                        new PartnerSwitchPacket(client.Tamer.GenericHandler, previousType, client.Partner, slot)
                            .Serialize());
                    break;
                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client,
                        new PartnerSwitchPacket(client.Tamer.GenericHandler, previousType, client.Partner, slot)
                            .Serialize());
                    break;
                case MapTypeEnum.Default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client,
                        new PartnerSwitchPacket(client.Tamer.GenericHandler, previousType, client.Partner, slot)
                            .Serialize());
                    break;
            }

            if (client.Tamer.Partner.BuffList.Buffs.Any())
            {
                var buffToApply = client.Tamer.Partner.BuffList.Buffs;


                buffToApply.ForEach(digimonBuffModel =>
                {
                    var Ts = 0;

                    if (digimonBuffModel.Duration != 0)
                        Ts = UtilitiesFunctions.RemainingTimeSeconds(digimonBuffModel.RemainingSeconds);

                    if (digimonBuffModel.BuffInfo != null)
                    {
                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffInfo,
                                        (short)digimonBuffModel.TypeN, Ts).Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffInfo,
                                        (short)digimonBuffModel.TypeN, Ts).Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffInfo,
                                        (short)digimonBuffModel.TypeN, Ts).Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                    new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffInfo,
                                        (short)digimonBuffModel.TypeN, Ts).Serialize());
                                break;
                        }
                    }
                });
            }

            client.Send(new UpdateStatusPacket(client.Tamer));

            if (client.Tamer.HasXai)
            {
                client.Send(new XaiInfoPacket(client.Tamer.Xai));
                client.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));
            }

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                party.UpdateMember(party[client.TamerId], client.Tamer);

                foreach (var target in party.Members.Values)
                {
                    var targetClient = _mapServer.FindClientByTamerId(target.Id) ?? _dungeonServer.FindClientByTamerId(target.Id);
                    targetClient ??= _eventServer.FindClientByTamerId(target.Id);
                    targetClient ??= _pvpServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) continue;

                    if (target.Id != client.Tamer.Id)
                        targetClient.Send(new PartyMemberPartnerSwitchPacket(party[client.TamerId]).Serialize());
                }
            }

            client.Tamer.ActiveEvolution.SetDs(0);
            client.Tamer.ActiveEvolution.SetXg(0);

            await _sender.Send(new UpdatePartnerCurrentTypeCommand(client.Partner));
            await _sender.Send(new UpdateCharacterDigimonsOrderCommand(client.Tamer));
            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
            await _sender.Send(new UpdateCharacterActiveEvolutionCommand(client.Tamer.ActiveEvolution));

            _logger.Debug($"Tamer {client.Tamer.Name} switched partner {previousType} with {client.Partner.BaseType}.");
        }
    }
}