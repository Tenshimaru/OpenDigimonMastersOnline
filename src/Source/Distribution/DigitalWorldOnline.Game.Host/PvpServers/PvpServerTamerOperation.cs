using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using System.Diagnostics;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class PvpServer
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

                GetInViewMobs(map, tamer);

                ShowOrHideTamer(map, tamer);

                //RemoveMapBuff(client);
                //PvpMapBuff(client);

                if (client.Tamer.GodMode)
                    client.Tamer.SetGodMode(false);

                if (tamer.TargetMobs.Count > 0)
                    PartnerAutoAttackMob(tamer);

                if (tamer.TargetPartners.Count > 0)
                {
                    _logger.Information($"Digimon Target found");

                    if (tamer.Partner.Level <= 25 || tamer.TargetPartner.Level <= 25)
                    {
                        _logger.Information($"You cant attack noobs !!");
                    }
                    else
                    {
                        PartnerAutoAttackPlayer(tamer);
                    }
                }

                tamer.AutoRegen();
                tamer.ActiveEvolutionReduction();

                if (tamer.Riding)
                {
                    tamer.StopRideMode();

                    BroadcastForTamerViewsAndSelf(tamer.Id, new UpdateMovementSpeedPacket(tamer).Serialize());
                    BroadcastForTamerViewsAndSelf(tamer.Id, new RideModeStopPacket(tamer.GeneralHandler, tamer.Partner.GeneralHandler).Serialize());
                }

                if (tamer.BreakEvolution)
                {
                    tamer.ActiveEvolution.SetDs(0);
                    tamer.ActiveEvolution.SetXg(0);

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

                    tamer.Partner.SetBaseInfo(
                        _statusManager.GetDigimonBaseInfo(
                            tamer.Partner.CurrentType
                        )
                    );

                    tamer.Partner.SetBaseStatus(
                        _statusManager.GetDigimonBaseStatus(
                            tamer.Partner.CurrentType,
                            tamer.Partner.Level,
                            tamer.Partner.Size
                        )
                    );

                    client.Partner.AdjustHpAndDs(currentHp, currentMaxHp, currentDs, currentMaxDs);
                    client.Send(new UpdateStatusPacket(tamer));

                    _sender.Send(new UpdatePartnerCurrentTypeCommand(client.Partner));
                    _sender.Send(new UpdateCharacterActiveEvolutionCommand(tamer.ActiveEvolution));
                }

                if (tamer.CheckBuffsTime)
                {
                    tamer.UpdateBuffsCheckTime();

                    if (tamer.BuffList.HasActiveBuffs)
                    {
                        var buffsToRemove = tamer.BuffList.Buffs.Where(x => x.Expired).ToList();

                        buffsToRemove.ForEach(buffToRemove =>
                        { map.BroadcastForTamerViewsAndSelf(tamer.Id, new RemoveBuffPacket(tamer.GeneralHandler, buffToRemove.BuffId).Serialize()); });

                        if (buffsToRemove.Any())
                        {
                            client?.Send(new UpdateStatusPacket(tamer));
                            map.BroadcastForTargetTamers(map.TamersView[tamer.Id], new UpdateCurrentHPRatePacket(tamer.GeneralHandler, tamer.HpRate).Serialize());
                            _sender.Send(new UpdateCharacterBuffListCommand(tamer.BuffList));
                        }
                    }

                    if (tamer.Partner.BuffList.HasActiveBuffs)
                    {
                        var buffsToRemove = tamer.Partner.BuffList.Buffs
                            .Where(x => x.Expired)
                            .ToList();

                        buffsToRemove.ForEach(buffToRemove =>
                        { map.BroadcastForTamerViewsAndSelf(tamer.Id, new RemoveBuffPacket(tamer.Partner.GeneralHandler, buffToRemove.BuffId).Serialize()); });

                        if (buffsToRemove.Any())
                        {
                            client?.Send(new UpdateStatusPacket(tamer));
                            map.BroadcastForTargetTamers(map.TamersView[tamer.Id], new UpdateCurrentHPRatePacket(tamer.Partner.GeneralHandler, tamer.Partner.HpRate).Serialize());

                            _sender.Send(new UpdateDigimonBuffListCommand(tamer.Partner.BuffList));
                        }
                    }
                }

                if (tamer.SyncResourcesTime)
                {
                    tamer.UpdateSyncResourcesTime();

                    client?.Send(new UpdateCurrentResourcesPacket(tamer.GeneralHandler, (short)tamer.CurrentHp, (short)tamer.CurrentDs, 0));
                    client?.Send(new UpdateCurrentResourcesPacket(tamer.Partner.GeneralHandler, (short)tamer.Partner.CurrentHp, (short)tamer.Partner.CurrentDs, 0));
                    map.BroadcastForTargetTamers(map.TamersView[tamer.Id], new UpdateCurrentHPRatePacket(tamer.GeneralHandler, tamer.HpRate).Serialize());
                    map.BroadcastForTargetTamers(map.TamersView[tamer.Id], new UpdateCurrentHPRatePacket(tamer.Partner.GeneralHandler, tamer.Partner.HpRate).Serialize());
                    map.BroadcastForTamerViewsAndSelf(tamer.Id, new SyncConditionPacket(tamer.GeneralHandler, tamer.CurrentCondition, tamer.ShopName).Serialize());
                }

                if (tamer.SaveResourcesTime)
                {
                    tamer.UpdateSaveResourcesTime();

                    var subStopWatch = new Stopwatch();
                    subStopWatch.Start();

                    _sender.Send(new UpdateCharacterLocationCommand(tamer.Location));
                    _sender.Send(new UpdateDigimonLocationCommand(tamer.Partner.Location));

                    _sender.Send(new UpdateCharacterBasicInfoCommand(tamer));

                    _sender.Send(new UpdateCharacterBasicInfoCommand(tamer));
                    _sender.Send(new UpdateEvolutionCommand(tamer.Partner.CurrentEvolution));

                    //_sender.Send(new UpdateCharacterProgressCommand(tamer.Progress));

                    subStopWatch.Stop();

                    if (subStopWatch.ElapsedMilliseconds >= 1500)
                    {
                        Console.WriteLine($"Save resources elapsed time: {subStopWatch.ElapsedMilliseconds}");
                    }
                }
            }

            stopwatch.Stop();

            var totalTime = stopwatch.Elapsed.TotalMilliseconds;

            if (totalTime >= 1000)
                Console.WriteLine($"TamersOperation ({map.ConnectedTamers.Count}): {totalTime}.");
        }

        // ---------------------------------------------------------------------------------------------------

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

        // ---------------------------------------------------------------------------------------------------

        public void SwapDigimonHandlers(int mapId, DigimonModel oldPartner, DigimonModel newPartner)
        {
            Maps.FirstOrDefault(x => x.MapId == mapId)?.SwapDigimonHandlers(oldPartner, newPartner);
        }

        public void SwapDigimonHandlers(int mapId, int channel, DigimonModel oldPartner, DigimonModel newPartner)
        {
            Maps.FirstOrDefault(x => x.MapId == mapId && x.Channel == channel)?.SwapDigimonHandlers(oldPartner, newPartner);
        }

        // ---------------------------------------------------------------------------------------------------

        private void ShowOrHideTamer(GameMap map, CharacterModel tamer)
        {
            foreach (var connectedTamer in map.ConnectedTamers.Where(x => x.Id != tamer.Id))
            {
                ShowTamer(map, tamer, connectedTamer.Id);
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
                    if (tamerToShow.InBattle)
                    {
                        targetClient.Send(new SetCombatOnPacket(tamerToShow.GeneralHandler));
                        targetClient.Send(new SetCombatOnPacket(tamerToShow.Partner.GeneralHandler));
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------

        private async void RemoveMapBuff(GameClient client)
        {
            var buff1 = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == 40327);
            var buff2 = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == 40350);

            if (buff1 != null)
            {
                var characterBuff1 = client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff1.BuffId);

                if (characterBuff1 != null)
                {
                    client.Tamer.BuffList.Buffs.Remove(characterBuff1);

                    client.Send(new UpdateStatusPacket(client.Tamer));
                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff1.BuffId).Serialize());
                }
            }

            if (buff2 != null)
            {
                var characterBuff2 = client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff2.BuffId);

                if (characterBuff2 != null)
                {
                    client.Tamer.BuffList.Buffs.Remove(characterBuff2);

                    client.Send(new UpdateStatusPacket(client.Tamer));
                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff2.BuffId).Serialize());
                }
            }

            await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
        }

        private async void PvpMapBuff(GameClient client)
        {
            var buff = _assets.BuffInfo.Where(x => x.BuffId == 40345).ToList();

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

        // ---------------------------------------------------------------------------------------------------

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
                    _logger.Verbose(
                        $"Character {tamer.Id} engaged partner {tamer.TargetPartner.Id} - {tamer.TargetPartner.Name}.");
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.Partner.GeneralHandler).Serialize());
                    tamer.StartBattle(tamer.TargetPartner);
                }

                if (!tamer.TargetPartner.Character.InBattle)
                {
                    BroadcastForTamerViewsAndSelf(tamer.Id,
                        new SetCombatOnPacket(tamer.TargetPartner.Character.GeneralHandler).Serialize());
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
                        _logger.Warning($"Partner {tamer.Partner.Id} killed partner {tamer.TargetPartner?.Id} - {tamer.TargetPartner?.Name} with {finalDmg} damage.");

                        BroadcastForTamerViewsAndSelf(tamer.Id,
                            new KillOnHitPacket(
                                tamer.Partner.GeneralHandler, tamer.TargetPartner.GeneralHandler,
                                finalDmg, hitType).Serialize());

                        tamer.TargetPartner.Character.Die();

                        if (!EnemiesAttacking(tamer.Location.MapId, tamer.Partner.Id, tamer.Id))
                        {
                            tamer.StopBattle();

                            BroadcastForTamerViewsAndSelf(tamer.Id, new SetCombatOffPacket(tamer.Partner.GeneralHandler).Serialize());
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
                        _logger.Verbose($"Partner {tamer.Partner.Id} killed mob {tamer.TargetMob?.Id} - {tamer.TargetMob?.Name}({tamer.TargetMob?.Type}) with {finalDmg} damage.");

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

        // ---------------------------------------------------------------------------------------------------

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

        // ---------------------------------------------------------------------------------------------------

    }
}