using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class QuestDeliverPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.QuestDeliver;

        private readonly StatusManager _statusManager;
        private readonly ExpManager _expManager;
        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public QuestDeliverPacketProcessor(
            StatusManager statusManager,
            ExpManager expManager,
            AssetsLoader assets,
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender)
        {
            _statusManager = statusManager;
            _expManager = expManager;
            _mapServer = mapServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var questId = packet.ReadShort();

            var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questId);

            if (questInfo == null)
            {
                _logger.Error($"Unknown quest id {questId}.");
                client.Send(new SystemMessagePacket($"Unknown quest id {questId}."));
                client.Tamer.Progress.RemoveQuest(questId);
                return;
            }

            _logger.Verbose($"Character {client.TamerId} delivered quest {questId}.");

            DeliverItems(client, questId, questInfo);
            ReturnSupplies(client, questId, questInfo);
            QuestRewards(client, questInfo);

            var evolutionQuest = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?
                .Lines.FirstOrDefault(y => y.UnlockQuestId == questId && y.UnlockItemSection == 0);

            if (evolutionQuest != null)
            {
                var targetEvolution = client.Tamer.Partner.Evolutions[evolutionQuest.SlotLevel - 1];

                if (targetEvolution != null)
                {
                    targetEvolution.Unlock();

                    await _sender.Send(new UpdateEvolutionCommand(targetEvolution));

                    _logger.Verbose(
                        $"Character {client.TamerId} unlocked evolution {targetEvolution.Type} on quest {questId} completion.");

                    var evoInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?.Lines
                        .FirstOrDefault(x => x.Type == targetEvolution.Type);

                    var encyclopedia =
                        client.Tamer.Encyclopedia.First(x => x.DigimonEvolutionId == evoInfo.EvolutionId);

                    if (encyclopedia != null)
                    {
                        var encyclopediaEvolution =
                            encyclopedia.Evolutions.First(x => x.DigimonBaseType == targetEvolution.Type);

                        encyclopediaEvolution.Unlock();

                        await _sender.Send(new UpdateCharacterEncyclopediaEvolutionsCommand(encyclopediaEvolution));

                        int LockedEncyclopediaCount = encyclopedia.Evolutions.Count(x => x.IsUnlocked == false);

                        if (LockedEncyclopediaCount <= 0)
                        {
                            encyclopedia.SetRewardAllowed();
                            await _sender.Send(new UpdateCharacterEncyclopediaCommand(encyclopedia));
                        }
                    }
                }
            }

            UpdateProgressValue(client, questId);

            var questToUpdate = client.Tamer.Progress.InProgressQuestData.FirstOrDefault(x => x.QuestId == questId);

            var id = client.Tamer.Progress.RemoveQuest(questId);

            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            await _sender.Send(new RemoveActiveQuestCommand(id));
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateCharacterProgressCompleteCommand(client.Tamer.Progress));
        }

        private int GetBitValue(int[] array, int x)
        {
            int arrIDX = x / 32;
            int bitPosition = x % 32;

            if (arrIDX >= array.Length)
            {
                _logger.Error($"Invalid array index.");
                throw new ArgumentOutOfRangeException("Invalid array index");
            }

            int value = array[arrIDX];
            return (value >> bitPosition) & 1;
        }

        private void SetBitValue(int[] array, int x, int bitValue)
        {
            int arrIDX = x / 32;
            int bitPosition = x % 32;

            if (arrIDX >= array.Length)
            {
                _logger.Error($"Invalid array index on set bit value.");
                throw new ArgumentOutOfRangeException("Invalid array index on set bit value.");
            }

            if (bitValue != 0 && bitValue != 1)
            {
                _logger.Error($"Invalid bit value. Only 0 or 1 are allowed.");
                throw new ArgumentException("Invalid bit value. Only 0 or 1 are allowed.");
            }

            int value = array[arrIDX];
            int mask = 1 << bitPosition;

            if (bitValue == 1)
                array[arrIDX] = value | mask;
            else
                array[arrIDX] = value & ~mask;
        }

        private void UpdateQuestComplete(GameClient client, int qIDX)
        {
            int intValue = GetBitValue(client.Tamer.Progress.CompletedDataValue, qIDX - 1);

            if (intValue == 0)
                SetBitValue(client.Tamer.Progress.CompletedDataValue, qIDX - 1, 1);
        }

        private void UpdateProgressValue(GameClient client, short questId)
        {
            UpdateQuestComplete(client, questId);
        }

        private void DeliverItems(GameClient client, short questId, QuestAssetModel questInfo)
        {
            foreach (var questGoal in questInfo.QuestGoals.Where(x => x.GoalType == QuestGoalTypeEnum.LootItem))
            {
                var item = new ItemModel();
                item.SetItemId(questGoal.GoalId);
                item.SetAmount(questGoal.GoalAmount);
                item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));

                if (item.ItemInfo == null)
                {
                    _logger.Error($"Item information not found for item {item.ItemId}.");
                    client.Send(new SystemMessagePacket($"Item information not found for item {item.ItemId}."));
                }
                else
                {
                    _logger.Verbose(
                        $"Character {client.TamerId} delivered quest {questId} goal item {questGoal.GoalId} x{questGoal.GoalAmount}.");
                    client.Tamer.Inventory.RemoveOrReduceItem(item, questGoal.GoalAmount);
                }
            }
        }

        private void ReturnSupplies(GameClient client, short questId, QuestAssetModel questInfo)
        {
            foreach (var questSupply in questInfo.QuestSupplies)
            {
                var item = new ItemModel();
                item.SetItemId(questSupply.ItemId);
                item.SetAmount(questSupply.Amount);
                item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));

                if (item.ItemInfo == null)
                {
                    _logger.Error($"Item information not found for item {item.ItemId}.");
                    client.Send(new SystemMessagePacket($"Item information not found for item {item.ItemId}."));
                }
                else
                {
                    _logger.Verbose(
                        $"Character {client.TamerId} delivered quest {questId} supply item {questSupply.ItemId} x{questSupply.Amount}.");
                    client.Tamer.Inventory.RemoveOrReduceItem(item, questSupply.Amount);
                }
            }
        }

        private void QuestRewards(GameClient client, QuestAssetModel questInfo)
        {
            var questRewards = questInfo.QuestRewards;
            foreach (var questReward in questRewards)
            {
                switch (questReward.RewardType)
                {
                    case QuestRewardTypeEnum.MoneyReward:
                    {
                        QuestMoneyReward(client, questReward);
                    }
                        break;

                    case QuestRewardTypeEnum.ExperienceReward:
                    {
                        QuestExpReward(client, questReward);
                    }
                        break;

                    case QuestRewardTypeEnum.ItemReward:
                    {
                        QuestItemReward(client, questReward);
                    }
                        break;
                }
            }
        }

        private void QuestItemReward(GameClient client, QuestRewardAssetModel questReward)
        {
            questReward.RewardObjectList.ForEach(rewardObject =>
            {
                var newItem = new ItemModel();
                newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == rewardObject.Reward));

                if (newItem.ItemInfo == null)
                {
                    _logger.Warning($"No item info found with ID {rewardObject.Reward} for tamer {client.TamerId}.");
                    client.Send(new SystemMessagePacket($"No item info found with ID {rewardObject.Reward}."));
                    return;
                }

                newItem.SetItemId(rewardObject.Reward);
                newItem.SetAmount(rewardObject.Amount);

                if (newItem.IsTemporary)
                    newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                var itemClone = (ItemModel)newItem.Clone();
                if (!client.Tamer.Inventory.AddItem(itemClone))
                {
                    client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                }
                else
                {
                    _logger.Verbose(
                        $"Character {client.TamerId} received quest {questReward.Quest.QuestId} item {rewardObject.Reward} x{rewardObject.Amount} reward.");
                }
            });
        }

        private void QuestExpReward(GameClient client, QuestRewardAssetModel questReward)
        {
            questReward.RewardObjectList.ForEach(async rewardObject =>
            {
                _logger.Verbose($"Character {client.TamerId} received quest {questReward.Quest.QuestId} exp reward.");

                var tamerExpToReceive = rewardObject.Amount / 10;
                var tamerResult = ReceiveTamerExp(client.Tamer, tamerExpToReceive);

                var partnerExpToReceive = rewardObject.Amount / 5;
                var partnerResult = ReceivePartnerExp(client.Partner, partnerExpToReceive);

                _logger.Verbose($"Tamer {client.TamerId}:{client.Tamer.Name} received TamerExp: {tamerExpToReceive} and PartnerExp: {partnerExpToReceive} from quest {questReward.Quest.QuestId}.");

                client.Send(
                    new ReceiveExpPacket(
                        tamerExpToReceive,
                        0, //TODO: obter os bonus
                        client.Tamer.CurrentExperience,
                        client.Partner.GeneralHandler,
                        partnerExpToReceive,
                        0, //TODO: obter os bonus
                        client.Partner.CurrentExperience,
                        client.Partner.CurrentEvolution.SkillExperience
                    )
                );

                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                if (tamerResult.LevelGain > 0 || partnerResult.LevelGain > 0)
                {
                    client.Send(new UpdateStatusPacket(client.Tamer));
                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                            break;
                    }
                }

                if (tamerResult.Success)
                {
                    await _sender.Send(
                        new UpdateCharacterExperienceCommand(
                            client.TamerId,
                            client.Tamer.CurrentExperience,
                            client.Tamer.Level
                        )
                    );
                }

                if (partnerResult.Success)
                {
                    await _sender.Send(
                        new UpdateDigimonExperienceCommand(
                            client.Partner
                        )
                    );
                }
            });
        }

        private void QuestMoneyReward(GameClient client, QuestRewardAssetModel questReward)
        {
            questReward.RewardObjectList.ForEach(rewardObject =>
            {
                _logger.Verbose(
                    $"Character {client.TamerId} received quest {questReward.Quest.QuestId} {rewardObject.Amount} bits reward.");
                client.Tamer.Inventory.AddBits(rewardObject.Amount);
            });
        }

        private ReceiveExpResult ReceiveTamerExp(CharacterModel tamer, long tamerExpToReceive)
        {
            var tamerResult = _expManager.ReceiveTamerExperience(tamerExpToReceive, tamer);

            if (tamerResult.LevelGain > 0)
            {
                _mapServer.BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler, tamer.Level).Serialize());
                _dungeonsServer.BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler, tamer.Level).Serialize());
                _eventServer.BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler, tamer.Level).Serialize());
                _pvpServer.BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler, tamer.Level).Serialize());

                tamer.SetLevelStatus(
                    _statusManager.GetTamerLevelStatus(
                        tamer.Model,
                        tamer.Level
                    )
                );

                tamer.FullHeal();
            }

            return tamerResult;
        }

        private ReceiveExpResult ReceivePartnerExp(DigimonModel partner, long partnerExpToReceive)
        {
            var partnerResult = _expManager.ReceiveDigimonExperience(partnerExpToReceive, partner);

            if (partnerResult.LevelGain > 0)
            {
                partner.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        partner.CurrentType,
                        partner.Level,
                        partner.Size
                    )
                );

                _mapServer.BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler, partner.Level).Serialize());

                _dungeonsServer.BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler, partner.Level).Serialize());

                _eventServer.BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler, partner.Level).Serialize());

                _pvpServer.BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler, partner.Level).Serialize());

                partner.FullHeal();
            }

            return partnerResult;
        }
    }
}