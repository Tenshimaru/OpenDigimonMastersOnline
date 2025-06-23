using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class GuildInviteAcceptPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.GuildInviteAccept;

        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public GuildInviteAcceptPacketProcessor(
            MapServer mapServer,
            EventServer eventServer,
            DungeonsServer dungeonsServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender,
            IMapper mapper)
        {
            _mapServer = mapServer;
            _eventServer = eventServer;
            _dungeonsServer = dungeonsServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var guildId = packet.ReadInt();
            var inviterName = packet.ReadString();

            var inviterCharacter = _mapServer.FindClientByTamerName(inviterName);

            if (inviterCharacter == null)
            {
                _logger.Warning($"Character not found with name {inviterName}.");
                client.Send(new SystemMessagePacket($"Character not found with name {inviterName}."));
                return;
            }

            var targetGuild = inviterCharacter.Tamer.Guild;

            if (targetGuild == null)
            {
                _logger.Warning($"Guild not found with id {guildId}.");
                client.Send(new SystemMessagePacket($"Guild not found with id {guildId}."));
                return;
            }

            _logger.Verbose(
                $"Character {client.TamerId} joinned guild {targetGuild.Id} {targetGuild.Name} through character {inviterCharacter.TamerId} invite.");

            var newMember = targetGuild.AddMember(client.Tamer);
            var senderMember = targetGuild.FindMember(inviterCharacter.TamerId);
            senderMember?.SetCharacterInfo(inviterCharacter.Tamer);

            var newEntry = targetGuild.AddHistoricEntry(GuildHistoricTypeEnum.MemberJoin, senderMember, newMember);

            client.Tamer.SetGuild(targetGuild);

            foreach (var guildMember in targetGuild.Members)
            {
                if (guildMember.CharacterInfo == null)
                {
                    var guildMemberClient = _mapServer.FindClientByTamerId(guildMember.CharacterId);
                    guildMember.SetCharacterInfo(guildMemberClient!.Tamer);
                }
            }

            _logger.Debug($"Sending guild information packet for character {client.TamerId}...");
            client.Send(new GuildInformationPacket(targetGuild));

            _mapServer.BroadcastForTargetTamers(client.TamerId, UtilitiesFunctions.GroupPackets(
                new UnloadTamerPacket(client.Tamer).Serialize(),
                new LoadTamerPacket(client.Tamer).Serialize(),
                new LoadBuffsPacket(client.Tamer).Serialize()
            ));

            _dungeonsServer.BroadcastForTargetTamers(client.TamerId, UtilitiesFunctions.GroupPackets(
                new UnloadTamerPacket(client.Tamer).Serialize(),
                new LoadTamerPacket(client.Tamer).Serialize(),
                new LoadBuffsPacket(client.Tamer).Serialize()
            ));

            _eventServer.BroadcastForTargetTamers(client.TamerId, UtilitiesFunctions.GroupPackets(
                new UnloadTamerPacket(client.Tamer).Serialize(),
                new LoadTamerPacket(client.Tamer).Serialize(),
                new LoadBuffsPacket(client.Tamer).Serialize()
            ));

            _pvpServer.BroadcastForTargetTamers(client.TamerId, UtilitiesFunctions.GroupPackets(
                new UnloadTamerPacket(client.Tamer).Serialize(),
                new LoadTamerPacket(client.Tamer).Serialize(),
                new LoadBuffsPacket(client.Tamer).Serialize()
            ));

            foreach (var guildMember in targetGuild.Members)
            {
                //_logger.Debug($"Sending guild member connection packet for character {guildMember.CharacterId}...");
                //_mapServer.BroadcastForUniqueTamer(guildMember.CharacterId, new GuildMemberConnectPacket(client.Tamer).Finalize());

                _logger.Debug($"Sending guild information packet for character {client.TamerId}...");

                _logger.Debug($"Sending guild historic packet for character {guildMember.CharacterId}...");
                _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId, UtilitiesFunctions.GroupPackets(
                    new GuildInformationPacket(client.Tamer.Guild).Serialize(),
                    new GuildHistoricPacket(targetGuild.Historic).Serialize()
                ));
                _dungeonsServer.BroadcastForUniqueTamer(guildMember.CharacterId, UtilitiesFunctions.GroupPackets(
                    new GuildInformationPacket(client.Tamer.Guild).Serialize(),
                    new GuildHistoricPacket(targetGuild.Historic).Serialize()
                ));
                _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId, UtilitiesFunctions.GroupPackets(
                    new GuildInformationPacket(client.Tamer.Guild).Serialize(),
                    new GuildHistoricPacket(targetGuild.Historic).Serialize()
                ));
                _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId, UtilitiesFunctions.GroupPackets(
                    new GuildInformationPacket(client.Tamer.Guild).Serialize(),
                    new GuildHistoricPacket(targetGuild.Historic).Serialize()
                ));
            }

            _logger.Debug($"Getting guild rank position for guild {targetGuild.Id}...");
            var guildRank = await _sender.Send(new GuildCurrentRankByGuildIdQuery(guildId));
            if (guildRank > 0 && guildRank <= 100)
            {
                _logger.Debug($"Sending guild rank packet for character {client.TamerId}...");
                client.Send(new GuildRankPacket(guildRank));
            }

            _logger.Debug($"Saving historic entry for guild {guildId}...");
            await _sender.Send(new CreateGuildHistoricEntryCommand(newEntry, guildId));

            _logger.Debug($"Saving new member for guild {guildId}...");
            await _sender.Send(new CreateGuildMemberCommand(newMember, guildId));
        }
    }
}