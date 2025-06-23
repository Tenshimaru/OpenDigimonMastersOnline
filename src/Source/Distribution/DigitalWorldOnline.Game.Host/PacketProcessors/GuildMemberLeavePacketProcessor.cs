using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class GuildMemberLeavePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.GuildMemberLeave;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public GuildMemberLeavePacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender,
            IMapper mapper)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            _logger.Debug($"Searching guild by character id {client.TamerId}...");
            var targetGuild = _mapper.Map<GuildModel>(await _sender.Send(new GuildByCharacterIdQuery(client.TamerId)));
            if (targetGuild == null)
            {
                _logger.Warning($"Character {client.Tamer.Name} does not belong to a guild.");
                client.Send(new SystemMessagePacket($"Character {client.Tamer.Name} does not belong to a guild."));
                return;
            }

            foreach (var guildMember in targetGuild.Members)
            {
                if (guildMember.CharacterInfo == null)
                {
                    var guildMemberClient = _mapServer.FindClientByTamerId(guildMember.CharacterId);
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

            var targetMember = targetGuild.FindMember(client.TamerId);
            var newEntry =
                targetGuild.AddHistoricEntry(GuildHistoricTypeEnum.MemberLeave, targetGuild.Master, targetMember);

            targetGuild.RemoveMember(client.TamerId);
            client.Tamer.SetGuild();

            foreach (var guildMember in targetGuild.Members)
            {
                _logger.Debug($"Sending guild historic packet for character {guildMember.CharacterId}...");
                _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildHistoricPacket(targetGuild.Historic).Serialize());

                _dungeonServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildHistoricPacket(targetGuild.Historic).Serialize());


                _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildHistoricPacket(targetGuild.Historic).Serialize());

                _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildHistoricPacket(targetGuild.Historic).Serialize());

                _logger.Debug($"Sending guild member quit packet for character {guildMember.CharacterId}...");
                _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildMemberQuitPacket(client.Tamer.Name).Serialize());

                _dungeonServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildMemberQuitPacket(client.Tamer.Name).Serialize());

                _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildMemberQuitPacket(client.Tamer.Name).Serialize());

                _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                    new GuildMemberQuitPacket(client.Tamer.Name).Serialize());
            }

            _logger.Debug($"Saving historic entry for guild {targetGuild.Id}...");
            await _sender.Send(new CreateGuildHistoricEntryCommand(newEntry, targetGuild.Id));

            _logger.Debug($"Removing member {client.TamerId} for guild {targetGuild.Id}...");
            await _sender.Send(new DeleteGuildMemberCommand(client.TamerId, targetGuild.Id));
        }
    }
}