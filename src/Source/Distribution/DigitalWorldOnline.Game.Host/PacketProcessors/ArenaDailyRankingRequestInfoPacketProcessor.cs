using AutoMapper;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ArenaDailyRankingRequestInfoPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ArenaDailyRankingLoad;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ArenaDailyRankingRequestInfoPacketProcessor(
            ILogger logger,
            ISender sender,
            IMapper mapper)
        {
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var rankingInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Weekly)));
            
            if (rankingInfo == null)
            {
                client.Send(new ArenaRankingDailyLoadPacket(0, 0));
                return;
            }

            var ranking = rankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);
            
            if (ranking != null)
            {

                client.Send(new ArenaRankingDailyLoadPacket(UtilitiesFunctions.CurrentRemainingTimeToResetDay(), client.Tamer.DailyPoints.Points));

            }
            else
            {
                client.Send(new ArenaRankingDailyLoadPacket(UtilitiesFunctions.CurrentRemainingTimeToResetDay(), 0));
            }
        }
    }
}
