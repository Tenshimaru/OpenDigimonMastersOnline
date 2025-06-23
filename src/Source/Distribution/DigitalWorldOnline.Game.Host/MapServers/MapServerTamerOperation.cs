using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
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
using DigitalWorldOnline.Game.Managers;
using System.Diagnostics;
using System.Text;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class MapServer
    {
        public void TamerOperation(GameMap map)
        {
            if (!map.ConnectedTamers.Any())
            {
                map.SetNoTamers();
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var tamer in map.ConnectedTamers)
            {
                var client = map.Clients.FirstOrDefault(x => x.TamerId == tamer.Id);

                if (client == null || !client.IsConnected || client.Partner == null)
                    continue;

                CheckLocationDebuff(client);
                //MapBuff(client);

                GetInViewMobs(map, tamer);
                GetInViewMobs(map, tamer, true);

                ShowOrHideTamer(map, tamer);
                ShowOrHideConsignedShop(map, tamer);

                if (tamer.TargetMobs.Count > 0)
                    PartnerAutoAttackMob(tamer);

                if (tamer.TargetSummonMobs.Count > 0)
                    PartnerAutoAttackSummon(tamer);

                if (tamer.TargetPartner != null)
                {
                    tamer.StopBattle();
                    tamer.Partner?.StopAutoAttack();
                }
                    
                CheckTimeReward(client);

                if (client.Tamer.AttendanceReward.ReedemRewards)
                {
                    CheckMonthlyReward(client);
                }

                tamer.AutoRegen();
                tamer.ActiveEvolutionReduction();

                if (tamer.BreakEvolution)
                {
                    tamer.ActiveEvolution.SetDs(0);
                    tamer.ActiveEvolution.SetXg(0);

                    if (tamer.Riding)
                    {
                        tamer.StopRideMode();

                        BroadcastForTamerViewsAndSelf(tamer.Id, new UpdateMovementSpeedPacket(tamer).Serialize());
                        BroadcastForTamerViewsAndSelf(tamer.Id, new RideModeStopPacket(tamer.GeneralHandler, tamer.Partner.GeneralHandler).Serialize());
                    }

                    var buffToRemove = client.Tamer.Partner.BuffList.TamerBaseSkill();
                    if (buffToRemove != null)
                    {
                        BroadcastForTamerViewsAndSelf(client.TamerId, new RemoveBuffPacket(client.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                    }

                    client.Tamer.RemovePartnerPassiveBuff();

                    map.BroadcastForTamerViewsAndSelf(tamer.Id,
                        new DigimonEvolutionSucessPacket(tamer.GeneralHandler,
                            tamer.Partner.GeneralHandler,
                            tamer.Partner.BaseType,
                            DigimonEvolutionEffectEnum.Back).Serialize());

                    var currentHp = client.Partner.CurrentHp;
                    var currentMaxHp = client.Partner.HP;
                    var currentDs = client.Partner.CurrentDs;
                    var currentMaxDs = client.Partner.DS;

                    tamer.Partner.UpdateCurrentType(tamer.Partner.BaseType);

                    tamer.Partner.SetBaseInfo(_statusManager.GetDigimonBaseInfo(tamer.Partner.CurrentType));

                    tamer.Partner.SetBaseStatus(
                        _statusManager.GetDigimonBaseStatus(
                            tamer.Partner.CurrentType,
                            tamer.Partner.Level,
                            tamer.Partner.Size
                        )
                    );

                    client.Tamer.SetPartnerPassiveBuff();

                    client.Partner.AdjustHpAndDs(currentHp, currentMaxHp, currentDs, currentMaxDs);

                    foreach (var buff in client.Tamer.Partner.BuffList.ActiveBuffs)
                        buff.SetBuffInfo(_assets.BuffInfo.FirstOrDefault(x =>
                            x.SkillCode == buff.SkillId && buff.BuffInfo == null ||
                            x.DigimonSkillCode == buff.SkillId && buff.BuffInfo == null));

                    client.Send(new UpdateStatusPacket(tamer));

                    if (client.Tamer.Partner.BuffList.TamerBaseSkill() != null)
                    {
                        var buffToApply = client.Tamer.Partner.BuffList.Buffs.Where(x => x.Duration == 0).ToList();

                        buffToApply.ForEach(digimonBuffModel =>
                        {
                            BroadcastForTamerViewsAndSelf(client.Tamer.Id,
                                new AddBuffPacket(client.Tamer.Partner.GeneralHandler, digimonBuffModel.BuffId,
                                    digimonBuffModel.SkillId, (short)digimonBuffModel.TypeN, 0).Serialize());
                        });
                    }

                    var party = _partyManager.FindParty(client.TamerId);

                    if (party != null)
                    {
                        party.UpdateMember(party[client.TamerId], client.Tamer);

                        BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberInfoPacket(party[client.TamerId]).Serialize());
                    }

                    _sender.Send(new UpdatePartnerCurrentTypeCommand(client.Partner));
                    _sender.Send(new UpdateCharacterActiveEvolutionCommand(tamer.ActiveEvolution));
                    _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
                }

                if (tamer.CheckExpiredItemsTime)
                {
                    tamer.SetLastExpiredItemsCheck();

                    tamer.Inventory.EquippedItems.ForEach(item =>
                    {
                        if (item.ItemInfo != null && item.IsTemporary && item.Expired)
                        {
                            if (item.ItemInfo.UseTimeType == 2 || item.ItemInfo.UseTimeType == 3)
                            {
                                item.SetFirstExpired(false);
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabInven, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Quit));
                            }
                            else if (item.ItemInfo.UseTimeType == 4)
                            {
                                item.SetFirstExpired(false);
                            }
                            else
                            {
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabInven, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Remove));
                                tamer.Inventory.RemoveOrReduceItem(item, item.Amount);
                            }
                        }
                    });

                    tamer.Warehouse.EquippedItems.ForEach(item =>
                    {
                        if (item.ItemInfo != null && item.IsTemporary && item.Expired)
                        {
                            if (item.ItemInfo.UseTimeType == 2 || item.ItemInfo.UseTimeType == 3)
                            {
                                item.SetFirstExpired(false);

                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabWarehouse, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Quit));
                            }
                            else
                            {
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabWarehouse, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Remove));
                                tamer.Warehouse.RemoveOrReduceItem(item, item.Amount);
                            }
                        }
                    });

                    tamer.AccountWarehouse?.EquippedItems.ForEach(item =>
                    {
                        if (item.ItemInfo != null && item.IsTemporary && item.Expired)
                        {
                            if (item.ItemInfo.UseTimeType == 2 || item.ItemInfo.UseTimeType == 3)
                            {
                                item.SetFirstExpired(false);

                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabShareStash, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Quit));
                            }
                            else
                            {
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabShareStash, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Remove));
                                tamer.AccountWarehouse.RemoveOrReduceItem(item, item.Amount);
                            }
                        }
                    });

                    tamer.Equipment.EquippedItems.ForEach(item =>
                    {
                        if (item.ItemInfo != null && item.IsTemporary && item.Expired)
                        {
                            if (item.ItemInfo.UseTimeType == 2)
                            {
                                item.SetFirstExpired(false);

                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabEquip, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Quit));
                            }
                            else
                            {
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabEquip, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Remove));
                                tamer.Equipment.RemoveOrReduceItem(item, item.Amount);
                            }
                        }
                    });

                    tamer.ChipSets.EquippedItems.ForEach(item =>
                    {
                        if (item.ItemInfo != null && item.IsTemporary && item.Expired)
                        {
                            if (item.ItemInfo.UseTimeType == 2)
                            {
                                item.SetFirstExpired(false);

                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabChipset, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Quit));
                            }
                            else
                            {
                                client.Send(new ItemExpiredPacket(InventorySlotTypeEnum.TabChipset, item.Slot,
                                    item.ItemId, ExpiredTypeEnum.Remove));
                                tamer.ChipSets.RemoveOrReduceItem(item, item.Amount);
                            }
                        }
                    });

                    _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    _sender.Send(new UpdateItemsCommand(client.Tamer.Warehouse));

                    if (client.Tamer.AccountWarehouse != null)
                    {
                        _sender.Send(new UpdateItemsCommand(client.Tamer.AccountWarehouse));
                    }
                    else
                    {
                        _logger.Debug($"Account warehouse for tamerId: {client.TamerId} with name: {client.Tamer.Name} is null");
                    }

                    _sender.Send(new UpdateItemsCommand(client.Tamer.Equipment));
                    _sender.Send(new UpdateItemsCommand(client.Tamer.ChipSets));
                }

                if (tamer.CheckBuffsTime)
                {
                    tamer.UpdateBuffsCheckTime();

                    if (tamer.BuffList.HasActiveBuffs)
                    {
                        var buffsToRemove = tamer.BuffList.Buffs.Where(x => x.Expired).ToList();

                        buffsToRemove.ForEach(buffToRemove =>
                        {
                            tamer.BuffList.Remove(buffToRemove.BuffId);
                            map.BroadcastForTamerViewsAndSelf(tamer.Id,
                                new RemoveBuffPacket(tamer.GeneralHandler, buffToRemove.BuffId).Serialize());
                        });

                        if (buffsToRemove.Any())
                        {
                            client?.Send(new UpdateStatusPacket(tamer));
                            map.BroadcastForTargetTamers(tamer.Id,
                                new UpdateCurrentHPRatePacket(tamer.GeneralHandler, tamer.HpRate).Serialize());
                            _sender.Send(new UpdateCharacterBuffListCommand(tamer.BuffList));
                        }
                    }

                    if (tamer.Partner.BuffList.HasActiveBuffs)
                    {
                        var buffsToRemove = tamer.Partner.BuffList.Buffs
                            .Where(x => x.Expired)
                            .ToList();


                        buffsToRemove.ForEach(buffToRemove =>
                        {
                            tamer.Partner.BuffList.Remove(buffToRemove.BuffId);
                            map.BroadcastForTamerViewsAndSelf(tamer.Id,
                                new RemoveBuffPacket(tamer.Partner.GeneralHandler, buffToRemove.BuffId).Serialize());
                        });

                        if (buffsToRemove.Any())
                        {
                            client?.Send(new UpdateStatusPacket(tamer));
                            map.BroadcastForTargetTamers(tamer.Id,
                                new UpdateCurrentHPRatePacket(tamer.Partner.GeneralHandler, tamer.Partner.HpRate)
                                    .Serialize());
                            _sender.Send(new UpdateDigimonBuffListCommand(tamer.Partner.BuffList));
                        }
                    }

                    if (tamer.HaveActiveCashSkill)
                    {
                        var buffsToRemove = tamer.ActiveSkill
                            .Where(x => x.Expired && x.SkillId > 0 && x.Type == TamerSkillTypeEnum.Cash)
                            .ToList();

                        buffsToRemove.ForEach(buffToRemove =>
                        {
                            var activeSkill = tamer.ActiveSkill.FirstOrDefault(x => x.Id == buffToRemove.Id);

                            activeSkill.SetTamerSkill(0, 0, TamerSkillTypeEnum.Normal);

                            client?.Send(new ActiveTamerCashSkillExpire(activeSkill.SkillId));

                            _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));
                        });
                    }
                }

                if (tamer.SyncResourcesTime)
                {
                    tamer.UpdateSyncResourcesTime();

                    client?.Send(new UpdateCurrentResourcesPacket(tamer.GeneralHandler, (short)tamer.CurrentHp,
                        (short)tamer.CurrentDs, 0));
                    client?.Send(new UpdateCurrentResourcesPacket(tamer.Partner.GeneralHandler,
                        (short)tamer.Partner.CurrentHp, (short)tamer.Partner.CurrentDs, 0));
                    client?.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));

                    map.BroadcastForTargetTamers(tamer.Id,
                        new UpdateCurrentHPRatePacket(tamer.GeneralHandler, tamer.HpRate).Serialize());
                    map.BroadcastForTargetTamers(tamer.Id,
                        new UpdateCurrentHPRatePacket(tamer.Partner.GeneralHandler, tamer.Partner.HpRate).Serialize());
                    map.BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SyncConditionPacket(tamer.GeneralHandler, tamer.CurrentCondition, tamer.ShopName)
                            .Serialize());

                    var party = _partyManager.FindParty(client.Tamer.Id);
                    if (party != null && party.Members.Count == 1)
                    {
                        BroadcastForTargetTamers(client.Tamer.Id,new PartyMemberKickPacket(0).Serialize());
                        BroadcastForTargetTamers(client.Tamer.Id,new PartyMemberKickPacket(1).Serialize());
                        BroadcastForTargetTamers(client.Tamer.Id,new PartyMemberKickPacket(2).Serialize());
                        BroadcastForTargetTamers(client.Tamer.Id,new PartyMemberKickPacket(3).Serialize());
                        _partyManager.RemoveParty(party.Id);
                    }
                    else if(party != null && party.Members.Count > 1)
                    {
               
                        
                        party.UpdateMember(party[client.Tamer.Id], client.Tamer);

                        map.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberInfoPacket(party[client.Tamer.Id]).Serialize());

                            var memberEntry = party.GetMemberById(client.TamerId);
                        var leaveTargetKey = memberEntry.Value.Key;

                        var currentLeaderEntry = party.GetMemberById(party.LeaderId);
                        client.Send(new PartyMemberListPacket(party, client.Tamer.Id));

                        if (currentLeaderEntry != null)
                        {
                            var currentLeaderKey = currentLeaderEntry.Value.Key;
                            party.LeaderSlot = currentLeaderKey;

                            BroadcastForTargetTamers(party.GetMembersIdList(),
                                new PartyLeaderChangedPacket((int)currentLeaderKey).Serialize());

                        }
                    }

                }

                if (tamer.SaveResourcesTime)
                {
                    tamer.UpdateSaveResourcesTime();

                    var subStopWatch = new Stopwatch();
                    subStopWatch.Start();

                    _sender.Send(new UpdateCharacterBasicInfoCommand(tamer));
                    _sender.Send(new UpdateEvolutionCommand(tamer.Partner.CurrentEvolution));

                    subStopWatch.Stop();

                    if (subStopWatch.ElapsedMilliseconds >= 1500)
                    {
                        Console.WriteLine($"Save resources elapsed time: {subStopWatch.ElapsedMilliseconds}");
                    }
                }

                if (tamer.ResetDailyQuestsTime)
                {
                    tamer.UpdateDailyQuestsSyncTime();

                    var dailyQuestResetTime = _sender.Send(new DailyQuestResetTimeQuery());

                    if (DateTime.Now >= dailyQuestResetTime.Result)
                    {
                        client?.Send(new QuestDailyUpdatePacket());
                    }
                }
            }

            stopwatch.Stop();

            var totalTime = stopwatch.Elapsed.TotalMilliseconds;

            if (totalTime >= 1000)
                Console.WriteLine($"TamersOperation ({map.ConnectedTamers.Count}): {totalTime}.");
        }

        private void GetInViewMobs(GameMap map, CharacterModel tamer)
        {
            List<long> mobsToAdd = new List<long>();
            List<long> mobsToRemove = new List<long>();

            // Criar uma cópia da lista de Mobs
            List<MobConfigModel> mobsCopy = new List<MobConfigModel>(map.Mobs);

            // Iterar sobre a cópia da lista
            mobsCopy.ForEach(mob =>
            {
                if (tamer.TempShowFullMap)
                {
                    if (!tamer.MobsInView.Contains(mob.Id))
                        mobsToAdd.Add(mob.Id);
                }
                else
                {
                    var distanceDifference = UtilitiesFunctions.CalculateDistance(
                        tamer.Location.X,
                        mob.CurrentLocation.X,
                        tamer.Location.Y,
                        mob.CurrentLocation.Y);

                    if (distanceDifference <= _startToSee && !tamer.MobsInView.Contains(mob.Id))
                        mobsToAdd.Add(mob.Id);

                    if (distanceDifference >= _stopSeeing && tamer.MobsInView.Contains(mob.Id))
                        mobsToRemove.Add(mob.Id);
                }
            });

            // Adicionar e remover os IDs de Mob na lista tamer.MobsInView após a iteração
            mobsToAdd.ForEach(id => tamer.MobsInView.Add(id));
            mobsToRemove.ForEach(id => tamer.MobsInView.Remove(id));
        }

        private void GetInViewMobs(GameMap map, CharacterModel tamer, bool Summon)
        {
            List<long> mobsToAdd = new List<long>();
            List<long> mobsToRemove = new List<long>();

            // Criar uma cópia da lista de Mobs
            List<SummonMobModel> mobsCopy = new List<SummonMobModel>(map.SummonMobs);

            // Iterar sobre a cópia da lista
            mobsCopy.ForEach(mob =>
            {
                if (tamer.TempShowFullMap)
                {
                    if (!tamer.MobsInView.Contains(mob.Id))
                        mobsToAdd.Add(mob.Id);
                }
                else
                {
                    var distanceDifference = UtilitiesFunctions.CalculateDistance(
                        tamer.Location.X,
                        mob.CurrentLocation.X,
                        tamer.Location.Y,
                        mob.CurrentLocation.Y);

                    if (distanceDifference <= _startToSee && !tamer.MobsInView.Contains(mob.Id))
                        mobsToAdd.Add(mob.Id);

                    if (distanceDifference >= _stopSeeing && tamer.MobsInView.Contains(mob.Id))
                        mobsToRemove.Add(mob.Id);
                }
            });

            // Adicionar e remover os IDs de Mob na lista tamer.MobsInView após a iteração
            mobsToAdd.ForEach(id => tamer.MobsInView.Add(id));
            mobsToRemove.ForEach(id => tamer.MobsInView.Remove(id));
        }

        /// <summary>
        /// Updates the current partners handler values;
        /// </summary>
        /// <param name="mapId">Current map id</param>
        /// <param name="digimons">Current digimons</param>
        public void SetDigimonHandlers(int mapId, List<DigimonModel> digimons)
        {
            Maps.FirstOrDefault(x => x.MapId == mapId)?.SetDigimonHandlers(digimons);
        }

        // -----------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Swaps the digimons current handler.
        /// </summary>
        /// <param name="mapId">Target map handler manager</param>
        /// <param name="oldPartnerId">Old partner identifier</param>
        /// <param name="newPartner">New partner</param>
        public void SwapDigimonHandlers(int mapId, DigimonModel oldPartner, DigimonModel newPartner)
        {
            Maps.FirstOrDefault(x => x.MapId == mapId)?.SwapDigimonHandlers(oldPartner, newPartner);
        }

        public void SwapDigimonHandlers(int mapId, int channel, DigimonModel oldPartner, DigimonModel newPartner)
        {
            Maps.FirstOrDefault(x => x.MapId == mapId && x.Channel == channel)?.SwapDigimonHandlers(oldPartner, newPartner);
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private async void CheckLocationDebuff(GameClient client)
        {
            if (client.Tamer.DebuffTime)
            {
                client.Tamer.UpdateDebuffTime();

                // Verification for Dark Tower
                if (client.Tamer.Location.MapId == 1109)
                {
                    var mapDebuff = client.Partner.DebuffList.ActiveBuffs.Where(x => x.BuffId == 50101);
                    var evolutionType = _assets.DigimonBaseInfo.First(x => x.Type == client.Partner.CurrentType).EvolutionType;

                    if ((EvolutionRankEnum)evolutionType != EvolutionRankEnum.Rookie && (EvolutionRankEnum)evolutionType != EvolutionRankEnum.Capsule &&
                        (EvolutionRankEnum)evolutionType != EvolutionRankEnum.Spirit)
                    {
                        await Task.Delay(1000);
                        client.Tamer.IsSpecialMapActive = true;

                        client.Tamer.ActiveEvolution.SetDs(0);
                        client.Tamer.ActiveEvolution.SetXg(0);
                    }
                    else
                    {
                        client.Tamer.IsSpecialMapActive = false;
                    }

                    if (mapDebuff != null)
                    {
                        _logger.Information($"Map Debuff Activated !!");

                        foreach (var teste in mapDebuff)
                        {
                            _logger.Information($"Map Debuff ID: {teste.BuffId}");
                        }

                    }

                }

                // Verifica Buff do PvpMap
                var buff1 = client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == 40345);

                if (buff1 != null)
                {
                    client.Tamer.BuffList.Buffs.Remove(buff1);

                    client.Send(new UpdateStatusPacket(client.Tamer));
                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff1.BuffId).Serialize());
                }

                await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
            }

        }

        private async void MapBuff(GameClient client)
        {
            var buff = _assets.BuffInfo.Where(x => x.BuffId == 40327 || x.BuffId == 40350).ToList();

            if (buff != null)
            {
                buff.ForEach(buffAsset =>
                {
                    if (!client.Tamer.BuffList.Buffs.Any(x => x.BuffId == buffAsset.BuffId))
                    {
                        var newCharacterBuff = CharacterBuffModel.Create(buffAsset.BuffId, buffAsset.SkillId, 2592000, 0);

                        newCharacterBuff.SetBuffInfo(buffAsset);

                        client.Tamer.BuffList.Buffs.Add(newCharacterBuff);

                        BroadcastForTamerViewsAndSelf(client.TamerId, new AddBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, 0).Serialize());
                    }
                });

                await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private void ShowOrHideTamer(GameMap map, CharacterModel tamer)
        {
            foreach (var connectedTamer in map.ConnectedTamers.Where(x => x.Id != tamer.Id))
            {
                var distanceDifference = UtilitiesFunctions.CalculateDistance(
                    tamer.Location.X,
                    connectedTamer.Location.X,
                    tamer.Location.Y,
                    connectedTamer.Location.Y);

                if (distanceDifference <= _startToSee)
                    ShowTamer(map, tamer, connectedTamer.Id);
                else if (distanceDifference >= _stopSeeing)
                    HideTamer(map, tamer, connectedTamer.Id);
            }
        }

        private void ShowTamer(GameMap map, CharacterModel tamerToShow, long tamerToSeeId)
        {
            if (!map.ViewingTamer(tamerToShow.Id, tamerToSeeId))
            {
                foreach (var item in tamerToShow.Equipment.EquippedItems.Where(x => x.ItemInfo == null))
                    item?.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item?.ItemId));

                map.ShowTamer(tamerToShow.Id, tamerToSeeId);

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamerToSeeId);
                if (targetClient != null)
                {
                    targetClient.Send(new LoadTamerPacket(tamerToShow));
                    targetClient.Send(new LoadBuffsPacket(tamerToShow));
                    if (tamerToShow.InBattle)
                    {
                        targetClient.Send(new SetCombatOnPacket(tamerToShow.GeneralHandler));
                        targetClient.Send(new SetCombatOnPacket(tamerToShow.Partner.GeneralHandler));
                    }
#if DEBUG
                    var serialized = SerializeShowTamer(tamerToShow);
                    //File.WriteAllText($"Shows\\Show{tamerToShow.Id}To{tamerToSeeId}_{DateTime.Now:dd_MM_yy_HH_mm_ss}.temp", serialized);
#endif
                }
            }
        }

        private void HideTamer(GameMap map, CharacterModel tamerToHide, long tamerToBlindId)
        {
            if (map.ViewingTamer(tamerToHide.Id, tamerToBlindId))
            {
                map.HideTamer(tamerToHide.Id, tamerToBlindId);

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamerToBlindId);

                if (targetClient != null)
                {
                    targetClient.Send(new UnloadTamerPacket(tamerToHide));

#if DEBUG
                    var serialized = SerializeHideTamer(tamerToHide);
                    //File.WriteAllText($"Hides\\Hide{tamerToHide.Id}To{tamerToBlindId}_{DateTime.Now:dd_MM_yy_HH_mm_ss}.temp", serialized);
#endif
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private static string SerializeHideTamer(CharacterModel tamer)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Tamer{tamer.Id}{tamer.Name}");
            sb.AppendLine($"TamerHandler {tamer.GeneralHandler.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.X.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.Y.ToString()}");

            sb.AppendLine($"Partner{tamer.Partner.Id}{tamer.Partner.Name}");
            sb.AppendLine($"PartnerHandler {tamer.Partner.GeneralHandler.ToString()}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.X.ToString()}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.Y.ToString()}");

            return sb.ToString();
        }

        private static string SerializeShowTamer(CharacterModel tamer)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Partner{tamer.Partner.Id}");
            sb.AppendLine($"PartnerName {tamer.Partner.Name}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.X.ToString()}");
            sb.AppendLine($"PartnerLocation {tamer.Partner.Location.Y.ToString()}");
            sb.AppendLine($"PartnerHandler {tamer.Partner.GeneralHandler.ToString()}");
            sb.AppendLine($"PartnerCurrentType {tamer.Partner.CurrentType.ToString()}");
            sb.AppendLine($"PartnerSize {tamer.Partner.Size.ToString()}");
            sb.AppendLine($"PartnerLevel {tamer.Partner.Level.ToString()}");
            sb.AppendLine($"PartnerModel {tamer.Partner.Model.ToString()}");
            sb.AppendLine($"PartnerMS {tamer.Partner.MS.ToString()}");
            sb.AppendLine($"PartnerAS {tamer.Partner.AS.ToString()}");
            sb.AppendLine($"PartnerHPRate {tamer.Partner.HpRate.ToString()}");
            sb.AppendLine($"PartnerCloneTotalLv {tamer.Partner.Digiclone.CloneLevel.ToString()}");
            sb.AppendLine($"PartnerCloneAtLv {tamer.Partner.Digiclone.ATLevel.ToString()}");
            sb.AppendLine($"PartnerCloneBlLv {tamer.Partner.Digiclone.BLLevel.ToString()}");
            sb.AppendLine($"PartnerCloneCtLv {tamer.Partner.Digiclone.CTLevel.ToString()}");
            sb.AppendLine($"PartnerCloneEvLv {tamer.Partner.Digiclone.EVLevel.ToString()}");
            sb.AppendLine($"PartnerCloneHpLv {tamer.Partner.Digiclone.HPLevel.ToString()}");

            sb.AppendLine($"Tamer{tamer.Id}");
            sb.AppendLine($"TamerName {tamer.Name.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.X.ToString()}");
            sb.AppendLine($"TamerLocation {tamer.Location.Y.ToString()}");
            sb.AppendLine($"TamerHandler {tamer.GeneralHandler.ToString()}");
            sb.AppendLine($"TamerModel {tamer.Model.ToString()}");
            sb.AppendLine($"TamerLevel {tamer.Level.ToString()}");
            sb.AppendLine($"TamerMS {tamer.MS.ToString()}");
            sb.AppendLine($"TamerHpRate {tamer.HpRate.ToString()}");
            sb.AppendLine($"TamerEquipment {tamer.Equipment.ToString()}");
            sb.AppendLine($"TamerDigivice {tamer.Digivice.ToString()}");
            sb.AppendLine($"TamerCurrentCondition {tamer.CurrentCondition.ToString()}");
            sb.AppendLine($"TamerSize {tamer.Size.ToString()}");
            sb.AppendLine($"TamerCurrentTitle {tamer.CurrentTitle.ToString()}");
            sb.AppendLine($"TamerSealLeaderId {tamer.SealList.SealLeaderId.ToString()}");

            return sb.ToString();
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private void PartnerAutoAttackPlayer(CharacterModel tamer)
        {
            if (!tamer.Partner.AutoAttack || tamer.Partner.HP < 1)
                return;

            if (!tamer.Partner.IsAttacking && tamer.TargetPartner != null && tamer.TargetPartner.Alive)
            {
                tamer.Partner.SetEndAttacking(tamer.Partner.AS);
                tamer.SetHidden(false);

                if (!tamer.InBattle)
                {
                    _logger.Verbose($"Character {tamer.Id} engaged partner {tamer.TargetPartner.Id} - {tamer.TargetPartner.Name}.");

                    BroadcastForTamerViewsAndSelf(tamer.Id, new SetCombatOnPacket(tamer.Partner.GeneralHandler).Serialize());

                    tamer.StartBattle(tamer.TargetPartner);
                }

                if (!tamer.TargetPartner.Character.InBattle)
                {
                    BroadcastForTamerViewsAndSelf(tamer.Id, new SetCombatOnPacket(tamer.TargetPartner.Character.GeneralHandler).Serialize());

                    tamer.TargetPartner.Character.StartBattle(tamer.Partner);
                }

                var missed = false;

                if (missed)
                {
                    _logger.Warning(
                        $"Partner {tamer.Partner.Id} missed hit on partner {tamer.TargetPartner.Id} - {tamer.TargetPartner.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new MissHitPacket(tamer.Partner.GeneralHandler, tamer.TargetPartner.GeneralHandler)
                            .Serialize());
                }
                else
                {
                    #region Hit Damage

                    var critBonusMultiplier = 0.00;
                    var blocked = false;
                    var finalDmg = CalculateDamagePlayer(tamer, out critBonusMultiplier, out blocked);

                    #endregion

                    if (finalDmg <= 0) finalDmg = 1;
                    if (finalDmg > tamer.TargetPartner.CurrentHp) finalDmg = tamer.TargetPartner.CurrentHp;

                    var newHp = tamer.TargetPartner.ReceiveDamage(finalDmg);

                    var hitType = blocked ? 2 : critBonusMultiplier > 0 ? 1 : 0;

                    if (newHp > 0)
                    {
                        _logger.Warning(
                            $"Partner {tamer.Partner.Id} inflicted {finalDmg} to partner {tamer.TargetPartner?.Id} - {tamer.TargetPartner?.Name}.");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new HitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetPartner.GeneralHandler,
                                finalDmg,
                                tamer.TargetPartner.HP,
                                newHp,
                                hitType).Serialize());
                    }
                    else
                    {
                        _logger.Warning(
                            $"Partner {tamer.Partner.Id} killed partner {tamer.TargetPartner?.Id} - {tamer.TargetPartner?.Name} with {finalDmg} damage.");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new KillOnHitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetPartner.GeneralHandler,
                                finalDmg,
                                hitType).Serialize());

                        tamer.TargetPartner.Character.Die();

                        if (!EnemiesAttacking(tamer.Location.MapId, tamer.Partner.Id, tamer.Id))
                        {
                            tamer.StopBattle();

                            BroadcastForTamerViewsAndSelf(
                                tamer.Id,
                                new SetCombatOffPacket(tamer.Partner.GeneralHandler).Serialize());
                        }
                    }
                }

                tamer.Partner.UpdateLastHitTime();
            }

            bool StopAttackPlayer = tamer.TargetPartner == null || !tamer.TargetPartner.Alive || tamer.Partner.HP < 1;

            if (StopAttackPlayer) tamer.Partner?.StopAutoAttack();
        }

        public void PartnerAutoAttackMob(CharacterModel tamer)
        {
            if (!tamer.Partner.AutoAttack)
                return;

            if (!tamer.Partner.IsAttacking && tamer.TargetMob != null && tamer.TargetMob.Alive & tamer.Partner.Alive)
            {
                tamer.Partner.SetEndAttacking(tamer.Partner.AS);
                tamer.SetHidden(false);

                if (!tamer.InBattle)
                {
                    _logger.Verbose($"Character {tamer.Id} engaged {tamer.TargetMob.Id} - {tamer.TargetMob.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.Partner.GeneralHandler).Serialize());
                    tamer.StartBattle(tamer.TargetMob);
                    tamer.Partner.StartAutoAttack();
                }

                if (!tamer.TargetMob.InBattle)
                {
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.TargetMob.GeneralHandler).Serialize());
                    tamer.TargetMob.StartBattle(tamer);
                    tamer.Partner.StartAutoAttack();
                }

                var missed = false;

                if (!tamer.GodMode)
                {
                    missed = tamer.CanMissHit();
                }

                if (missed)
                {
                    _logger.Verbose(
                        $"Partner {tamer.Partner.Id} missed hit on {tamer.TargetMob.Id} - {tamer.TargetMob.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new MissHitPacket(tamer.Partner.GeneralHandler, tamer.TargetMob.GeneralHandler).Serialize());
                }
                else
                {
                    #region Hit Damage

                    var critBonusMultiplier = 0.00;
                    var blocked = false;
                    var finalDmg = tamer.GodMode
                        ? tamer.TargetMob.CurrentHP
                        : CalculateDamageMob(tamer, out critBonusMultiplier, out blocked);

                    #endregion

                    if (finalDmg <= 0) finalDmg = 1;
                    if (finalDmg > tamer.TargetMob.CurrentHP) finalDmg = tamer.TargetMob.CurrentHP;

                    var newHp = tamer.TargetMob.ReceiveDamage(finalDmg, tamer.Id);

                    var hitType = blocked ? 2 : critBonusMultiplier > 0 ? 1 : 0;

                    if (newHp > 0)
                    {
                        _logger.Verbose(
                            $"Partner {tamer.Partner.Id} inflicted {finalDmg} to mob {tamer.TargetMob?.Id} - {tamer.TargetMob?.Name}({tamer.TargetMob?.Type}).");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new HitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetMob.GeneralHandler,
                                finalDmg,
                                tamer.TargetMob.HPValue,
                                newHp,
                                hitType).Serialize());
                    }
                    else
                    {
                        _logger.Verbose(
                            $"Partner {tamer.Partner.Id} killed mob {tamer.TargetMob?.Id} - {tamer.TargetMob?.Name}({tamer.TargetMob?.Type}) with {finalDmg} damage.");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new KillOnHitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetMob.GeneralHandler,
                                finalDmg,
                                hitType).Serialize());

                        tamer.TargetMob?.Die();

                        if (!MobsAttacking(tamer.Location.MapId, tamer.Id))
                        {
                            tamer.StopBattle();

                            BroadcastForTamerViewsAndSelf(
                                tamer.Id,
                                new SetCombatOffPacket(tamer.Partner.GeneralHandler).Serialize());
                        }
                    }
                }

                tamer.Partner.UpdateLastHitTime();
            }

            bool StopAttackMob = tamer.TargetMob == null || tamer.TargetMob.Dead;

            if (StopAttackMob) tamer.Partner?.StopAutoAttack();
        }

        public void PartnerAutoAttackSummon(CharacterModel tamer)
        {
            if (!tamer.Partner.AutoAttack)
                return;

            if (!tamer.Partner.IsAttacking && tamer.TargetSummonMob != null &&
                tamer.TargetSummonMob.Alive & tamer.Partner.Alive)
            {
                tamer.Partner.SetEndAttacking(tamer.Partner.AS);
                tamer.SetHidden(false);

                if (!tamer.InBattle)
                {
                    _logger.Verbose(
                        $"Character {tamer.Id} engaged {tamer.TargetSummonMob.Id} - {tamer.TargetSummonMob.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.Partner.GeneralHandler).Serialize());
                    tamer.StartBattle(tamer.TargetMob);
                    tamer.Partner.StartAutoAttack();
                }

                if (!tamer.TargetSummonMob.InBattle)
                {
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.TargetSummonMob.GeneralHandler).Serialize());
                    tamer.TargetSummonMob.StartBattle(tamer);
                    tamer.Partner.StartAutoAttack();
                }

                var missed = false;

                if (!tamer.GodMode)
                {
                    missed = tamer.CanMissHit(true);
                }

                if (missed)
                {
                    _logger.Verbose(
                        $"Partner {tamer.Partner.Id} missed hit on {tamer.TargetSummonMob.Id} - {tamer.TargetSummonMob.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new MissHitPacket(tamer.Partner.GeneralHandler, tamer.TargetSummonMob.GeneralHandler)
                            .Serialize());
                }
                else
                {
                    #region Hit Damage

                    var critBonusMultiplier = 0.00;
                    var blocked = false;
                    var finalDmg = tamer.GodMode
                        ? tamer.TargetSummonMob.CurrentHP
                        : CalculateDamageSummon(tamer, out critBonusMultiplier, out blocked);

                    #endregion

                    if (finalDmg <= 0) finalDmg = 1;
                    if (finalDmg > tamer.TargetSummonMob.CurrentHP) finalDmg = tamer.TargetSummonMob.CurrentHP;

                    var newHp = tamer.TargetSummonMob.ReceiveDamage(finalDmg, tamer.Id);

                    var hitType = blocked ? 2 : critBonusMultiplier > 0 ? 1 : 0;

                    if (newHp > 0)
                    {
                        _logger.Verbose(
                            $"Partner {tamer.Partner.Id} inflicted {finalDmg} to mob {tamer.TargetSummonMob?.Id} - {tamer.TargetSummonMob?.Name}({tamer.TargetSummonMob?.Type}).");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new HitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetSummonMob.GeneralHandler,
                                finalDmg,
                                tamer.TargetSummonMob.HPValue,
                                newHp,
                                hitType).Serialize());
                    }
                    else
                    {
                        _logger.Verbose(
                            $"Partner {tamer.Partner.Id} killed mob {tamer.TargetSummonMob?.Id} - {tamer.TargetSummonMob?.Name}({tamer.TargetSummonMob?.Type}) with {finalDmg} damage.");

                        BroadcastForTamerViewsAndSelf(
                            tamer.Id,
                            new KillOnHitPacket(
                                tamer.Partner.GeneralHandler,
                                tamer.TargetSummonMob.GeneralHandler,
                                finalDmg,
                                hitType).Serialize());

                        tamer.TargetSummonMob?.Die();

                        if (!MobsAttacking(tamer.Location.MapId, tamer.Id))
                        {
                            tamer.StopBattle(true);

                            BroadcastForTamerViewsAndSelf(
                                tamer.Id,
                                new SetCombatOffPacket(tamer.Partner.GeneralHandler).Serialize());
                        }
                    }
                }

                tamer.Partner.UpdateLastHitTime();
            }

            bool StopAttackSummon = tamer.TargetSummonMob == null || tamer.TargetSummonMob.Dead;

            if (StopAttackSummon) tamer.Partner?.StopAutoAttack();
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private ReceiveExpResult ReceiveBonusTamerExp(CharacterModel tamer,long totalTamerExp)
        {
            var tamerResult = _expManager.ReceiveTamerExperience(totalTamerExp,tamer);

            if (tamerResult.LevelGain > 0)
            {
                BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler,tamer.Level).Serialize());

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
        private ReceiveExpResult ReceiveTamerExp(CharacterModel tamer,long tamerExpToReceive)
        {
            var tamerResult = _expManager.ReceiveTamerExperience(tamerExpToReceive,tamer);

            if (tamerResult.LevelGain > 0)
            {
                BroadcastForTamerViewsAndSelf(tamer.Id,
                    new LevelUpPacket(tamer.GeneralHandler,tamer.Level).Serialize());

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


        private ReceiveExpResult ReceiveBonusPartnerExp(DigimonModel partner,MobConfigModel targetMob,long totalPartnerExp)
        {
            var partnerResult = _expManager.ReceiveDigimonExperience(totalPartnerExp,partner);

            if (partnerResult.LevelGain > 0)
            {
                partner.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        partner.CurrentType,
                        partner.Level,
                        partner.Size
                    )
                );

                BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler,partner.Level).Serialize());

                partner.FullHeal();
            }
            return partnerResult;
        }

        private ReceiveExpResult ReceivePartnerExp(GameClient client,DigimonModel partner,MobConfigModel targetMob,long partnerExpToReceive)
        {
            var attributeExp = partner.GetAttributeExperience();
            var elementExp = partner.GetElementExperience();

            var partnerResult = _expManager.ReceiveDigimonExperience(partnerExpToReceive,partner);

            if (attributeExp < 10000) _expManager.ReceiveAttributeExperience(client,partner,targetMob.Attribute,targetMob.ExpReward);
            if (elementExp < 10000) _expManager.ReceiveElementExperience(client,partner,targetMob.Element,targetMob.ExpReward);


            partner.ReceiveSkillExp(targetMob.ExpReward.SkillExperience);

            if (partnerResult.LevelGain > 0)
            {
                partner.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        partner.CurrentType,
                        partner.Level,
                        partner.Size
                    )
                );

                BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler,partner.Level).Serialize());

                partner.FullHeal();
            }

            return partnerResult;
        }

        private ReceiveExpResult ReceivePartnerExp(GameClient client,DigimonModel partner,SummonMobModel targetMob,long partnerExpToReceive)
        {
            var attributeExp = partner.GetAttributeExperience();
            var elementExp = partner.GetElementExperience();
            var partnerResult = _expManager.ReceiveDigimonExperience(partnerExpToReceive,partner);

            if (attributeExp < 10000) _expManager.ReceiveAttributeExperience(client,partner,targetMob.Attribute,targetMob.ExpReward);
            if (elementExp < 10000) _expManager.ReceiveElementExperience(client,partner,targetMob.Element,targetMob.ExpReward);

            partner.ReceiveSkillExp(targetMob.ExpReward.SkillExperience);

            if (partnerResult.LevelGain > 0)
            {
                partner.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        partner.CurrentType,
                        partner.Level,
                        partner.Size
                    )
                );

                BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler,partner.Level).Serialize());

                partner.FullHeal();
            }

            return partnerResult;
        }
        private ReceiveExpResult ReceiveBonusPartnerExp(DigimonModel partner,SummonMobModel targetMob,long totalPartnerExp)
        {
            var partnerResult = _expManager.ReceiveDigimonExperience(totalPartnerExp,partner);

            if (partnerResult.LevelGain > 0)
            {
                partner.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        partner.CurrentType,
                        partner.Level,
                        partner.Size
                    )
                );

                BroadcastForTamerViewsAndSelf(partner.Character.Id,
                    new LevelUpPacket(partner.GeneralHandler,partner.Level).Serialize());

                partner.FullHeal();
            }
            return partnerResult;
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private static int CalculateDamagePlayer(CharacterModel tamer, out double critBonusMultiplier, out bool blocked)
        {
            var baseDamage = (tamer.Partner.AT / tamer.TargetPartner.DE * 150) + UtilitiesFunctions.RandomInt(5, 50);
            if (baseDamage < 0) baseDamage = 0;

            critBonusMultiplier = 0.00;
            double critChance = tamer.Partner.CC / 100;

            if (critChance >= UtilitiesFunctions.RandomDouble())
            {
                var vlrAtual = tamer.Partner.CD;
                var bonusMax = 1.00; //TODO: externalizar?
                var expMax = 10000; //TODO: externalizar?

                critBonusMultiplier = (bonusMax * vlrAtual) / expMax;
            }

            blocked = tamer.TargetPartner.BL >= UtilitiesFunctions.RandomDouble();

            var levelBonusMultiplier = tamer.Partner.Level > tamer.TargetPartner.Level
                ? (0.01f * (tamer.Partner.Level - tamer.TargetPartner.Level))
                : 0; //TODO: externalizar no portal

            var attributeMultiplier = 0.00;

            if (tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(tamer.TargetPartner.BaseInfo.Attribute))
            {
                var vlrAtual = tamer.Partner.GetAttributeExperience();
                var bonusMax = 1.00; //TODO: externalizar?
                var expMax = 10000; //TODO: externalizar?

                attributeMultiplier = (bonusMax * vlrAtual) / expMax;
            }
            else if (tamer.TargetPartner.BaseInfo.Attribute.HasAttributeAdvantage(tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            var elementMultiplier = 0.00;

            if (tamer.Partner.BaseInfo.Element.HasElementAdvantage(tamer.TargetPartner.BaseInfo.Element))
            {
                var vlrAtual = tamer.Partner.GetElementExperience();
                var bonusMax = 0.5; //TODO: externalizar?
                var expMax = 10000; //TODO: externalizar?

                elementMultiplier = (bonusMax * vlrAtual) / expMax;
            }
            else if (tamer.TargetPartner.BaseInfo.Element.HasElementAdvantage(tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.50;
            }

            baseDamage /= blocked ? 2 : 1;

            return (int)Math.Floor(baseDamage +
                                   (baseDamage * critBonusMultiplier) +
                                   (baseDamage * levelBonusMultiplier) +
                                   (baseDamage * attributeMultiplier) +
                                   (baseDamage * elementMultiplier));
        }

        private static int CalculateDamageMob(CharacterModel tamer, out double critBonusMultiplier, out bool blocked)
        {
            int baseDamage = tamer.Partner.AT - tamer.TargetMob.DEValue;

            if (baseDamage < tamer.Partner.AT * 0.5) // If Damage is less than 50% of AT
            {
                baseDamage = (int)(tamer.Partner.AT * 0.9); // give 90% of AT as Damage
            }

            // -------------------------------------------------------------------------------

            critBonusMultiplier = 0.00;
            double critChance = tamer.Partner.CC / 100;

            if (critChance >= UtilitiesFunctions.RandomDouble())
            {
                blocked = false;

                var critDamageMultiplier = tamer.Partner.CD / 100.0;
                critBonusMultiplier = baseDamage * (critDamageMultiplier / 100);
            }

            if (tamer.TargetMob != null)
            {
                blocked = tamer.TargetMob.BLValue >= UtilitiesFunctions.RandomDouble();
            }
            else
            {
                blocked = false;
                return 0;
            }

            // -------------------------------------------------------------------------------

            // Level Diference
            var levelBonusMultiplier = 0;
            //var levelDifference = client.Tamer.Partner.Level - targetMob.Level;
            //var levelBonusMultiplier = levelDifference > 0 ? levelDifference * 0.02 : levelDifference * 0.01;

            // Attribute
            var attributeMultiplier = 0.00;
            if (tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(tamer.TargetMob.Attribute))
            {
                var attExp = tamer.Partner.GetAttributeExperience();
                var attValue = tamer.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (tamer.TargetMob.Attribute.HasAttributeAdvantage(tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            // Element
            var elementMultiplier = 0.00;
            if (tamer.Partner.BaseInfo.Element.HasElementAdvantage(tamer.TargetMob.Element))
            {
                var vlrAtual = tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * vlrAtual) / expMax;
            }
            else if (tamer.TargetMob.Element.HasElementAdvantage(tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.25;
            }

            // -------------------------------------------------------------------------------

            if (blocked)
                baseDamage /= 2;

            return (int)Math.Max(1, Math.Floor(baseDamage + critBonusMultiplier +
                                               (baseDamage * levelBonusMultiplier) +
                                               (baseDamage * attributeMultiplier) + (baseDamage * elementMultiplier)));
        }

        private static int CalculateDamageSummon(CharacterModel tamer, out double critBonusMultiplier, out bool blocked)
        {
            int baseDamage = tamer.Partner.AT - tamer.TargetSummonMob.DEValue;

            if (baseDamage < tamer.Partner.AT * 0.5) // If Damage is less than 50% of AT
            {
                baseDamage = (int)(tamer.Partner.AT * 0.9); // give 90% of AT as Damage
            }

            // -------------------------------------------------------------------------------

            critBonusMultiplier = 0.00;
            double critChance = tamer.Partner.CC / 100;

            if (critChance >= UtilitiesFunctions.RandomDouble())
            {
                blocked = false;

                var critDamageMultiplier = tamer.Partner.CD / 100.0;
                critBonusMultiplier = baseDamage * (critDamageMultiplier / 100);
            }

            if (tamer.TargetSummonMob != null)
            {
                blocked = tamer.TargetSummonMob.BLValue >= UtilitiesFunctions.RandomDouble();
            }
            else
            {
                blocked = false;
                return 0;
            }

            // -------------------------------------------------------------------------------

            // Level
            var levelBonusMultiplier = 0;

            // Atributte
            var attributeMultiplier = 0.00;
            if (tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(tamer.TargetSummonMob.Attribute))
            {
                var attExp = tamer.Partner.GetAttributeExperience();
                var attValue = tamer.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (tamer.TargetSummonMob.Attribute.HasAttributeAdvantage(tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            // Element
            var elementMultiplier = 0.00;
            if (tamer.Partner.BaseInfo.Element.HasElementAdvantage(tamer.TargetSummonMob.Element))
            {
                var vlrAtual = tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * vlrAtual) / expMax;
            }
            else if (tamer.TargetSummonMob.Element.HasElementAdvantage(tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.25;
            }

            // -------------------------------------------------------------------------------

            if (blocked)
                baseDamage /= 2;

            return (int)Math.Max(1, Math.Floor(baseDamage + critBonusMultiplier +
                                               (baseDamage * levelBonusMultiplier) +
                                               (baseDamage * attributeMultiplier) + (baseDamage * elementMultiplier)));
        }

        // -----------------------------------------------------------------------------------------------------------------------

        private async void CheckMonthlyReward(GameClient client)
        {
            var attendenceReward = client.Tamer.AttendanceReward;
            if (attendenceReward.LastRewardDate.Day == attendenceReward.TotalDays)
            {
                return;
            }
            else if (attendenceReward.LastRewardDate.Day > attendenceReward.TotalDays)
            {
                client.Tamer.AttendanceReward.SetLastRewardDate();
                client.Tamer.AttendanceReward.IncreaseTotalDays();
                ReedemReward(client);

                client.Send(new TamerAttendancePacket(attendenceReward.TotalDays));
            }
            else
            {
                await  _sender.Send(new UpdateTamerAttendanceRewardCommand(client.Tamer.AttendanceReward));

            }
        }

        private async void CheckTimeReward(GameClient client)
        {
            if (client.Tamer.TimeReward.ReedemRewards)
            {
                _logger.Debug($"Reward Index: {client.Tamer.TimeReward.RewardIndex}");
                client.Tamer.TimeReward.SetStartTime();
                await _sender.Send(new UpdateTamerAttendanceTimeRewardCommand(client.Tamer.TimeReward));
            }

            if (client.Tamer.TimeReward.RewardIndex <= TimeRewardIndexEnum.Fourth)
            {
                if (client.Tamer.TimeReward.CurrentTime == 0)
                {
                    client.Tamer.TimeReward.CurrentTime = client.Tamer.TimeReward.AtualTime;
                }

                if (DateTime.Now >= client.Tamer.TimeReward.LastTimeRewardUpdate)
                {
                    //_logger.Information($"CurrentTime: {client.Tamer.TimeReward.CurrentTime} | AtualTime: {client.Tamer.TimeReward.AtualTime}");

                    client.Tamer.TimeReward.CurrentTime++;
                    client.Tamer.TimeReward.UpdateCounter++;
                    client.Tamer.TimeReward.SetAtualTime();

                    if (client.Tamer.TimeReward.TimeCompleted())
                    {
                        ReedemTimeReward(client);
                        client.Tamer.TimeReward.RewardIndex++;
                        client.Tamer.TimeReward.CurrentTime = 0;
                        client.Tamer.TimeReward.SetAtualTime();

                        await _sender.Send(new UpdateTamerAttendanceTimeRewardCommand(client.Tamer.TimeReward));
                    }
                    else if (client.Tamer.TimeReward.UpdateCounter >= 5)
                    {
                        await _sender.Send(new UpdateTamerAttendanceTimeRewardCommand(client.Tamer.TimeReward));
                        client.Tamer.TimeReward.UpdateCounter = 0;
                    }

                    client.Send(new TimeRewardPacket(client.Tamer.TimeReward));
                }

                client.Tamer.TimeReward.SetLastTimeRewardDate();
            }
            else
            {
                client.Tamer.TimeReward.RewardIndex = TimeRewardIndexEnum.Ended;
                await _sender.Send(new UpdateTamerAttendanceTimeRewardCommand(client.Tamer.TimeReward));
            }
        }

        private void ReedemTimeReward(GameClient client)
        {
            var reward = new ItemModel();

            switch (client.Tamer.TimeReward.RewardIndex)
            {
                case TimeRewardIndexEnum.First:
                    {
                        var GetPrizes = _assets.TimeRewardAssets
                            .Where(drop => drop.CurrentReward == (int)TimeRewardIndexEnum.First).ToList();

                        GetPrizes.ForEach(drop =>
                        {
                            reward.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == drop.ItemId));
                            reward.ItemId = drop.ItemId;
                            reward.Amount = drop.ItemCount;

                            if (reward.IsTemporary)
                                reward.SetRemainingTime((uint)reward.ItemInfo.UsageTimeMinutes);

                            if (client.Tamer.Inventory.AddItem(reward))
                            {
                                client.Send(new ReceiveItemPacket(reward, InventoryTypeEnum.Inventory));
                                _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                        });
                    }
                    break;

                case TimeRewardIndexEnum.Second:
                    {
                        var GetPrizes = _assets.TimeRewardAssets
                            .Where(drop => drop.CurrentReward == (int)TimeRewardIndexEnum.Second).ToList();

                        GetPrizes.ForEach(drop =>
                        {
                            reward.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == drop.ItemId));
                            reward.ItemId = drop.ItemId;
                            reward.Amount = drop.ItemCount;

                            if (reward.IsTemporary)
                                reward.SetRemainingTime((uint)reward.ItemInfo.UsageTimeMinutes);

                            if (client.Tamer.Inventory.AddItem(reward))
                            {
                                client.Send(new ReceiveItemPacket(reward, InventoryTypeEnum.Inventory));
                                _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                        });
                    }
                    break;

                case TimeRewardIndexEnum.Third:
                    {
                        var GetPrizes = _assets.TimeRewardAssets
                            .Where(drop => drop.CurrentReward == (int)TimeRewardIndexEnum.Third).ToList();

                        GetPrizes.ForEach(drop =>
                        {
                            reward.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == drop.ItemId));
                            reward.ItemId = drop.ItemId;
                            reward.Amount = drop.ItemCount;

                            if (reward.IsTemporary)
                                reward.SetRemainingTime((uint)reward.ItemInfo.UsageTimeMinutes);

                            if (client.Tamer.Inventory.AddItem(reward))
                            {
                                client.Send(new ReceiveItemPacket(reward, InventoryTypeEnum.Inventory));
                                _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                        });
                    }
                    break;

                case TimeRewardIndexEnum.Fourth:
                    {
                        var GetPrizes = _assets.TimeRewardAssets
                            .Where(drop => drop.CurrentReward == (int)TimeRewardIndexEnum.Fourth).ToList();

                        GetPrizes.ForEach(drop =>
                        {
                            reward.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == drop.ItemId));
                            reward.ItemId = drop.ItemId;
                            reward.Amount = drop.ItemCount;

                            if (reward.IsTemporary)
                                reward.SetRemainingTime((uint)reward.ItemInfo.UsageTimeMinutes);

                            if (client.Tamer.Inventory.AddItem(reward))
                            {
                                client.Send(new ReceiveItemPacket(reward, InventoryTypeEnum.Inventory));
                                _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                        });
                    }
                    break;

                default:
                    break;
            }
        }

        private void ReedemReward(GameClient client)
        {
            var rewardInfo =
                _assets.MonthlyEvents.FirstOrDefault(x => x.CurrentDay == client.Tamer.AttendanceReward.TotalDays);

            if (rewardInfo != null)
            {
                var newItem = new ItemModel();
                newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == rewardInfo.ItemId));

                if (newItem.ItemInfo == null)
                {
                    _logger.Warning($"No item info found with ID {rewardInfo.ItemId} for tamer {client.TamerId}.");
                    client.Send(
                        new SystemMessagePacket($"No item info found with ID {rewardInfo.ItemId} by MonthEvent"));
                    return;
                }

                newItem.ItemId = rewardInfo.ItemId;
                newItem.Amount = rewardInfo.ItemCount;

                if (newItem.IsTemporary)
                    newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                newItem.EndDate = DateTime.Now.AddDays(7);

                var itemClone = (ItemModel)newItem.Clone();

                if (client.Tamer.GiftWarehouse.AddItemGiftStorage(newItem))
                {
                    _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                    client.Send(new SystemMessagePacket(
                        $"Added {newItem.ItemInfo.Name} x{newItem.Amount} to GiftStorage by MonthEvent"));
                }

                _sender.Send(new UpdateTamerAttendanceRewardCommand(client.Tamer.AttendanceReward));
            }
        }
    }
}