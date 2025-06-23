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
    public class ArenaRankingAllRequestInfoPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ArenaRankingAllRequestInfo;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ArenaRankingAllRequestInfoPacketProcessor(
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
            var rankingWeeklyInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Weekly)));
            var rankingMonthlyInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Monthly)));
            var rankingSeasonalInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Seasonal)));
            
            if (rankingWeeklyInfo != null)
            {
                foreach (var targetTamer in rankingWeeklyInfo.Competitors)
                {
                    var targetInfo = await _sender.Send(new GetCharacterNameAndGuildByIdQuery(targetTamer.TamerId));

                    targetTamer.SetTamerAndGuildName(targetInfo.TamerName, targetInfo.GuildName);
                }

                client.Send(new ArenaRankingInfoPacket((int)client.TamerId,rankingWeeklyInfo, ArenaRankingEnum.Weekly, ArenaRankingStatusEnum.Open, ArenaRankingPositionTypeEnum.Absolut));
                
            }

            if(rankingMonthlyInfo != null)
            {
                foreach (var targetTamer in rankingMonthlyInfo.Competitors)
                {
                    var targetInfo = await _sender.Send(new GetCharacterNameAndGuildByIdQuery(targetTamer.TamerId));

                    targetTamer.SetTamerAndGuildName(targetInfo.TamerName, targetInfo.GuildName);
                }

                client.Send(new ArenaRankingInfoPacket((int)client.TamerId, rankingMonthlyInfo, ArenaRankingEnum.Monthly, ArenaRankingStatusEnum.Open, ArenaRankingPositionTypeEnum.Absolut));
               
            }

            if (rankingSeasonalInfo != null)
            {
                foreach (var targetTamer in rankingSeasonalInfo.Competitors)
                {
                    var targetInfo = await _sender.Send(new GetCharacterNameAndGuildByIdQuery(targetTamer.TamerId));

                    targetTamer.SetTamerAndGuildName(targetInfo.TamerName, targetInfo.GuildName);
                }

                client.Send(new ArenaRankingInfoPacket((int)client.TamerId, rankingSeasonalInfo, ArenaRankingEnum.Seasonal, ArenaRankingStatusEnum.Open, ArenaRankingPositionTypeEnum.Absolut));
                
            }
        }
    }
}