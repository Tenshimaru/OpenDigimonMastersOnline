using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using System.Reflection.Metadata.Ecma335;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ArenaDailyInsertPointsPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ArenaDailyInsertPoints;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly AssetsLoader _assets;

        public ArenaDailyInsertPointsPacketProcessor(
            ILogger logger,
            ISender sender,
            IMapper mapper,
            AssetsLoader assets)
        {
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _assets = assets;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {

            var packet = new GamePacketReader(packetData);
            _ = packet.ReadShort();
            short itemSlot = packet.ReadShort();
            short Points = packet.ReadShort(); //Quantidade
            short ItemId = packet.ReadShort(); // ItemId
            short Result = packet.ReadShort(); // Insert Result

            // Weekly Ranking
            var weeklyRankingInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Weekly)));

            if (weeklyRankingInfo == null)
            {
                return;
            }

            var weeklyRanking = weeklyRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);

            if (weeklyRanking == null)
            {
                weeklyRankingInfo.JoinRanking(client.TamerId, Points);
                weeklyRanking = weeklyRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);

                if (weeklyRanking != null)
                {
                    client.Tamer.AddPoints(Points);
                  
                    var todayRewards = _assets.ArenaRankingDailyItemRewards.FirstOrDefault(x => x.WeekDay == DateTime.Now.DayOfWeek);

                    var rewardsToReceive = todayRewards.GetRewards(0, client.Tamer.DailyPoints.Points);

                    var removeItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);

                    client.Tamer.Inventory.RemoveOrReduceItem(removeItem, Points);

                    foreach (var item in rewardsToReceive)
                    {
                        var targetItem = new ItemModel();
                        targetItem.ItemId = item.ItemId;
                        targetItem.Amount = item.Amount;
                        targetItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault( x=> x.ItemId == item.ItemId));

                        if(!client.Tamer.Inventory.AddItem(targetItem))
                        {
                            client.Tamer.GiftWarehouse.AddItem(targetItem);
                            await _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                        }
                      
                    }

                    await _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                 
                    client.Send(new ArenaRankingDailyUpdatePointsPacket(client.Tamer.DailyPoints.Points));
                    client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
                }
            }
            else
            {
                weeklyRanking.AddPoints(Points);

                var previousPoints = client.Tamer.DailyPoints.Points;

                var currentPoints = client.Tamer.DailyPoints.Points + Points;

                client.Tamer.AddPoints(Points);

                var todayRewards = _assets.ArenaRankingDailyItemRewards.FirstOrDefault(x => x.WeekDay == DateTime.Now.DayOfWeek);

                var rewardsToReceive = todayRewards.GetRewards(previousPoints, currentPoints);

                var removeItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);
                client.Tamer.Inventory.RemoveOrReduceItem(removeItem, Points);

                foreach (var item in rewardsToReceive)
                {
                    var targetItem = new ItemModel();
                    targetItem.ItemId = item.ItemId;
                    targetItem.Amount = item.Amount;
                    targetItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));

                    if (!client.Tamer.Inventory.AddItem(targetItem))
                    {
                        client.Tamer.GiftWarehouse.AddItem(targetItem);
                        await _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                    }
                    
                 
                }

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

                client.Send(new ArenaRankingDailyUpdatePointsPacket(client.Tamer.DailyPoints.Points));
                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            }

            await _sender.Send(new UpdateArenaRankingCommand(weeklyRankingInfo));
            await _sender.Send(new UpdateCharacterArenaDailyPointsCommand(client.Tamer.DailyPoints));

            // Monthly Ranking
            var monthlyRankingInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Monthly)));

            if (monthlyRankingInfo == null)
            {
                return;
            }

            var monthlyRanking = monthlyRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);

            if (monthlyRanking == null)
            {
                monthlyRankingInfo.JoinRanking(client.TamerId, Points);
                monthlyRanking = monthlyRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);
            
            }
            else
            {
                monthlyRanking.AddPoints(Points);
            }

            await _sender.Send(new UpdateArenaRankingCommand(monthlyRankingInfo));
         

            // Seasonal Ranking
            var seasonalRankingInfo = _mapper.Map<ArenaRankingModel>(await _sender.Send(new GetArenaRankingQuery(ArenaRankingEnum.Seasonal)));

            if (seasonalRankingInfo == null)
            {
                return;
            }

            var seasonalRanking = seasonalRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);

            if (seasonalRanking == null)
            {
                seasonalRankingInfo.JoinRanking(client.TamerId, Points);
                seasonalRanking = seasonalRankingInfo.Competitors.FirstOrDefault(x => x.TamerId == client.TamerId);              
            }
            else
            {
                seasonalRanking.AddPoints(Points);
            }

        }

    }
}
