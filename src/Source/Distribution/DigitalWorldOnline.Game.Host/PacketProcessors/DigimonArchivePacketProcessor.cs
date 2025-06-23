using AutoMapper;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigimonArchivePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DigimonArchive;

        private readonly StatusManager _statusManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public DigimonArchivePacketProcessor(
            StatusManager statusManager,
            IMapper mapper,
            ILogger logger,
            ISender sender)
        {
            _statusManager = statusManager;
            _mapper = mapper;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            
            var isVip = packet.ReadByte();
            var nInvenIdx = packet.ReadInt();
            var nNpcId = packet.ReadInt();
            var nNpcType = packet.ReadByte();

            _logger.Debug($"isVip: {isVip} | nInvenIdx: {nInvenIdx} | nNpcId: {nNpcId} | nNpcType: {nNpcType}");

            foreach (var digimonArchive in client.Tamer.DigimonArchive.DigimonArchives.Where(x => x.DigimonId > 0))
            {
                digimonArchive.SetDigimonInfo(_mapper.Map<DigimonModel>(
                    await _sender.Send(
                        new GetDigimonByIdQuery(digimonArchive.DigimonId))
                    )
                );

                digimonArchive.Digimon?.SetBaseInfo(
                    _statusManager.GetDigimonBaseInfo(
                        digimonArchive.Digimon.BaseType
                    )
                );

                digimonArchive.Digimon?.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        digimonArchive.Digimon.BaseType,
                        digimonArchive.Digimon.Level,
                        digimonArchive.Digimon.Size
                    )
                );
            }

            _logger.Debug($"Character {client.TamerId} loaded digimon archive info.");

            client.Send(new DigimonArchiveLoadPacket(client.Tamer.DigimonArchive));
        }
    }
}