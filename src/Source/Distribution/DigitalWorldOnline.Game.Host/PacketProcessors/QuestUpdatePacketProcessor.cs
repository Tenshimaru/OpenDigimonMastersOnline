using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class QuestUpdatePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.QuestUpdate;

        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public QuestUpdatePacketProcessor(
            AssetsLoader assets,
            ILogger logger,
            ISender sender)
        {
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var questId = packet.ReadShort();
            var goalIndex = packet.ReadByte();

            var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questId);
            if (questInfo == null)
            {
                _logger.Error($"Unknown quest id {questId}.");
                client.Send(new SystemMessagePacket($"Unknown quest id {questId}."));
                return;
            }

            var currentGoalValue = client.Tamer.Progress.GetQuestGoalProgress(questId, goalIndex);
            currentGoalValue++;

            var questToUpdate = client.Tamer.Progress.InProgressQuestData.FirstOrDefault(x => x.QuestId == questId);
            if(questToUpdate == null)
            {
                return;
            }

            client.Tamer.Progress.UpdateQuestInProgress(questId, goalIndex, currentGoalValue);

            client.Send(new QuestGoalUpdatePacket(questId, goalIndex, currentGoalValue));

            var targetGoal = questInfo.QuestGoals[goalIndex];

            switch (targetGoal.GoalType)
            {
                case QuestGoalTypeEnum.ReachRegion:
                case QuestGoalTypeEnum.TalkToNpc:
                    break;

                case QuestGoalTypeEnum.UseItem:
                case QuestGoalTypeEnum.UseItemInNpc:
                case QuestGoalTypeEnum.UseItemAtRegion:
                case QuestGoalTypeEnum.UseItemInMonster:
                    {
                        var itemToRemove = new ItemModel();
                        itemToRemove.SetItemId(targetGoal.GoalId);

                        client.Tamer.Inventory.RemoveOrReduceItem(itemToRemove, 1);

                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    }
                    break;

                case QuestGoalTypeEnum.ClientAction:
                case QuestGoalTypeEnum.ReachLevel:
                case QuestGoalTypeEnum.AcquirePartner:
                    {
                        _logger.Error($"Quest {questId} goal {goalIndex} not implemented.");
                        client.Send(new SystemMessagePacket($"Quest {questId} goal {goalIndex} not implemented."));
                        return;
                    }
            }

            _logger.Verbose($"Character {client.TamerId} updated quest {questId} goal {goalIndex}.");

            await _sender.Send(new UpdateCharacterInProgressCommand(questToUpdate));
        }
    }
}
