using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Map;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using System.Diagnostics;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class PvpServer
    {
        private void MonsterOperation(GameMap map)
        {
            if (!map.ConnectedTamers.Any())
                return;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            map.UpdateMapMobs();

            foreach (var mob in map.Mobs)
            {
                if (!mob.AwaitingKillSpawn && DateTime.Now > mob.ViewCheckTime)
                {
                    mob.SetViewCheckTime();

                    mob.TamersViewing.RemoveAll(x => !map.ConnectedTamers.Select(y => y.Id).Contains(x));

                    var nearTamers = map.NearestTamers(mob.Id);

                    if (!nearTamers.Any() && !mob.TamersViewing.Any())
                        continue;

                    if (!mob.Dead)
                    {
                        nearTamers.ForEach(nearTamer =>
                        {
                            if (!mob.TamersViewing.Contains(nearTamer))
                            {
                                mob.TamersViewing.Add(nearTamer);

                                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == nearTamer);

                                targetClient?.Send(new LoadMobsPacket(mob));
                                targetClient?.Send(new LoadBuffsPacket(mob));
                            }
                        });
                    }

                    var farTamers = map.ConnectedTamers.Select(x => x.Id).Except(nearTamers).ToList();

                    farTamers.ForEach(farTamer =>
                    {
                        if (mob.TamersViewing.Contains(farTamer))
                        {
                            var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == farTamer);

                            mob.TamersViewing.Remove(farTamer);
                            targetClient?.Send(new UnloadMobsPacket(mob));
                        }
                    });
                }

                if (!mob.CanAct)
                    continue;

                MobsOperation(map, mob);

                mob.SetNextAction();
            }

            //map.UpdateMapMobs(true);

            stopwatch.Stop();

            var totalTime = stopwatch.Elapsed.TotalMilliseconds;

            if (totalTime >= 1000)
                Console.WriteLine($"Pvp MonstersOperation ({map.Mobs.Count}): {totalTime}.");
        }

        private void MobsOperation(GameMap map, MobConfigModel mob)
        {
            switch (mob.CurrentAction)
            {
                case MobActionEnum.CrowdControl:
                    {
                        var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                            buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

                        if (debuff.Any())
                        {
                            CheckDebuff(map, mob, debuff);
                            break;
                        }
                    }
                    break;

                case MobActionEnum.Respawn:
                    {
                        mob.Reset();
                        mob.ResetLocation();
                    }
                    break;

                case MobActionEnum.Reward:
                    {
                        if (mob.Class == 8)
                        {
                            mob.UpdateDeathAndResurrectionTime();
                            //SaveMobToDatabase(mob);
                        }

                        if (mob.RespawnInterval > 3599)
                        {
                            TimeSpan time = TimeSpan.FromSeconds(mob.RespawnInterval);
                            DateTime currentTime = DateTime.Now;
                            DateTime newTime = currentTime.Add(time);
                            string formattedTime = newTime.ToString("HH:mm:ss");
                        }

                        ItemsReward(map, mob);
                        QuestKillReward(map, mob);
                        //ExperienceReward(map, mob);

                        SourceKillSpawn(map, mob);
                        TargetKillSpawn(map, mob);
                    }
                    break;

                case MobActionEnum.Wait:
                    {
                        if (mob.Respawn && DateTime.Now > mob.DieTime.AddSeconds(2))
                        {
                            mob.SetNextWalkTime(UtilitiesFunctions.RandomInt(7, 14));
                            mob.SetAgressiveCheckTime(5);
                            mob.SetRespawn();

                            if (mob.Class == 8)
                            {
                                mob.SetDeathAndResurrectionTime(null, null);
                                //SaveMobToDatabase(mob);
                            }
                        }
                    }
                    break;

                case MobActionEnum.Walk:
                    {
                        if (mob.DebuffList.ActiveBuffs.Count > 0)
                        {
                            var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                                buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

                            if (debuff.Any())
                            {
                                CheckDebuff(map, mob, debuff);
                                break;
                            }
                        }

                        map.BroadcastForTargetTamers(mob.TamersViewing, new SyncConditionPacket(mob.GeneralHandler, ConditionEnum.Default).Serialize());
                        mob.Move();
                        map.BroadcastForTargetTamers(mob.TamersViewing, new MobWalkPacket(mob).Serialize());
                    }
                    break;

                case MobActionEnum.GiveUp:
                    {
                        map.BroadcastForTargetTamers(mob.TamersViewing, new SyncConditionPacket(mob.GeneralHandler, ConditionEnum.Immortal).Serialize());
                        mob.ResetLocation();
                        map.BroadcastForTargetTamers(mob.TamersViewing, new MobRunPacket(mob).Serialize());
                        map.BroadcastForTargetTamers(mob.TamersViewing, new SetCombatOffPacket(mob.GeneralHandler).Serialize());

                        foreach (var targetTamer in mob.TargetTamers)
                        {
                            if (targetTamer.TargetMobs.Count <= 1)
                            {
                                targetTamer.StopBattle();
                                map.BroadcastForTamerViewsAndSelf(targetTamer.Id, new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                            }
                        }

                        mob.Reset(true);
                        map.BroadcastForTargetTamers(mob.TamersViewing, new UpdateCurrentHPRatePacket(mob.GeneralHandler, mob.CurrentHpRate).Serialize());
                    }
                    break;

                case MobActionEnum.Attack:
                    {
                        if (mob.DebuffList.ActiveBuffs.Count > 0)
                        {
                            var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                                buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

                            if (debuff.Any())
                            {
                                CheckDebuff(map, mob, debuff);
                                break;
                            }
                        }

                        if (!mob.Dead && mob.SkillTime && !mob.CheckSkill && mob.IsPossibleSkill)
                        {
                            mob.UpdateCurrentAction(MobActionEnum.UseAttackSkill);
                            mob.SetNextAction();
                            break;
                        }

                        if (!mob.Dead && ((mob.TargetTamer == null || mob.TargetTamer.Hidden)))
                        {
                            mob.GiveUp();
                            break;
                        }

                        if (!mob.Dead && !mob.Chasing && mob.TargetAlive)
                        {
                            var diff = UtilitiesFunctions.CalculateDistance(
                                mob.CurrentLocation.X,
                                mob.Target.Location.X,
                                mob.CurrentLocation.Y,
                                mob.Target.Location.Y);

                            var range = Math.Max(mob.ARValue, mob.Target.BaseInfo.ARValue);
                            if (diff <= range)
                            {
                                if (DateTime.Now < mob.LastHitTime.AddMilliseconds(mob.ASValue))
                                    break;

                                var missed = false;

                                if (mob.TargetTamer != null && mob.TargetTamer.GodMode)
                                    missed = true;
                                else if (mob.CanMissHit())
                                    missed = true;

                                if (missed)
                                {
                                    mob.UpdateLastHitTry();
                                    map.BroadcastForTargetTamers(mob.TamersViewing, new MissHitPacket(mob.GeneralHandler, mob.TargetHandler).Serialize());
                                    mob.UpdateLastHit();
                                    break;
                                }

                                map.AttackTarget(mob, _assets.NpcColiseum);
                            }
                            else
                            {
                                map.ChaseTarget(mob);
                            }
                        }

                        if (mob.Dead)
                        {
                            foreach (var targetTamer in mob.TargetTamers)
                            {
                                if (targetTamer.TargetMobs.Count <= 1)
                                {
                                    targetTamer.StopBattle();
                                    map.BroadcastForTamerViewsAndSelf(targetTamer.Id, new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                                }
                            }
                        }
                    }
                    break;

                case MobActionEnum.UseAttackSkill:
                    {
                        if (mob.DebuffList.ActiveBuffs.Count > 0)
                        {
                            var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                                buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

                            if (debuff.Any())
                            {
                                CheckDebuff(map, mob, debuff);
                                break;
                            }
                        }

                        if (!mob.Dead && ((mob.TargetTamer == null || mob.TargetTamer.Hidden))) //Anti-kite
                        {
                            mob.GiveUp();
                            break;
                        }

                        var skillList = _assets.MonsterSkillInfo.Where(x => x.Type == mob.Type).ToList();

                        if (!skillList.Any())
                        {
                            mob.UpdateCheckSkill(true);
                            mob.UpdateCurrentAction(MobActionEnum.Wait);
                            mob.UpdateLastSkill();
                            mob.UpdateLastSkillTry();
                            mob.SetNextAction();
                            break;
                        }

                        Random random = new Random();

                        var targetSkill = skillList[random.Next(0, skillList.Count)];

                        if (!mob.Dead && !mob.Chasing && mob.TargetAlive)
                        {
                            var diff = UtilitiesFunctions.CalculateDistance(
                                mob.CurrentLocation.X,
                                mob.Target.Location.X,
                                mob.CurrentLocation.Y,
                                mob.Target.Location.Y);

                            if (diff <= 1900)
                            {
                                if (DateTime.Now < mob.LastSkillTime.AddMilliseconds(mob.Cooldown) && mob.Cooldown > 0)
                                    break;

                                map.SkillTarget(mob, targetSkill, _assets.NpcColiseum);


                                if (mob.Target != null)
                                {
                                    mob.UpdateCurrentAction(MobActionEnum.Wait);

                                    mob.SetNextAction();
                                }
                            }
                            else
                            {
                                map.ChaseTarget(mob);
                            }
                        }

                        if (mob.Dead)
                        {
                            foreach (var targetTamer in mob.TargetTamers)
                            {
                                if (targetTamer.TargetMobs.Count <= 1)
                                {
                                    targetTamer.StopBattle();
                                    map.BroadcastForTamerViewsAndSelf(targetTamer.Id, new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                                }
                            }

                            break;
                        }
                    }
                    break;
            }
        }

        // -------------------------------------------------------------------------------

        private static void CheckDebuff(GameMap map, MobConfigModel mob, List<MobDebuffModel> debuffs)
        {
            if (debuffs != null)
            {
                for (int i = 0; i < debuffs.Count; i++)
                {
                    var debuff = debuffs[i];

                    if (!debuff.DebuffExpired && mob.CurrentAction != MobActionEnum.CrowdControl)
                    {
                        mob.UpdateCurrentAction(MobActionEnum.CrowdControl);
                    }

                    if (debuff.DebuffExpired && mob.CurrentAction == MobActionEnum.CrowdControl)
                    {
                        debuffs.Remove(debuff);

                        if (debuffs.Count == 0)
                        {
                            map.BroadcastForTargetTamers(mob.TamersViewing, new RemoveBuffPacket(mob.GeneralHandler, debuff.BuffId, 1).Serialize());

                            mob.DebuffList.Buffs.Remove(debuff);

                            mob.UpdateCurrentAction(MobActionEnum.Wait);
                            mob.SetNextAction();
                        }
                        else
                        {
                            mob.DebuffList.Buffs.Remove(debuff);
                        }
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------

        private static void TargetKillSpawn(GameMap map, MobConfigModel mob)
        {
            var targetKillSpawn = map.KillSpawns.FirstOrDefault(x =>
                x.TargetMobs.Any(mobConfigModel => mobConfigModel.TargetMobType == mob.Type));

            if (targetKillSpawn != null)
            {
                mob.SetAwaitingKillSpawn();

                foreach (var targetMob in targetKillSpawn.TargetMobs.Where(x => x.TargetMobType == mob.Type).ToList())
                {
                    if (!map.Mobs.Exists(x => x.Type == targetMob.TargetMobType && !x.AwaitingKillSpawn))
                    {
                        targetKillSpawn.DecreaseTempMobs(targetMob);
                        targetKillSpawn.ResetCurrentSourceMobAmount();

                        map.BroadcastForMap(new KillSpawnEndChatNotifyPacket(targetMob.TargetMobType).Serialize());
                    }
                }
            }
        }

        private static void SourceKillSpawn(GameMap map, MobConfigModel mob)
        {
            var sourceMobKillSpawn =
                map.KillSpawns.FirstOrDefault(ks => ks.SourceMobs.Any(sm => sm.SourceMobType == mob.Type));

            if (sourceMobKillSpawn == null)
                return;

            var sourceKillSpawn = sourceMobKillSpawn.SourceMobs.FirstOrDefault(x => x.SourceMobType == mob.Type);

            if (sourceKillSpawn != null && sourceKillSpawn.CurrentSourceMobRequiredAmount <=
                sourceKillSpawn.SourceMobRequiredAmount)
            {
                sourceKillSpawn.DecreaseCurrentSourceMobAmount();

                if (sourceMobKillSpawn.ShowOnMinimap && sourceKillSpawn.CurrentSourceMobRequiredAmount <= 10)
                {
                    map.BroadcastForMap(new KillSpawnMinimapNotifyPacket(sourceKillSpawn.SourceMobType,
                        sourceKillSpawn.CurrentSourceMobRequiredAmount).Serialize());
                }

                if (sourceMobKillSpawn.Spawn())
                {
                    foreach (var targetMob in sourceMobKillSpawn.TargetMobs)
                    {
                        //TODO: para todos os canais (apenas do mapa)
                        map.BroadcastForMap(
                            new KillSpawnChatNotifyPacket(map.MapId, map.Channel, targetMob.TargetMobType).Serialize());

                        map.Mobs.Where(x => x.Type == targetMob.TargetMobType)?.ToList().ForEach(mobConfigModel =>
                        {
                            mobConfigModel.SetRespawn(true);
                            mobConfigModel.SetAwaitingKillSpawn(false);
                        });
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------

        private void QuestKillReward(GameMap map, MobConfigModel mob)
        {
            var partyIdList = new List<int>();

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                var giveUpList = new List<short>();

                foreach (var questInProgress in tamer.Progress.InProgressQuestData)
                {
                    var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questInProgress.QuestId);
                    if (questInfo != null)
                    {
                        if (!questInfo.QuestGoals.Exists(x => x.GoalType == QuestGoalTypeEnum.KillMonster))
                            continue;

                        var goalIndex = -1;
                        foreach (var questGoal in questInfo.QuestGoals)
                        {
                            if (questGoal.GoalId == mob?.Type)
                            {
                                goalIndex = questInfo.QuestGoals.FindIndex(x => x == questGoal);
                                break;
                            }
                        }

                        if (goalIndex != -1)
                        {
                            var currentGoalValue =
                                tamer.Progress.GetQuestGoalProgress(questInProgress.QuestId, goalIndex);
                            if (currentGoalValue < questInfo.QuestGoals[goalIndex].GoalAmount)
                            {
                                currentGoalValue++;
                                tamer.Progress.UpdateQuestInProgress(questInProgress.QuestId, goalIndex,
                                    currentGoalValue);
                                var questToUpdate =
                                    tamer.Progress.InProgressQuestData.FirstOrDefault(x =>
                                        x.QuestId == questInProgress.QuestId);

                                targetClient.Send(new QuestGoalUpdatePacket(questInProgress.QuestId, (byte)goalIndex,
                                    currentGoalValue));
                                _sender.Send(new UpdateCharacterInProgressCommand(questToUpdate));
                            }
                        }
                    }
                    else
                    {
                        _logger.Error($"Unknown quest id {questInProgress.QuestId}.");
                        targetClient.Send(new SystemMessagePacket($"Unknown quest id {questInProgress.QuestId}."));
                        giveUpList.Add(questInProgress.QuestId);
                    }
                }

                giveUpList.ForEach(giveUp => { tamer.Progress.RemoveQuest(giveUp); });

                var party = _partyManager.FindParty(targetClient.TamerId);
                if (party != null && !partyIdList.Contains(party.Id))
                {
                    partyIdList.Add(party.Id);

                    foreach (var partyMemberId in party.Members.Values.Select(x => x.Id))
                    {
                        var partyMemberClient = map.Clients.FirstOrDefault(x => x.TamerId == partyMemberId);
                        if (partyMemberClient == null || partyMemberId == targetClient.TamerId)
                            continue;

                        giveUpList = new List<short>();

                        foreach (var questInProgress in partyMemberClient.Tamer.Progress.InProgressQuestData)
                        {
                            var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questInProgress.QuestId);
                            if (questInfo != null)
                            {
                                if (!questInfo.QuestGoals.Exists(x => x.GoalType == QuestGoalTypeEnum.KillMonster))
                                    continue;

                                var goalIndex = -1;
                                foreach (var questGoal in questInfo.QuestGoals)
                                {
                                    if (questGoal.GoalId == mob?.Type)
                                    {
                                        goalIndex = questInfo.QuestGoals.FindIndex(x => x == questGoal);
                                        break;
                                    }
                                }

                                if (goalIndex != -1)
                                {
                                    var currentGoalValue =
                                        partyMemberClient.Tamer.Progress.GetQuestGoalProgress(questInProgress.QuestId,
                                            goalIndex);
                                    if (currentGoalValue < questInfo.QuestGoals[goalIndex].GoalAmount)
                                    {
                                        currentGoalValue++;
                                        partyMemberClient.Tamer.Progress.UpdateQuestInProgress(questInProgress.QuestId,
                                            goalIndex, currentGoalValue);
                                        var questToUpdate =
                                            partyMemberClient.Tamer.Progress.InProgressQuestData.FirstOrDefault(x =>
                                                x.QuestId == questInProgress.QuestId);

                                        partyMemberClient.Send(new QuestGoalUpdatePacket(questInProgress.QuestId,
                                            (byte)goalIndex, currentGoalValue));
                                        _sender.Send(new UpdateCharacterInProgressCommand(questToUpdate));
                                    }
                                }
                            }
                            else
                            {
                                _logger.Error($"Unknown quest id {questInProgress.QuestId}.");
                                partyMemberClient.Send(
                                    new SystemMessagePacket($"Unknown quest id {questInProgress.QuestId}."));
                                giveUpList.Add(questInProgress.QuestId);
                            }
                        }

                        giveUpList.ForEach(giveUp => { partyMemberClient.Tamer.Progress.RemoveQuest(giveUp); });
                    }
                }
            }

            partyIdList.Clear();
        }

        private void QuestDropReward(GameMap map, MobConfigModel mob)
        {
            var itemsReward = new List<ItemDropConfigModel>();
            itemsReward.AddRange(mob.DropReward.Drops);
            itemsReward.RemoveAll(x => !_assets.QuestItemList.Contains(x.ItemId));

            if (!itemsReward.Any())
                return;

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                if (!tamer.Progress.InProgressQuestData.Any())
                    continue;

                var updateItemList = false;
                var possibleDrops = itemsReward.Randomize();
                foreach (var itemDrop in possibleDrops)
                {
                    if (itemDrop.Chance >= UtilitiesFunctions.RandomDouble())
                    {
                        foreach (var questInProgress in tamer.Progress.InProgressQuestData)
                        {
                            var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questInProgress.QuestId);
                            if (questInfo != null)
                            {
                                if (!questInfo.QuestGoals.Exists(x => x.GoalType == QuestGoalTypeEnum.LootItem))
                                    continue;

                                var goalIndex = -1;
                                foreach (var questGoal in questInfo.QuestGoals)
                                {
                                    if (questGoal.GoalId == itemDrop?.ItemId)
                                    {
                                        var inventoryItems = tamer.Inventory.FindItemsById(questGoal.GoalId);
                                        var goalAmount = questGoal.GoalAmount;

                                        foreach (var inventoryItem in inventoryItems)
                                        {
                                            goalAmount -= inventoryItem.Amount;
                                            if (goalAmount <= 0)
                                            {
                                                goalAmount = 0;
                                                break;
                                            }
                                        }

                                        if (goalAmount > 0)
                                        {
                                            goalIndex = questInfo.QuestGoals.FindIndex(x => x == questGoal);
                                            break;
                                        }
                                    }
                                }

                                if (goalIndex != -1)
                                {
                                    var newItem = new ItemModel();
                                    newItem.SetItemInfo(
                                        _assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemDrop.ItemId));

                                    if (newItem.ItemInfo == null)
                                    {
                                        _logger.Warning(
                                            $"No item info found with ID {itemDrop.ItemId} for tamer {tamer.Id}.");
                                        targetClient.Send(
                                            new SystemMessagePacket($"No item info found with ID {itemDrop.ItemId}."));
                                        continue;
                                    }

                                    newItem.ItemId = itemDrop.ItemId;
                                    newItem.Amount =
                                        UtilitiesFunctions.RandomInt(itemDrop.MinAmount, itemDrop.MaxAmount);

                                    var itemClone = (ItemModel)newItem.Clone();
                                    if (tamer.Inventory.AddItem(newItem))
                                    {
                                        updateItemList = true;
                                        targetClient.Send(new ReceiveItemPacket(itemClone,
                                            InventoryTypeEnum.Inventory));
                                    }
                                    else
                                    {
                                        targetClient.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                                    }
                                }
                            }
                            else
                            {
                                _logger.Error($"Unknown quest id {questInProgress.QuestId}.");
                                targetClient.Send(
                                    new SystemMessagePacket($"Unknown quest id {questInProgress.QuestId}."));
                            }
                        }

                        if (updateItemList) _sender.Send(new UpdateItemsCommand(tamer.Inventory));

                        itemsReward.RemoveAll(x => x.Id == itemDrop.Id);
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------

        private void DropReward(GameMap map, MobConfigModel mob)
        {
            var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == mob.TargetTamer?.Id);

            if (targetClient == null)
                return;

            BitDropReward(map, mob, targetClient);

            ItemDropReward(map, mob, targetClient);
        }

        // -------------------------------------------------------------------------------

        private void ItemsReward(GameMap map, MobConfigModel mob)
        {
            if (mob.DropReward == null)
                return;

            QuestDropReward(map, mob);

            if (mob.Class == 8)
                RaidReward(map, mob);
            else
                DropReward(map, mob);
        }

        private void ItemDropReward(GameMap map, MobConfigModel mob, GameClient? targetClient)
        {
            if (!mob.DropReward.Drops.Any())
                return;

            var itemsReward = new List<ItemDropConfigModel>();
            itemsReward.AddRange(mob.DropReward.Drops);
            itemsReward.RemoveAll(x => _assets.QuestItemList.Contains(x.ItemId));

            if (!itemsReward.Any())
                return;

            var dropped = 0;
            var totalDrops = UtilitiesFunctions.RandomInt(mob.DropReward.MinAmount, mob.DropReward.MaxAmount);

            while (dropped < totalDrops)
            {
                if (!itemsReward.Any())
                {
                    _logger.Warning($"Mob {mob.Id} has incorrect drops configuration.");
                    _logger.Warning($"MinAmount {mob.DropReward.MinAmount} | MaxAmount {mob.DropReward.MaxAmount}");
                    break;
                }

                var possibleDrops = itemsReward.OrderBy(x => Guid.NewGuid()).ToList();
                foreach (var itemDrop in possibleDrops)
                {
                    if (itemDrop.Chance >= UtilitiesFunctions.RandomDouble())
                    {
                        if (targetClient.Tamer.HasAura && targetClient.Tamer.Aura.ItemInfo.Section == 2100)
                        {
                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemDrop.ItemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning(
                                    $"No item info found with ID {itemDrop.ItemId} for tamer {targetClient.Tamer.Id}.");
                                targetClient.Send(
                                    new SystemMessagePacket($"No item info found with ID {itemDrop.ItemId}."));
                                continue;
                            }

                            newItem.ItemId = itemDrop.ItemId;
                            newItem.Amount = UtilitiesFunctions.RandomInt(itemDrop.MinAmount, itemDrop.MaxAmount);

                            var itemClone = (ItemModel)newItem.Clone();
                            if (targetClient.Tamer.Inventory.AddItem(newItem))
                            {
                                targetClient.Send(new ReceiveItemPacket(itemClone, InventoryTypeEnum.Inventory));
                                _sender.Send(new UpdateItemsCommand(targetClient.Tamer.Inventory));
                                _logger.Verbose(
                                    $"Character {targetClient.TamerId} aquired {newItem.ItemId} x{newItem.Amount} from " +
                                    $"mob {mob.Id} with magnetic aura {targetClient.Tamer.Aura.ItemId}.");
                            }
                            else
                            {
                                targetClient.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));

                                var drop = _dropManager.CreateItemDrop(
                                    targetClient.Tamer.Id,
                                    targetClient.Tamer.GeneralHandler,
                                    itemDrop.ItemId,
                                    itemDrop.MinAmount,
                                    itemDrop.MaxAmount,
                                    mob.CurrentLocation.MapId,
                                    mob.CurrentLocation.X,
                                    mob.CurrentLocation.Y
                                );

                                map.AddMapDrop(drop);
                            }

                            dropped++;
                        }
                        else
                        {
                            var drop = _dropManager.CreateItemDrop(
                                targetClient.Tamer.Id,
                                targetClient.Tamer.GeneralHandler,
                                itemDrop.ItemId,
                                itemDrop.MinAmount,
                                itemDrop.MaxAmount,
                                mob.CurrentLocation.MapId,
                                mob.CurrentLocation.X,
                                mob.CurrentLocation.Y
                            );

                            dropped++;

                            map.AddMapDrop(drop);
                        }

                        itemsReward.RemoveAll(x => x.Id == itemDrop.Id);
                        break;
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------

        private void BitDropReward(GameMap map, MobConfigModel mob, GameClient? targetClient)
        {
            var bitsReward = mob.DropReward.BitsDrop;

            if (bitsReward != null && bitsReward.Chance >= UtilitiesFunctions.RandomDouble())
            {
                if (targetClient.Tamer.HasAura && targetClient.Tamer.Aura.ItemInfo.Section == 2100)
                {
                    var amount = UtilitiesFunctions.RandomInt(bitsReward.MinAmount, bitsReward.MaxAmount);

                    targetClient.Send(new PickBitsPacket(targetClient.Tamer.GeneralHandler, amount));

                    targetClient.Tamer.Inventory.AddBits(amount);

                    _sender.Send(new UpdateItemsCommand(targetClient.Tamer.Inventory));
                    _sender.Send(new UpdateItemListBitsCommand(targetClient.Tamer.Inventory.Id,
                        targetClient.Tamer.Inventory.Bits));
                    _logger.Verbose(
                        $"Character {targetClient.TamerId} aquired {amount} bits from mob {mob.Id} with magnetic aura {targetClient.Tamer.Aura.ItemId}.");
                }
                else
                {
                    var drop = _dropManager.CreateBitDrop(
                        targetClient.TamerId, targetClient.Tamer.GeneralHandler,
                        bitsReward.MinAmount, bitsReward.MaxAmount,
                        mob.CurrentLocation.MapId, mob.CurrentLocation.X, mob.CurrentLocation.Y
                    );

                    map.AddMapDrop(drop);
                }
            }
        }

        // -------------------------------------------------------------------------------

        private void RaidReward(GameMap map, MobConfigModel mob)
        {
            var raidResult = mob.RaidDamage.Where(x => x.Key > 0).DistinctBy(x => x.Key);
            var keyValuePairs = raidResult.ToList();

            var writer = new PacketWriter();
            writer.Type(1604);
            writer.WriteInt(keyValuePairs.Count());

            int i = 1;

            var updateItemList = new List<ItemListModel>();
            var attackerName = string.Empty;
            var attackerType = 0;

            foreach (var raidTamer in keyValuePairs.OrderByDescending(x => x.Value))
            {
                _logger.Verbose($"Character {raidTamer.Key} rank {i} on raid {mob.Id} - {mob.Name} with damage {raidTamer.Value}.");

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == raidTamer.Key);

                if (i == 1)
                {
                    if (targetClient != null)
                    {
                        attackerName = targetClient.Tamer.Name;
                        attackerType = targetClient.Tamer.Partner.CurrentType;
                    }
                }

                if (i <= 10)
                {
                    writer.WriteInt(i);
                    writer.WriteString(targetClient?.Tamer?.Name ?? $"Tamer{i}");
                    writer.WriteString(targetClient?.Partner?.Name ?? $"Partner{i}");
                    writer.WriteInt(raidTamer.Value);
                }

                var bitsReward = mob.DropReward.BitsDrop;

                if (targetClient != null && bitsReward != null && bitsReward.Chance >= UtilitiesFunctions.RandomDouble())
                {
                    var drop = _dropManager.CreateBitDrop(
                        targetClient.Tamer.Id,
                        targetClient.Tamer.GeneralHandler,
                        bitsReward.MinAmount,
                        bitsReward.MaxAmount,
                        mob.CurrentLocation.MapId,
                        mob.CurrentLocation.X,
                        mob.CurrentLocation.Y
                    );

                    map.DropsToAdd.Add(drop);
                }

                var raidRewards = mob.DropReward.Drops;
                raidRewards.RemoveAll(x => _assets.QuestItemList.Contains(x.ItemId));

                if (targetClient != null && raidRewards != null && raidRewards.Any())
                {
                    var i1 = i;
                    var rewards = raidRewards.Where(x => x.Rank == i1);
                    var itemDropConfigModels = rewards.ToList();

                    if (!itemDropConfigModels.Any())
                        rewards = raidRewards.Where(x => x.Rank == raidRewards.Max(itemDropConfigModel => itemDropConfigModel.Rank));

                    foreach (var reward in itemDropConfigModels)
                    {
                        if (reward.Chance >= UtilitiesFunctions.RandomDouble())
                        {
                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == reward.ItemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning($"No item info found with ID {reward.ItemId} for tamer {targetClient.TamerId}.");

                                targetClient.Send(new SystemMessagePacket($"No item info found with ID {reward.ItemId}."));
                                break;
                            }

                            newItem.ItemId = reward.ItemId;
                            newItem.Amount = UtilitiesFunctions.RandomInt(reward.MinAmount, reward.MaxAmount);

                            if (newItem.IsTemporary)
                                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                            var itemClone = (ItemModel)newItem.Clone();

                            if (targetClient.Tamer.Inventory.AddItem(newItem))
                            {
                                targetClient.Send(new ReceiveItemPacket(itemClone, InventoryTypeEnum.Inventory));
                                updateItemList.Add(targetClient.Tamer.Inventory);
                            }
                            else
                            {
                                targetClient.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                                targetClient.Tamer.GiftWarehouse.AddItem(newItem);
                                updateItemList.Add(targetClient.Tamer.GiftWarehouse);
                            }
                        }
                    }
                }

                i++;
            }

            map.BroadcastForTargetTamers(mob.RaidDamage.Select(x => x.Key).ToList(), writer.Serialize());
            updateItemList.ForEach(itemList => { _sender.Send(new UpdateItemsCommand(itemList)); });
        }

        // -------------------------------------------------------------------------------

    }

}
