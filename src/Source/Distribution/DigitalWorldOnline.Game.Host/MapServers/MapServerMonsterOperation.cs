using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Map;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.Infrastructure.Migrations;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class MapServer
    {
        private void MonsterOperation(GameMap map)
        {
            if (!map.ConnectedTamers.Any())
                return;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            map.UpdateMapMobs();
            //------------------------------------------------------------------------------------------------- EVENT MOBS
            _eventManager.EventServer(this);
            //-------------------------------------------------------------------------------------------------
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

            map.UpdateMapMobs(true);

            foreach (var mob in map.SummonMobs.ToList())
            {
                if (DateTime.Now > mob.ViewCheckTime)
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

                                targetClient?.Send(new LoadMobsPacket(mob,true));
                            }
                        });
                    }

                    // var farTamers = map.ConnectedTamers.Select(x => x.Id).Except(nearTamers).ToList();
                    //
                    // farTamers.ForEach(farTamer =>
                    // {
                    //     if (mob.TamersViewing.Contains(farTamer))
                    //     {
                    //         var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == farTamer);
                    //
                    //         mob.TamersViewing.Remove(farTamer);
                    //         targetClient?.Send(new UnloadMobsPacket(mob));
                    //     }
                    // });
                }

                if (!mob.CanAct)
                    continue;

                MobsOperation(map, mob);

                mob.SetNextAction();
            }

            stopwatch.Stop();

            var totalTime = stopwatch.Elapsed.TotalMilliseconds;

            if (totalTime >= 1000)
                Console.WriteLine($"MonstersOperation ({map.Mobs.Count}): {totalTime}.");
        }

        private void MobsOperation(GameMap map, MobConfigModel mob)
        {
            switch (mob.CurrentAction)
            {
                case MobActionEnum.CrowdControl:
                    {
                        var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                            buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

                        var dot = mob.DebuffList.ActiveBuffs.Where(buff =>
                            buff.BuffInfo.SkillInfo.Apply.Any(apply =>
                            apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.DOT ||
                            apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.DOT2
                            )).ToList();

                        if (debuff.Any())
                        {
                            CheckDebuff(map, mob, debuff);
                            break;
                        }
                        else if (dot.Any())
                        {
                            CheckDotDebuff(map, mob, dot);
                            break;
                        }
                    }
                    break;

                case MobActionEnum.Respawn:
                    {
                        mob.Reset();
                        mob.ResetLocation();
                        if (mob.RespawnInterval > 3599)
                            CallDiscordWarnings($"[SPAWN] {mob.Name} was spawned {map.Name} CH{map.Channel}.", "81ffcf");
                    }
                    break;

                case MobActionEnum.Reward:
                    {
                        if (mob.Class == 8)
                        {
                            mob.UpdateDeathAndResurrectionTime();
                            SaveMobToDatabase(mob);
                        }

                        if (mob.RespawnInterval > 3599)
                        {
                            TimeSpan time = TimeSpan.FromSeconds(mob.RespawnInterval);
                            DateTime currentTime = DateTime.Now;
                            DateTime newTime = currentTime.Add(time);
                            string formattedTime = newTime.ToString("HH:mm:ss");
                            //CallDiscordWarnings($"[DEAD] - {mob.Name} was killed in {map.Name} Channel {map.Channel}. \nNascerá novamente as {formattedTime} (UTC-03), em {(int)(mob.RespawnInterval / 3600)}hrs!", "ff0000");
                        }

                        ItemsReward(map, mob);
                        QuestKillReward(map, mob);
                        ExperienceReward(map, mob);

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
                                SaveMobToDatabase(mob);
                            }
                        }
                        else
                        {
                            // map.AttackNearbyTamer(mob, mob.TamersViewing, _assets.NpcColiseum); // comment ?
                        }
                    }
                    break;

                case MobActionEnum.Walk:
                    {
                        if (mob.DebuffList.ActiveBuffs.Count > 0)
                        {
                            var debuff = mob.DebuffList.ActiveBuffs.Where(buff =>
                                buff.BuffInfo.SkillInfo.Apply.Any(apply =>
                                    apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl
                                )
                            ).ToList();

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
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new SyncConditionPacket(mob.GeneralHandler, ConditionEnum.Immortal).Serialize());
                        mob.ResetLocation();
                        map.BroadcastForTargetTamers(mob.TamersViewing, new MobRunPacket(mob).Serialize());
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new SetCombatOffPacket(mob.GeneralHandler).Serialize());

                        foreach (var targetTamer in mob.TargetTamers)
                        {
                            if (targetTamer.TargetMobs.Count <= 1)
                            {
                                targetTamer.StopBattle();
                                map.BroadcastForTamerViewsAndSelf(targetTamer.Id,
                                    new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                            }
                        }

                        mob.Reset(true);
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new UpdateCurrentHPRatePacket(mob.GeneralHandler, mob.CurrentHpRate).Serialize());
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
                                buff.BuffInfo.SkillInfo.Apply.Any(apply =>
                                    apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl)).ToList();

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
                                    map.BroadcastForTamerViewsAndSelf(targetTamer.Id,
                                        new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                                }
                            }

                            break;
                        }
                    }
                    break;
            }
        }

        private void CallDiscordWarnings(string v1, string v2)
        {
            // throw new NotImplementedException();
        }

        // -----------------------------------------------------------------------------------------

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
                            map.BroadcastForTargetTamers(mob.TamersViewing,
                                new RemoveBuffPacket(mob.GeneralHandler, debuff.BuffId, 1).Serialize());

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

        private static void CheckDotDebuff(GameMap map, MobConfigModel mob, List<MobDebuffModel> debuffs)
        {
            //Console.WriteLine($"CheckDotDebuff (MapServer)!! {mob.CurrentAction}");

            if (debuffs != null)
            {
                for (int i = 0; i < debuffs.Count; i++)
                {
                    var debuff = debuffs[i];

                    if (debuff.DebuffExpired && mob.CurrentAction == MobActionEnum.CrowdControl)
                    {
                        debuffs.Remove(debuff);

                        if (debuffs.Count == 0)
                        {
                            map.BroadcastForTargetTamers(mob.TamersViewing,
                                new RemoveBuffPacket(mob.GeneralHandler, debuff.BuffId, 1).Serialize());

                            mob.DebuffList.Buffs.Remove(debuff);

                            mob.UpdateCurrentAction(MobActionEnum.Wait);
                            mob.SetNextAction();
                        }
                        else
                        {
                            mob.DebuffList.Buffs.Remove(debuff);
                        }
                    }
                    else if (!debuff.DebuffExpired && mob.CurrentAction == MobActionEnum.CrowdControl)
                    {
                        if (!mob.Dead && !mob.Chasing && mob.TargetAlive)
                        {
                            var diff = UtilitiesFunctions.CalculateDistance(
                                mob.CurrentLocation.X, mob.Target.Location.X,
                                mob.CurrentLocation.Y, mob.Target.Location.Y);

                            if (diff <= 1900)
                            {
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

                    }

                }
            }
        }

        // -----------------------------------------------------------------------------------------

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

        private long BonusPartnerExp(GameMap map,MobConfigModel mob)
        {
            long totalPartnerExp = 0;

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                double expBonusMultiplier = tamer.BonusEXP / 100.0 + targetClient.ServerExperience / 100.0;
                double levelDifference = mob.Level - tamer.Partner.Level;

                long partnerExpToReceive = (long)CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience);
                long finalExp = (long)(partnerExpToReceive * expBonusMultiplier);

                totalPartnerExp += (finalExp - partnerExpToReceive); 


            }

            return totalPartnerExp;
        }




        private long BonusTamerExp(GameMap map,SummonMobModel mob)
        {
            long totalTamerExp = 0; 

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;


                double expBonusMultiplier = tamer.BonusEXP / 100.0 + targetClient.ServerExperience / 100.0;
                double levelDifference = mob.Level - tamer.Partner.Level;


                long tamerExpToReceive = (long)CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience);

                long finalExp = (long)(tamerExpToReceive * expBonusMultiplier);


                totalTamerExp += (finalExp - tamerExpToReceive);
            }

            return totalTamerExp;
        }


        private long BonusPartnerExp(GameMap map,SummonMobModel mob)
        {
            long totalPartnerExp = 0; 

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                double expBonusMultiplier = tamer.BonusEXP / 100.0 + targetClient.ServerExperience / 100.0;
                double levelDifference = mob.Level - tamer.Partner.Level;

                long partnerExpToReceive = (long)CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience);
                long finalExp = (long)(partnerExpToReceive * expBonusMultiplier);

   

                totalPartnerExp += (finalExp - partnerExpToReceive); 


            }

            return totalPartnerExp;
        }




        private long BonusTamerExp(GameMap map,MobConfigModel mob)
        {
            long totalTamerExp = 0; 

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                double expBonusMultiplier = tamer.BonusEXP / 100.0 + targetClient.ServerExperience / 100.0;

                long tamerExpToReceive = (long)CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience);

                long finalExp = (long)(tamerExpToReceive * expBonusMultiplier);

                totalTamerExp += (finalExp - tamerExpToReceive);
            }

            return totalTamerExp;
        }


        private void ExperienceReward(GameMap map,MobConfigModel mob)
        {
            if (mob.ExpReward == null)
                return;

            var partyIdList = new List<int>();

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                var tamerExpToReceive = (long)(CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience));
                if (CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience) == 0)
                    tamerExpToReceive = 0;

                if (tamerExpToReceive > 100) tamerExpToReceive += UtilitiesFunctions.RandomInt(-35,45);
                var tamerResult = ReceiveTamerExp(targetClient.Tamer,tamerExpToReceive);

                var partnerExpToReceive = (long)(CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience));



                if (CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience) == 0)
                    partnerExpToReceive = 0;

                if (partnerExpToReceive > 100) partnerExpToReceive += UtilitiesFunctions.RandomInt(-35,45);
                var partnerResult = ReceivePartnerExp(targetClient,targetClient.Partner,mob,partnerExpToReceive);

                var totalTamerExp = BonusTamerExp(map,mob);

                var bonusTamerExp = ReceiveBonusTamerExp(targetClient.Tamer,totalTamerExp);

                var totalPartnerExp = BonusPartnerExp(map,mob);

                var bonusPartnerExp = ReceiveBonusPartnerExp(targetClient.Partner,mob,totalPartnerExp);


                targetClient.Send(
                    new ReceiveExpPacket(
                        tamerExpToReceive,
                        totalTamerExp,
                        targetClient.Tamer.CurrentExperience,
                        targetClient.Partner.GeneralHandler,
                        partnerExpToReceive,
                        totalPartnerExp,
                        targetClient.Partner.CurrentExperience,
                        targetClient.Partner.CurrentEvolution.SkillExperience
                    )
                );

                SkillExpReward(map,targetClient);

                if (tamerResult.LevelGain > 0 || partnerResult.LevelGain > 0)
                {
                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                    map.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                        new UpdateMovementSpeedPacket(targetClient.Tamer).Serialize());
                }

                _sender.Send(new UpdateCharacterExperienceCommand(tamer));
                _sender.Send(new UpdateDigimonExperienceCommand(tamer.Partner));

                PartyExperienceReward(map,mob,partyIdList,targetClient,tamerExpToReceive,tamerResult,partnerExpToReceive,partnerResult);
            }

            partyIdList.Clear();
        }

        public long CalculateExperience(int tamerLevel, int mobLevel, long baseExperience)
        {
            int levelDifference = tamerLevel - mobLevel;

            // Verify if Tamer is 30 levels more than mob
            if (levelDifference <= 25)
            {
                //if (levelDifference > 0)
                //{
                //return (long)(baseExperience * (1.0 - levelDifference * 0.03)); // 0.03 é o redutor por nível (3%)
                //}
            }
            else
            {
                return 0; // Tamer dont win exp
            }

            return baseExperience;
        }

        private void SkillExpReward(GameMap map, GameClient? targetClient)
        {
            var ExpNeed = int.MaxValue;
            var evolutionType = _assets.DigimonBaseInfo.First(x => x.Type == targetClient.Partner.CurrentEvolution.Type)
                .EvolutionType;

            ExpNeed = SkillExperienceTable(evolutionType, targetClient.Partner.CurrentEvolution.SkillMastery);

            if (targetClient.Partner.CurrentEvolution.SkillMastery < 30) // Skill Mastery bloqueia exp no 30
            {
                if (targetClient.Partner.CurrentEvolution.SkillExperience >= ExpNeed)
                {
                    targetClient.Partner.ReceiveSkillPoint(); // Increase skill point by 2
                    targetClient.Partner.ResetSkillExp(0); // Reset Skill exp to 0

                    var evolutionIndex = targetClient.Partner.Evolutions.IndexOf(targetClient.Partner.CurrentEvolution);

                    var packet = new PacketWriter();
                    packet.Type(1105);
                    packet.WriteInt(targetClient.Partner.GeneralHandler);
                    packet.WriteByte((byte)(evolutionIndex + 1));
                    packet.WriteByte(targetClient.Partner.CurrentEvolution.SkillPoints);
                    packet.WriteByte(targetClient.Partner.CurrentEvolution.SkillMastery);
                    packet.WriteInt(targetClient.Partner.CurrentEvolution.SkillExperience);

                    map.BroadcastForTamerViewsAndSelf(targetClient.TamerId, packet.Serialize());
                }
            }
            else
            {
                //_logger.Information("Skill Mastery has reached the maximum level 30, no more Skill Points or Experience can be gained.");
            }
        }

        private async Task PartyExperienceReward(
            GameMap map,
            MobConfigModel mob,
            List<int> partyIdList,
            GameClient? targetClient,
            long tamerExpToReceive,
            ReceiveExpResult tamerResult,
            long partnerExpToReceive,
            ReceiveExpResult partnerResult)
        {
            if (targetClient == null) throw new ArgumentNullException(nameof(targetClient));

            var party = _partyManager.FindParty(targetClient.TamerId);
            if (party != null && !partyIdList.Contains(party.Id))
            {
                partyIdList.Add(party.Id);

                foreach (var partyMemberId in party.Members.Values.Select(x => x.Id))
                {
                    var partyMemberClient = map.Clients.FirstOrDefault(x => x.TamerId == partyMemberId);
                    if (partyMemberClient == null || partyMemberId == targetClient.TamerId)
                        continue;

                    var totalTamerExp = BonusTamerExp(map,mob) / 2;
                    var bonusTamerExp = ReceiveBonusTamerExp(partyMemberClient.Tamer,totalTamerExp);

                    var totalPartnerExp = BonusPartnerExp(map,mob) / 2;
                    var localPartnerResult = ReceivePartnerExp(targetClient,partyMemberClient.Partner,mob,totalPartnerExp);

                    partyMemberClient.Send(
                        new PartyReceiveExpPacket(
                            tamerExpToReceive,
                            totalTamerExp,//TODO: obter os bonus
                            partyMemberClient.Tamer.CurrentExperience,
                            partyMemberClient.Partner.GeneralHandler,
                            partnerExpToReceive,
                            totalPartnerExp,//TODO: obter os bonus
                            partyMemberClient.Partner.CurrentExperience,
                            partyMemberClient.Partner.CurrentEvolution.SkillExperience,
                            targetClient.Tamer.Name            // partySourceName
                        ));


                    if (tamerResult.LevelGain > 0 || localPartnerResult.LevelGain > 0)
                    {
                        targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                        map.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                            new UpdateMovementSpeedPacket(targetClient.Tamer).Serialize());
                    }

                    await _sender.Send(new UpdateCharacterExperienceCommand(partyMemberClient.Tamer));
                    await _sender.Send(new UpdateDigimonExperienceCommand(partyMemberClient.Partner));
                }
            }
        }

        private async Task PartyExperienceReward(
          GameMap map,
          SummonMobModel mob,
          List<int> partyIdList,
          GameClient? targetClient,
           long tamerExpToReceive,
           ReceiveExpResult tamerResult,
           long partnerExpToReceive,
           ReceiveExpResult partnerResult)
        {
            var party = _partyManager.FindParty(targetClient.TamerId);
            if (party != null && !partyIdList.Contains(party.Id))
            {
                partyIdList.Add(party.Id);

                foreach (var partyMemberId in party.Members.Values.Select(x => x.Id))
                {
                    var partyMemberClient = map.Clients.FirstOrDefault(x => x.TamerId == partyMemberId);
                    if (partyMemberClient == null || partyMemberId == targetClient.TamerId)
                        continue;

                    var totalTamerExp = BonusTamerExp(map,mob) / 2;
                    var bonusTamerExp = ReceiveBonusTamerExp(partyMemberClient.Tamer,totalTamerExp);

                    var totalPartnerExp = BonusPartnerExp(map,mob) / 2;
                    var localPartnerResult = ReceivePartnerExp(targetClient,partyMemberClient.Partner,mob,totalPartnerExp);

                    partyMemberClient.Send(
                        new PartyReceiveExpPacket(
                            tamerExpToReceive,
                            totalTamerExp,//TODO: obter os bonus
                            partyMemberClient.Tamer.CurrentExperience,
                            partyMemberClient.Partner.GeneralHandler,
                            partnerExpToReceive,
                            totalPartnerExp,//TODO: obter os bonus
                            partyMemberClient.Partner.CurrentExperience,
                            partyMemberClient.Partner.CurrentEvolution.SkillExperience,
                            targetClient.Tamer.Name            // partySourceName
                        ));

                    if (tamerResult.LevelGain > 0 || partnerResult.LevelGain > 0)
                    {
                        targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                        map.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                            new UpdateMovementSpeedPacket(targetClient.Tamer).Serialize());
                    }

                    await _sender.Send(new UpdateCharacterExperienceCommand(partyMemberClient.Tamer));
                    await _sender.Send(new UpdateDigimonExperienceCommand(partyMemberClient.Partner));
                }
            }
        }

        private void DropReward(GameMap map, MobConfigModel mob)
        {
            var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == mob.TargetTamer?.Id);
            if (targetClient == null)
                return;

            BitDropReward(map, mob, targetClient);

            ItemDropReward(map, mob, targetClient);
        }

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
                _logger.Verbose(
                    $"Character {raidTamer.Key} rank {i} on raid {mob.Id} - {mob.Name} with damage {raidTamer.Value}.");

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
                if (targetClient != null && bitsReward != null &&
                    bitsReward.Chance >= UtilitiesFunctions.RandomDouble())
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
                        rewards = raidRewards.Where(x =>
                            x.Rank == raidRewards.Max(itemDropConfigModel => itemDropConfigModel.Rank));

                    foreach (var reward in itemDropConfigModels)
                    {
                        if (reward.Chance >= UtilitiesFunctions.RandomDouble())
                        {
                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == reward.ItemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning(
                                    $"No item info found with ID {reward.ItemId} for tamer {targetClient.TamerId}.");
                                targetClient.Send(
                                    new SystemMessagePacket($"No item info found with ID {reward.ItemId}."));
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

            BlessingFIW(mob, attackerName, attackerType);
        }

        private async Task BlessingFIW(MobConfigModel mob, string attackerName, int attackerType)
        {
            if (mob.Bless)
            {
                for (int x = 0; x < 8; x++)
                {
                    var mapId = 1300 + x;

                    var currentMap = Maps.FirstOrDefault(gameMap => gameMap.Clients.Any() && gameMap.MapId == mapId);

                    if (currentMap != null)
                    {
                        var clients = currentMap.Clients;

                        var targetItem =
                            _assets.ItemInfo.FirstOrDefault(itemAssetModel => itemAssetModel.ItemId == 71552);

                        if (targetItem != null)
                        {
                            var buff = _assets.BuffInfo.FirstOrDefault(buffInfoAssetModel =>
                                buffInfoAssetModel.SkillCode == targetItem.SkillCode ||
                                buffInfoAssetModel.DigimonSkillCode == targetItem.SkillCode);

                            if (buff != null)
                            {
                                foreach (var client in clients)
                                {
                                    var duration = UtilitiesFunctions.RemainingTimeSeconds(targetItem.TimeInSeconds);

                                    var newDigimonBuff = DigimonBuffModel.Create(buff.BuffId, buff.SkillId,
                                        targetItem.TypeN, targetItem.TimeInSeconds);
                                    newDigimonBuff.SetBuffInfo(buff);
                                    client.Tamer.Partner.BuffList.Add(newDigimonBuff);

                                    client.Send(new GlobalMessagePacket(mob.Type, attackerName, attackerType,
                                        targetItem.ItemId));
                                    client.Send(new UpdateStatusPacket(client.Tamer));

                                    BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)targetItem.TypeN,
                                            duration).Serialize());

                                    await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RaidReward(GameMap map, SummonMobModel mob)
        {
            var raidResult = mob.RaidDamage.Where(x => x.Key > 0).DistinctBy(x => x.Key);
            var keyValuePairs = raidResult.ToList();

            var writer = new PacketWriter();
            writer.Type(1604);
            writer.WriteInt(keyValuePairs.Count());

            int i = 1;

            var updateItemList = new List<ItemListModel>();

            foreach (var raidTamer in keyValuePairs.OrderByDescending(x => x.Value))
            {
                _logger.Verbose(
                    $"Character {raidTamer.Key} rank {i} on raid {mob.Id} - {mob.Name} with damage {raidTamer.Value}.");

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == raidTamer.Key);

                if (i <= 10)
                {
                    writer.WriteInt(i);
                    writer.WriteString(targetClient?.Tamer?.Name ?? $"Tamer{i}");
                    writer.WriteString(targetClient?.Partner?.Name ?? $"Partner{i}");
                    writer.WriteInt(raidTamer.Value);
                }

                var bitsReward = mob.DropReward.BitsDrop;
                if (targetClient != null && bitsReward != null &&
                    bitsReward.Chance >= UtilitiesFunctions.RandomDouble())
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
                    foreach (var reward in raidRewards)
                    {
                        if (reward.Chance >= UtilitiesFunctions.RandomDouble())
                        {
                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == reward.ItemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning(
                                    $"No item info found with ID {reward.ItemId} for tamer {targetClient.TamerId}.");
                                targetClient.Send(
                                    new SystemMessagePacket($"No item info found with ID {reward.ItemId}."));
                                continue; // Continue para a próxima recompensa se não houver informações sobre o item.
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

        private void MobsOperation(GameMap map, SummonMobModel mob)
        {
            switch (mob.CurrentAction)
            {
                case MobActionEnum.Respawn:
                    {
                        mob.Reset();
                        mob.ResetLocation();
                    }
                    break;

                case MobActionEnum.Reward:
                    {
                        ItemsReward(map, mob);
                        QuestKillReward(map, mob);
                        ExperienceReward(map, mob);
                    }
                    break;

                case MobActionEnum.Wait:
                    {
                        if (mob.Respawn && DateTime.Now > mob.DieTime.AddSeconds(2))
                        {
                            mob.SetNextWalkTime(UtilitiesFunctions.RandomInt(7, 14));
                            mob.SetAgressiveCheckTime(5);
                            mob.SetRespawn();
                        }
                        else
                        {
                            map.AttackNearbyTamer(mob, mob.TamersViewing, _assets.NpcColiseum);
                        }
                    }
                    break;

                case MobActionEnum.Walk:
                    {
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new SyncConditionPacket(mob.GeneralHandler, ConditionEnum.Default).Serialize());
                        mob.Move();
                        map.BroadcastForTargetTamers(mob.TamersViewing, new MobWalkPacket(mob).Serialize());
                    }
                    break;

                case MobActionEnum.GiveUp:
                    {
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new SyncConditionPacket(mob.GeneralHandler, ConditionEnum.Immortal).Serialize());
                        mob.ResetLocation();
                        map.BroadcastForTargetTamers(mob.TamersViewing, new MobRunPacket(mob).Serialize());
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new SetCombatOffPacket(mob.GeneralHandler).Serialize());

                        foreach (var targetTamer in mob.TargetTamers)
                        {
                            targetTamer.StopBattle(true);
                            map.BroadcastForTamerViewsAndSelf(targetTamer.Id,
                                new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                        }

                        mob.Reset(true);
                        map.BroadcastForTargetTamers(mob.TamersViewing,
                            new UpdateCurrentHPRatePacket(mob.GeneralHandler, mob.CurrentHpRate).Serialize());
                    }
                    break;

                case MobActionEnum.Attack:
                    {
                        if (!mob.Dead && mob.SkillTime && !mob.CheckSkill && mob.IsPossibleSkill)
                        {
                            mob.UpdateCurrentAction(MobActionEnum.UseAttackSkill);
                            mob.SetNextAction();
                            break;
                        }

                        if (!mob.Dead && ((mob.TargetTamer == null || mob.TargetTamer.Hidden) ||
                                          DateTime.Now > mob.LastHitTryTime.AddSeconds(15))) //Anti-kite
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
                                    map.BroadcastForTargetTamers(mob.TamersViewing,
                                        new MissHitPacket(mob.GeneralHandler, mob.TargetHandler).Serialize());
                                    mob.UpdateLastHit();
                                    break;
                                }

                                map.AttackTarget(mob);
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
                                targetTamer.StopBattle(true);
                                map.BroadcastForTamerViewsAndSelf(targetTamer.Id,
                                    new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                            }

                            break;
                        }
                    }
                    break;

                case MobActionEnum.UseAttackSkill:
                    {
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

                                map.SkillTarget(mob, targetSkill);


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
                                targetTamer.StopBattle(true);
                                map.BroadcastForTamerViewsAndSelf(targetTamer.Id,
                                    new SetCombatOffPacket(targetTamer.Partner.GeneralHandler).Serialize());
                            }

                            break;
                        }
                    }
                    break;
            }
        }

        private void QuestKillReward(GameMap map, SummonMobModel mob)
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

                                targetClient.Send(new QuestGoalUpdatePacket(questInProgress.QuestId, (byte)goalIndex,
                                    currentGoalValue));
                                var questToUpdate =
                                    targetClient.Tamer.Progress.InProgressQuestData.FirstOrDefault(x =>
                                        x.QuestId == questInProgress.QuestId);
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

        private void ItemsReward(GameMap map, SummonMobModel mob)
        {
            if (mob.DropReward == null)
                return;

            QuestDropReward(map, mob);

            if (mob.Class == 8)
                RaidReward(map, mob);
            else
                DropReward(map, mob);
        }

        private void ExperienceReward(GameMap map,SummonMobModel mob)
        {
            if (mob.ExpReward == null)
                return;

            var partyIdList = new List<int>();

            foreach (var tamer in mob.TargetTamers)
            {
                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamer?.Id);
                if (targetClient == null)
                    continue;

                var tamerExpToReceive = (long)(CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience)); //TODO: +bonus
                if (CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.TamerExperience) == 0)
                    tamerExpToReceive = 0;

                if (tamerExpToReceive > 100) tamerExpToReceive += UtilitiesFunctions.RandomInt(-35,45);
                var tamerResult = ReceiveTamerExp(targetClient.Tamer,tamerExpToReceive);

                var partnerExpToReceive = (long)(CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience));



                if (CalculateExperience(tamer.Partner.Level,mob.Level,mob.ExpReward.DigimonExperience) == 0)
                    partnerExpToReceive = 0;

                if (partnerExpToReceive > 100) partnerExpToReceive += UtilitiesFunctions.RandomInt(-35,45);
                var partnerResult = ReceivePartnerExp(targetClient,targetClient.Partner,mob,partnerExpToReceive);

                var totalTamerExp = BonusTamerExp(map,mob);

                var bonusTamerExp = ReceiveBonusTamerExp(targetClient.Tamer,totalTamerExp);

                var totalPartnerExp = BonusPartnerExp(map,mob);

                var bonusPartnerExp = ReceiveBonusPartnerExp(targetClient.Partner,mob,totalPartnerExp);


                targetClient.Send(
                    new ReceiveExpPacket(
                        tamerExpToReceive,
                        totalTamerExp,
                        targetClient.Tamer.CurrentExperience,
                        targetClient.Partner.GeneralHandler,
                        partnerExpToReceive,
                        totalPartnerExp,
                        targetClient.Partner.CurrentExperience,
                        targetClient.Partner.CurrentEvolution.SkillExperience
                    )
                );

                //TODO: importar o DMBase e tratar isso
                SkillExpReward(map,targetClient);

                if (tamerResult.LevelGain > 0 || partnerResult.LevelGain > 0)
                {
                    targetClient.Send(new UpdateStatusPacket(targetClient.Tamer));

                    map.BroadcastForTamerViewsAndSelf(targetClient.TamerId,
                        new UpdateMovementSpeedPacket(targetClient.Tamer).Serialize());
                }

                _sender.Send(new UpdateCharacterExperienceCommand(tamer));
                _sender.Send(new UpdateDigimonExperienceCommand(tamer.Partner));

                PartyExperienceReward(map,mob,partyIdList,targetClient,tamerExpToReceive,tamerResult,partnerExpToReceive,partnerResult);
            }

            partyIdList.Clear();
        }

        private void DropReward(GameMap map, SummonMobModel mob)
        {
            var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == mob.TargetTamer?.Id);
            if (targetClient == null)
                return;

            BitDropReward(map, mob, targetClient);

            ItemDropReward(map, mob, targetClient);
        }

        private void BitDropReward(GameMap map,SummonMobModel mob,GameClient? targetClient)
        {
            var bitsReward = mob.DropReward.BitsDrop;
            if (bitsReward != null && bitsReward.Chance >= UtilitiesFunctions.RandomDouble())
            {
                if (targetClient.Tamer.HasAura && targetClient.Tamer.Aura.ItemInfo.Section == 2100)
                {
                    var amount = UtilitiesFunctions.RandomInt(bitsReward.MinAmount,bitsReward.MaxAmount);

                    targetClient.Send(
                        new PickBitsPacket(
                            targetClient.Tamer.GeneralHandler,
                            amount
                        )
                    );

                    targetClient.Tamer.Inventory.AddBits(amount);
                    _sender.Send(new UpdateItemListBitsCommand(targetClient.Tamer.Inventory));

                    _sender.Send(new UpdateItemsCommand(targetClient.Tamer.Inventory));
                    _logger.Verbose($"Character {targetClient.TamerId} aquired {amount} bits from mob {mob.Id} with magnetic aura {targetClient.Tamer.Aura.ItemId}.");
                }
                else
                {

                    var drop = _dropManager.CreateBitDrop(
                        targetClient.TamerId,
                        targetClient.Tamer.GeneralHandler,
                        bitsReward.MinAmount,
                        bitsReward.MaxAmount,
                        mob.CurrentLocation.MapId,
                        mob.CurrentLocation.X,
                        mob.CurrentLocation.Y
                    );

                    map.AddMapDrop(drop);
                }
            }
        }

        private void ItemDropReward(GameMap map, SummonMobModel mob, GameClient? targetClient)
        {
            if (!mob.DropReward.Drops.Any())
                return;

            var itemsReward = new List<SummonMobItemDropModel>();
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
                    _logger.Warning($"Mob {mob.Id} has incorrect drops configuration. (MapServer)");
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

        private void QuestDropReward(GameMap map, SummonMobModel mob)
        {
            var itemsReward = new List<SummonMobItemDropModel>();
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

        private int SkillExperienceTable(int evolutionType, int SkillMastery)
        {
            var RockieExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 281),
                new Tuple<int, int>(1, 315),
                new Tuple<int, int>(2, 352),
                new Tuple<int, int>(3, 395),
                new Tuple<int, int>(4, 442),
                new Tuple<int, int>(5, 495),
                new Tuple<int, int>(6, 555),
                new Tuple<int, int>(7, 621),
                new Tuple<int, int>(8, 696),
                new Tuple<int, int>(9, 779),
                new Tuple<int, int>(10, 873),
                new Tuple<int, int>(11, 977),
                new Tuple<int, int>(12, 1095),
                new Tuple<int, int>(13, 1226),
                new Tuple<int, int>(14, 1373),
                new Tuple<int, int>(15, 1538),
                new Tuple<int, int>(16, 1722),
                new Tuple<int, int>(17, 1930),
                new Tuple<int, int>(18, 2160),
                new Tuple<int, int>(19, 2420),
                new Tuple<int, int>(20, 2710),
                new Tuple<int, int>(21, 3036),
                new Tuple<int, int>(22, 3400),
                new Tuple<int, int>(23, 3808),
                new Tuple<int, int>(24, 4264),
                new Tuple<int, int>(25, 4776),
                new Tuple<int, int>(26, 5350),
                new Tuple<int, int>(27, 5992),
                new Tuple<int, int>(28, 6712),
                new Tuple<int, int>(29, 7516),
                new Tuple<int, int>(30, 8418)
            };

            var ChampionExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 621),
                new Tuple<int, int>(1, 696),
                new Tuple<int, int>(2, 779),
                new Tuple<int, int>(3, 872),
                new Tuple<int, int>(4, 977),
                new Tuple<int, int>(5, 1095),
                new Tuple<int, int>(6, 1226),
                new Tuple<int, int>(7, 1374),
                new Tuple<int, int>(8, 1538),
                new Tuple<int, int>(9, 1722),
                new Tuple<int, int>(10, 1930),
                new Tuple<int, int>(11, 2160),
                new Tuple<int, int>(12, 2420),
                new Tuple<int, int>(13, 2710),
                new Tuple<int, int>(14, 3036),
                new Tuple<int, int>(15, 3400),
                new Tuple<int, int>(16, 3808),
                new Tuple<int, int>(17, 4264),
                new Tuple<int, int>(18, 4776),
                new Tuple<int, int>(19, 5350),
                new Tuple<int, int>(20, 5992),
                new Tuple<int, int>(21, 6712),
                new Tuple<int, int>(22, 7516),
                new Tuple<int, int>(23, 8418),
                new Tuple<int, int>(24, 9428),
                new Tuple<int, int>(25, 10560),
                new Tuple<int, int>(26, 11828),
                new Tuple<int, int>(27, 13246),
                new Tuple<int, int>(28, 14386),
                new Tuple<int, int>(29, 16616),
                new Tuple<int, int>(30, 18610)
            };

            var UltimateExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 3036),
                new Tuple<int, int>(1, 3400),
                new Tuple<int, int>(2, 3808),
                new Tuple<int, int>(3, 4264),
                new Tuple<int, int>(4, 4776),
                new Tuple<int, int>(5, 5350),
                new Tuple<int, int>(6, 5992),
                new Tuple<int, int>(7, 6712),
                new Tuple<int, int>(8, 7516),
                new Tuple<int, int>(9, 8418),
                new Tuple<int, int>(10, 9428),
                new Tuple<int, int>(11, 10560),
                new Tuple<int, int>(12, 11828),
                new Tuple<int, int>(13, 13246),
                new Tuple<int, int>(14, 14836),
                new Tuple<int, int>(15, 16616),
                new Tuple<int, int>(16, 18610),
                new Tuple<int, int>(17, 20844),
                new Tuple<int, int>(18, 23344),
                new Tuple<int, int>(19, 26145),
                new Tuple<int, int>(20, 29283),
                new Tuple<int, int>(21, 32798),
                new Tuple<int, int>(22, 36734),
                new Tuple<int, int>(23, 41142),
                new Tuple<int, int>(24, 46078),
                new Tuple<int, int>(25, 51608),
                new Tuple<int, int>(26, 57800),
                new Tuple<int, int>(27, 64736),
                new Tuple<int, int>(28, 72504),
                new Tuple<int, int>(29, 81206),
                new Tuple<int, int>(30, 90950)
            };

            var MegaExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 18610),
                new Tuple<int, int>(1, 20844),
                new Tuple<int, int>(2, 23344),
                new Tuple<int, int>(3, 26145),
                new Tuple<int, int>(4, 29283),
                new Tuple<int, int>(5, 32798),
                new Tuple<int, int>(6, 36734),
                new Tuple<int, int>(7, 41142),
                new Tuple<int, int>(8, 46078),
                new Tuple<int, int>(9, 51608),
                new Tuple<int, int>(10, 57800),
                new Tuple<int, int>(11, 64736),
                new Tuple<int, int>(12, 72504),
                new Tuple<int, int>(13, 81206),
                new Tuple<int, int>(14, 90950),
                new Tuple<int, int>(15, 101864),
                new Tuple<int, int>(16, 114088),
                new Tuple<int, int>(17, 127778),
                new Tuple<int, int>(18, 143112),
                new Tuple<int, int>(19, 160286),
                new Tuple<int, int>(20, 179520),
                new Tuple<int, int>(21, 201062),
                new Tuple<int, int>(22, 225190),
                new Tuple<int, int>(23, 252212),
                new Tuple<int, int>(24, 282478),
                new Tuple<int, int>(25, 316374),
                new Tuple<int, int>(26, 354340),
                new Tuple<int, int>(27, 396860),
                new Tuple<int, int>(28, 444484),
                new Tuple<int, int>(29, 497822),
                new Tuple<int, int>(30, 557560)
            };

            var JogressExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 57800),
                new Tuple<int, int>(1, 64736),
                new Tuple<int, int>(2, 72504),
                new Tuple<int, int>(3, 81206),
                new Tuple<int, int>(4, 90950),
                new Tuple<int, int>(5, 101864),
                new Tuple<int, int>(6, 114088),
                new Tuple<int, int>(7, 127778),
                new Tuple<int, int>(8, 143112),
                new Tuple<int, int>(9, 160286),
                new Tuple<int, int>(10, 179520),
                new Tuple<int, int>(11, 201062),
                new Tuple<int, int>(12, 225190),
                new Tuple<int, int>(13, 252212),
                new Tuple<int, int>(14, 282478),
                new Tuple<int, int>(15, 316374),
                new Tuple<int, int>(16, 354340),
                new Tuple<int, int>(17, 396860),
                new Tuple<int, int>(18, 444484),
                new Tuple<int, int>(19, 497822),
                new Tuple<int, int>(20, 557560),
                new Tuple<int, int>(21, 624468),
                new Tuple<int, int>(22, 699404),
                new Tuple<int, int>(23, 783332),
                new Tuple<int, int>(24, 877332),
                new Tuple<int, int>(25, 982612),
                new Tuple<int, int>(26, 1100524),
                new Tuple<int, int>(27, 1232588),
                new Tuple<int, int>(28, 1380497),
                new Tuple<int, int>(29, 1546158),
                new Tuple<int, int>(30, 1731696)
            };

            var BurstModeExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 57800),
                new Tuple<int, int>(1, 64736),
                new Tuple<int, int>(2, 72504),
                new Tuple<int, int>(3, 81206),
                new Tuple<int, int>(4, 90950),
                new Tuple<int, int>(5, 101864),
                new Tuple<int, int>(6, 114088),
                new Tuple<int, int>(7, 127778),
                new Tuple<int, int>(8, 143112),
                new Tuple<int, int>(9, 160286),
                new Tuple<int, int>(10, 179520),
                new Tuple<int, int>(11, 201062),
                new Tuple<int, int>(12, 225190),
                new Tuple<int, int>(13, 252212),
                new Tuple<int, int>(14, 282478),
                new Tuple<int, int>(15, 316374),
                new Tuple<int, int>(16, 354340),
                new Tuple<int, int>(17, 396860),
                new Tuple<int, int>(18, 444484),
                new Tuple<int, int>(19, 497822),
                new Tuple<int, int>(20, 557560),
                new Tuple<int, int>(21, 624468),
                new Tuple<int, int>(22, 699404),
                new Tuple<int, int>(23, 783332),
                new Tuple<int, int>(24, 877332),
                new Tuple<int, int>(25, 982612),
                new Tuple<int, int>(26, 1100524),
                new Tuple<int, int>(27, 1232588),
                new Tuple<int, int>(28, 1380497),
                new Tuple<int, int>(29, 1546158),
                new Tuple<int, int>(30, 1731696)
            };

            var HybridExperienceTemp = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 200),
                new Tuple<int, int>(1, 224),
                new Tuple<int, int>(2, 250),
                new Tuple<int, int>(3, 280),
                new Tuple<int, int>(4, 314),
                new Tuple<int, int>(5, 352),
                new Tuple<int, int>(6, 394),
                new Tuple<int, int>(7, 442),
                new Tuple<int, int>(8, 496),
                new Tuple<int, int>(9, 554),
                new Tuple<int, int>(10, 622),
                new Tuple<int, int>(11, 696),
                new Tuple<int, int>(12, 780),
                new Tuple<int, int>(13, 872),
                new Tuple<int, int>(14, 977),
                new Tuple<int, int>(15, 1095),
                new Tuple<int, int>(16, 1226),
                new Tuple<int, int>(17, 1374),
                new Tuple<int, int>(18, 1538),
                new Tuple<int, int>(19, 1722),
                new Tuple<int, int>(20, 1930),
                new Tuple<int, int>(21, 2160),
                new Tuple<int, int>(22, 2420),
                new Tuple<int, int>(23, 2710),
                new Tuple<int, int>(24, 3036),
                new Tuple<int, int>(25, 3400),
                new Tuple<int, int>(26, 3808),
                new Tuple<int, int>(27, 4264),
                new Tuple<int, int>(28, 4776),
                new Tuple<int, int>(29, 5350),
                new Tuple<int, int>(30, 5992)
            };

            switch ((EvolutionRankEnum)evolutionType)
            {
                case EvolutionRankEnum.RookieX:
                case EvolutionRankEnum.Rookie:
                    return RockieExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.ChampionX:
                case EvolutionRankEnum.Champion:
                    return ChampionExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.UltimateX:
                case EvolutionRankEnum.Ultimate:
                    return UltimateExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.MegaX:
                case EvolutionRankEnum.Mega:
                    return MegaExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.BurstModeX:
                case EvolutionRankEnum.BurstMode:
                    return BurstModeExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.JogressX:
                case EvolutionRankEnum.Jogress:
                    return JogressExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.Capsule:
                    return HybridExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.Spirit:
                    return HybridExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                case EvolutionRankEnum.Extra:
                    return HybridExperienceTemp.FirstOrDefault(x => x.Item1 == SkillMastery)?.Item2 ?? -1;

                default:
                    break;
            }

            return -1;
        }
    }
}