using Microsoft.Extensions.Configuration;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;
using static DigitalWorldOnline.Commons.Packets.GameServer.AddBuffPacket;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Models;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using DigitalWorldOnline.Commons.Enums.Map;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public partial class PartnerSkillPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerSkill;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public PartnerSkillPacketProcessor(AssetsLoader assets, MapServer mapServer, DungeonsServer dungeonServer, EventServer eventServer, PvpServer pvpServer,
            ILogger logger, ISender sender, IConfiguration configuration)
        {
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _configuration = configuration;
        }

        public Task Process(GameClient client,byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var skillSlot = packet.ReadByte();
            var attackerHandler = packet.ReadInt();
            var targetHandler = packet.ReadInt();

            if (client.Partner == null)
                return Task.CompletedTask;

            var skill = _assets.DigimonSkillInfo.FirstOrDefault(x => x.Type == client.Partner.CurrentType && x.Slot == skillSlot);

            if (skill == null || skill.SkillInfo == null)
            {
                _logger.Error($"Skill not found !!");
                return Task.CompletedTask;
            }

            var targetSummonMobs = new List<SummonMobModel>();
            SkillTypeEnum skillType;

            if (client.PvpMap)
            {
                var areaOfEffect = skill.SkillInfo.AreaOfEffect;
                var range = skill.SkillInfo.Range;
                var targetType = skill.SkillInfo.Target;

                // PVP SERVER -> ATTACK MOB
                if (_pvpServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId) != null)
                {
                    var mobTarget = _pvpServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                    if (mobTarget == null || client.Partner == null)
                        return Task.CompletedTask;

                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _pvpServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _pvpServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _pvpServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg != 0)
                                {
                                    finalDmg = DebuffReductionDamage(client,finalDmg);
                                }

                                if (finalDmg <= 0) finalDmg = 1;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());


                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_pvpServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }

                }
                // PVP SERVER -> ATTACK PLAYER
                else if (_pvpServer.GetEnemyByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId) != null)
                {
                    _logger.Information($"Getting digimon target !!");

                    var pvpPartner = _pvpServer.GetEnemyByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _pvpServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _pvpServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _pvpServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg != 0)
                                {
                                    finalDmg = DebuffReductionDamage(client,finalDmg);
                                }

                                if (finalDmg <= 0) finalDmg = 1;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());


                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_pvpServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }

                }
                // MAP SERVER -> ATTACK MOB
                else if (_mapServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId) != null)
                {
                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _mapServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _mapServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else if (areaOfEffect == 0 && targetType == 80)
                    {
                        skillType = SkillTypeEnum.Implosion;

                        var targets = new List<MobConfigModel>();

                        targets = _mapServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,range,client.TamerId);

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _mapServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    _logger.Verbose($"Skill Type: {skillType}");

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round(skill.SkillInfo.CastingTime);

                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                    // This packet would send attack skill packet that would make DS consume for each monster (Visual only)
                                    // This packet should be sent only if it is single monster skill
                                    // _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new KillOnSkillPacket(attackerHandler, targetMob.GeneralHandler, skillSlot, finalDmg).Serialize());

                                    targetMob?.Die();
                                }

                            });

                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new AreaSkillPacket(attackerHandler,client.Partner.HpRate,targetMobs,skillSlot,finalDmg).Serialize());

                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SkillHitPacket(attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg,targetMob.CurrentHpRate).Serialize());


                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new KillOnSkillPacket(
                                        attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_mapServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();
                            SendBattleOffTask(client,attackerHandler);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        // Save cooldown in database if the cooldown is more than 20 seconds
                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 >= 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }

                }
                // MAP SERVER -> ATTACK PLAYER
                else if (_mapServer.GetEnemyByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId) != null)
                {
                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _pvpServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _pvpServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _pvpServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg != 0)
                                {
                                    finalDmg = DebuffReductionDamage(client,finalDmg);
                                }

                                if (finalDmg <= 0) finalDmg = 1;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _pvpServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());


                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _pvpServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_pvpServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }

                }

            }
            else if (client.DungeonMap)
            {
                var areaOfEffect = skill.SkillInfo.AreaOfEffect;
                var range = skill.SkillInfo.Range;
                var targetType = skill.SkillInfo.Target;

                // DUNGEON SERVER -> ATTACK SUMMON
                if (_dungeonServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId) != null)
                {
                    _logger.Verbose($"Using skill on Summon (Dungeon Server)");

                    var targets = new List<SummonMobModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _dungeonServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,true,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _dungeonServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,true,client.TamerId);
                        }

                        targetSummonMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _dungeonServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetSummonMobs.Add(mob);
                    }

                    _logger.Verbose($"Skill Type: {skillType}");

                    if (targetSummonMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetSummonMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetSummonMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetSummonMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetSummonMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetSummonMobs.First().CurrentHP : SummonAoEDamage(client,targetSummonMobs.First(),skill,skillSlot,_configuration);

                            targetSummonMobs.ForEach(targetMob =>
                            {
                                if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize()
                            );

                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AreaSkillPacket(attackerHandler,client.Partner.HpRate,targetSummonMobs,skillSlot,finalDmg).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetSummonMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _dungeonServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _dungeonServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());
                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new KillOnSkillPacket(attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_dungeonServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId,true))
                        {
                            client.Tamer.StopBattle(true);

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }
                }
                // DUNGEON SERVER -> ATTACK MOB
                else
                {
                    _logger.Verbose($"Using skill on Mob (Dungeon Server)");

                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _dungeonServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _dungeonServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _dungeonServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    _logger.Verbose($"Skill Type: {skillType}");

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg != 0)
                                {
                                    finalDmg = DebuffReductionDamage(client,finalDmg);
                                }

                                if (finalDmg <= 0) finalDmg = 1;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _dungeonServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _dungeonServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                                finalDmg = DebuffReductionDamage(client,finalDmg);

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());

                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new SkillHitPacket(attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg,targetMob.CurrentHpRate).Serialize());

                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _dungeonServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_dungeonServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }
                }
            }
            else if (client.EventMap)
            {
                var areaOfEffect = skill.SkillInfo.AreaOfEffect;
                var range = skill.SkillInfo.Range;
                var targetType = skill.SkillInfo.Target;

                // EVENT SERVER -> ATTACK SUMMON
                if (_eventServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId) != null)
                {
                    var targets = new List<SummonMobModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _eventServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,true,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _eventServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,true,client.TamerId);
                        }

                        targetSummonMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _eventServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetSummonMobs.Add(mob);
                    }

                    if (targetSummonMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetSummonMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetSummonMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetSummonMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetSummonMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetSummonMobs.First().CurrentHP : SummonAoEDamage(client,targetSummonMobs.First(),skill,skillSlot,_configuration);

                            targetSummonMobs.ForEach(targetMob =>
                            {
                                if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());

                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                new AreaSkillPacket(attackerHandler,client.Partner.HpRate,targetSummonMobs,skillSlot,finalDmg).Serialize());
                        }
                        else
                        {
                            var targetMob = targetSummonMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _eventServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _eventServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());
                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new KillOnSkillPacket(attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_eventServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId,true))
                        {
                            client.Tamer.StopBattle(true);

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }
                }
                // EVENT SERVER -> ATTACK MOB
                else if (_eventServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId) != null)
                {
                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _eventServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _eventServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _eventServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg != 0)
                                {
                                    finalDmg = DebuffReductionDamage(client,finalDmg);
                                }

                                if (finalDmg <= 0) finalDmg = 1;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _eventServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _eventServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _eventServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _eventServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());


                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _eventServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_eventServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId))
                        {
                            client.Tamer.StopBattle();

                            SendBattleOffTask(client,attackerHandler,true);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }
                }
            }
            else
            {
                var areaOfEffect = skill.SkillInfo.AreaOfEffect;
                var range = skill.SkillInfo.Range;
                var targetType = skill.SkillInfo.Target;

                if (_mapServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId) != null)
                {
                    _logger.Debug($"Using skill on Summon (Map Server)");

                    var targets = new List<SummonMobModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _mapServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,true,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _mapServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,true,client.TamerId);
                        }

                        targetSummonMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _mapServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,true,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetSummonMobs.Add(mob);
                    }

                    _logger.Verbose($"Skill Type: {skillType}");

                    if (targetSummonMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetSummonMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round((float)skill.SkillInfo.CastingTime);
                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetSummonMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetSummonMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetSummonMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = 0;

                            finalDmg = client.Tamer.GodMode ? targetSummonMobs.First().CurrentHP : SummonAoEDamage(client,targetSummonMobs.First(),skill,skillSlot,_configuration);

                            targetSummonMobs.ForEach(targetMob =>
                            {
                                if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");
                                    targetMob?.Die();
                                }
                            });

                            _mapServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new CastSkillPacket(
                                    skillSlot,
                                    attackerHandler,
                                    targetHandler
                                ).Serialize()
                            );

                            _mapServer.BroadcastForTamerViewsAndSelf(
                                client.TamerId,
                                new AreaSkillPacket(
                                    attackerHandler,
                                    client.Partner.HpRate,
                                    targetSummonMobs,
                                    skillSlot,
                                    finalDmg
                                ).Serialize()
                            );
                        }
                        else
                        {
                            var targetMob = targetSummonMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = 1;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _mapServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new CastSkillPacket(
                                        skillSlot,
                                        attackerHandler,
                                        targetHandler).Serialize());

                                _mapServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new SkillHitPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg,
                                        targetMob.CurrentHpRate
                                        ).Serialize());

                            }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _mapServer.BroadcastForTamerViewsAndSelf(
                                    client.TamerId,
                                    new KillOnSkillPacket(
                                        attackerHandler,
                                        targetMob.GeneralHandler,
                                        skillSlot,
                                        finalDmg
                                        ).Serialize());

                                targetMob?.Die();
                            }
                        }

                        if (!_mapServer.MobsAttacking(client.Tamer.Location.MapId,client.TamerId,true))
                        {
                            client.Tamer.StopBattle(true);

                            SendBattleOffTask(client,attackerHandler);
                        }

                        var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                        if (evolution != null && skill.SkillInfo.Cooldown / 1000 > 20)
                        {
                            evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                            _sender.Send(new UpdateEvolutionCommand(evolution));
                        }
                    }
                }
                else
                {
                    _logger.Debug($"Using skill on Mob (Map Server)");

                    var targetMobs = new List<MobConfigModel>();

                    if (areaOfEffect > 0)
                    {
                        skillType = SkillTypeEnum.TargetArea;

                        var targets = new List<MobConfigModel>();

                        if (targetType == 17)   // Mobs around partner
                        {
                            targets = _mapServer.GetMobsNearbyPartner(client.Partner.Location,areaOfEffect,client.TamerId);
                        }
                        else if (targetType == 18)   // Mobs around mob
                        {
                            targets = _mapServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,areaOfEffect,client.TamerId);
                        }

                        targetMobs.AddRange(targets);
                    }
                    else if (areaOfEffect == 0 && targetType == 80)
                    {
                        skillType = SkillTypeEnum.Implosion;

                        var targets = new List<MobConfigModel>();

                        targets = _mapServer.GetMobsNearbyTargetMob(client.Partner.Location.MapId,targetHandler,range,client.TamerId);

                        targetMobs.AddRange(targets);
                    }
                    else
                    {
                        skillType = SkillTypeEnum.Single;

                        var mob = _mapServer.GetMobByHandler(client.Tamer.Location.MapId,targetHandler,client.TamerId);

                        if (mob == null)
                            return Task.CompletedTask;

                        targetMobs.Add(mob);
                    }

                    _logger.Debug($"Skill Type: {skillType}");

                    if (targetMobs.Any())
                    {
                        if (skillType == SkillTypeEnum.Single && !targetMobs.First().Alive)
                            return Task.CompletedTask;

                        client.Partner.ReceiveDamage(skill.SkillInfo.HPUsage);
                        client.Partner.UseDs(skill.SkillInfo.DSUsage);

                        var castingTime = (int)Math.Round(skill.SkillInfo.CastingTime);

                        if (castingTime <= 0) castingTime = 2000;

                        client.Partner.SetEndCasting(castingTime);

                        targetMobs.ForEach(targetMob =>
                        {
                            _logger.Verbose($"Character {client.Tamer.Id} engaged {targetMob.Id} - {targetMob.Name} with skill {skill.SkillId}.");
                        });

                        if (!client.Tamer.InBattle)
                        {
                            client.Tamer.SetHidden(false);
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(attackerHandler).Serialize());
                            client.Tamer.StartBattleWithSkill(targetMobs,skillType);
                        }
                        else
                        {
                            client.Tamer.SetHidden(false);
                            client.Tamer.UpdateTargetWithSkill(targetMobs,skillType);
                        }

                        if (skillType != SkillTypeEnum.Single)
                        {
                            var finalDmg = client.Tamer.GodMode ? targetMobs.First().CurrentHP : AoeDamage(client,targetMobs.First(),skill,skillSlot,_configuration);

                            targetMobs.ForEach(targetMob =>
                            {
                                if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                                if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                                if (!targetMob.InBattle)
                                {
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                    targetMob.StartBattle(client.Tamer);
                                }
                                else
                                {
                                    targetMob.AddTarget(client.Tamer);
                                }

                                var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                                if (newHp > 0)
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");
                                }
                                else
                                {
                                    _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                    // This packet would send attack skill packet that would make DS consume for each monster (Visual only)
                                    // This packet should be sent only if it is single monster skill
                                    // _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new KillOnSkillPacket(attackerHandler, targetMob.GeneralHandler, skillSlot, finalDmg).Serialize());

                                    targetMob?.Die();
                                }

                            });

                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());
                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new AreaSkillPacket(attackerHandler,client.Partner.HpRate,targetMobs,skillSlot,finalDmg).Serialize());

                        }
                        else
                        {
                            var targetMob = targetMobs.First();

                            if (!targetMob.InBattle)
                            {
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SetCombatOnPacket(targetHandler).Serialize());
                                targetMob.StartBattle(client.Tamer);
                            }
                            else
                            {
                                targetMob.AddTarget(client.Tamer);
                            }

                            var finalDmg = client.Tamer.GodMode ? targetMob.CurrentHP : CalculateDamageOrHeal(client,targetMob,skill,_assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skill.SkillId),skillSlot);

                            if (finalDmg != 0 && !client.Tamer.GodMode)
                            {
                                finalDmg = DebuffReductionDamage(client,finalDmg);
                            }

                            if (finalDmg <= 0) finalDmg = client.Tamer.Partner.AT;
                            if (finalDmg > targetMob.CurrentHP) finalDmg = targetMob.CurrentHP;

                            var newHp = targetMob.ReceiveDamage(finalDmg,client.TamerId);

                            if (newHp > 0)
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} inflicted {finalDmg} damage with skill {skill.SkillId} in mob {targetMob?.Id} - {targetMob?.Name}.");

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new CastSkillPacket(skillSlot,attackerHandler,targetHandler).Serialize());

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new SkillHitPacket(attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg,targetMob.CurrentHpRate).Serialize());
                                }
                            else
                            {
                                _logger.Verbose($"Partner {client.Partner.Id} killed mob {targetMob?.Id} - {targetMob?.Name} with {finalDmg} skill {skill.Id} damage.");

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,new KillOnSkillPacket(
                                        attackerHandler,targetMob.GeneralHandler,skillSlot,finalDmg).Serialize());

                                targetMob?.Die();
                            }
                            }

                            if (!_mapServer.MobsAttacking(client.Tamer.Location.MapId, client.TamerId))
                            {
                                client.Tamer.StopBattle();
                                SendBattleOffTask(client, attackerHandler);
                            }

                            var evolution = client.Tamer.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Tamer.Partner.CurrentType);

                            // Save cooldown in database if the cooldown is more than 20 seconds
                            if (evolution != null && skill.SkillInfo.Cooldown / 1000 >= 20)
                            {
                                evolution.Skills[skillSlot].SetCooldown(skill.SkillInfo.Cooldown / 1000);
                                _sender.Send(new UpdateEvolutionCommand(evolution));
                            }
                    }

                }
            }

            return Task.CompletedTask;
        }

        // -------------------------------------------------------------------------------------

        public async Task SendBattleOffTask(GameClient client, int attackerHandler)
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(4000);

                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new SetCombatOffPacket(attackerHandler).Serialize());
            });
        }

        public async Task SendBattleOffTask(GameClient client, int attackerHandler, bool dungeon)
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(4000);

                _dungeonServer.BroadcastForTamerViewsAndSelf(
                        client.TamerId,
                        new SetCombatOffPacket(attackerHandler).Serialize()
                    );
            });
        }

        // -------------------------------------------------------------------------------------

        private static int DebuffReductionDamage(GameClient client, int finalDmg)
        {
            if (client.Tamer.Partner.DebuffList.ActiveDebuffReductionDamage())
            {
                var debuffInfo = client.Tamer.Partner.DebuffList.ActiveBuffs.Where(buff => buff.BuffInfo.SkillInfo.Apply.Any(apply => apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.AttackPowerDown)).ToList();

                var totalValue = 0;
                var SomaValue = 0;

                foreach (var debuff in debuffInfo)
                {
                    foreach (var apply in debuff.BuffInfo.SkillInfo.Apply)
                    {

                        switch (apply.Type)
                        {
                            case SkillCodeApplyTypeEnum.Default:
                                totalValue += apply.Value;
                                break;

                            case SkillCodeApplyTypeEnum.AlsoPercent:
                            case SkillCodeApplyTypeEnum.Percent:
                                {

                                    SomaValue += apply.Value + (debuff.TypeN) * apply.IncreaseValue;

                                    double fatorReducao = SomaValue / 100;

                                    // Calculando o novo finalDmg após a redução
                                    finalDmg -= (int)(finalDmg * fatorReducao);

                                }
                                break;

                            case SkillCodeApplyTypeEnum.Unknown200:
                                {

                                    SomaValue += apply.AdditionalValue;

                                    double fatorReducao = SomaValue / 100.0;

                                    // Calculando o novo finalDmg após a redução
                                    finalDmg -= (int)(finalDmg * fatorReducao);

                                }
                                break;

                        }
                        break;

                    }
                }
            }

            return finalDmg;
        }

        // -------------------------------------------------------------------------------------

        private int CalculateDamageOrHeal(GameClient client,MobConfigModel? targetMob,DigimonSkillAssetModel? targetSkill,SkillCodeAssetModel? skill,byte skillSlot)
        {
            var skillValue = skill.Apply
                .Where(x => x.Type > 0)
                .Take(3)
                .ToList();

            var partnerEvolution = client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType);

            double f1BaseDamage = (skillValue[0].Value) + ((partnerEvolution.Skills[skillSlot].CurrentLevel) * skillValue[0].IncreaseValue);
            var buff = _assets.BuffInfo.FirstOrDefault(x => x.SkillCode == skill.SkillCode);
            int skillDuration = GetDurationBySkillId((int)skill.SkillCode);

            var durationBuff = UtilitiesFunctions.RemainingTimeSeconds(skillDuration);

            double SkillFactor = 0;
            int clonDamage = 0;
            var attributeMultiplier = 0.00;
            var elementMultiplier = 0.00;

            var Percentual = (decimal)client.Partner.SCD / 100;
            SkillFactor = (double)Percentual;
            var activationChance = 0.0;

            // -- CLON -------------------------------------------------------------------

            double clonPercent = client.Tamer.Partner.Digiclone.ATValue / 100.0;
            int cloValue = (int)(client.Tamer.Partner.BaseStatus.ATValue * clonPercent);

            double factorFromPF = 144.0 / client.Tamer.Partner.Digiclone.ATValue;
            double cloneFactor = Math.Round(1.0 + (0.43 / factorFromPF),2);

            // ---------------------------------------------------------------------------

            f1BaseDamage = Math.Floor(f1BaseDamage * cloneFactor);

            double addedf1Damage = Math.Floor(f1BaseDamage * SkillFactor / 100.0);

            // ---------------------------------------------------------------------------

            int baseDamage = (int)Math.Floor(f1BaseDamage + addedf1Damage + client.Tamer.Partner.AT + client.Tamer.Partner.SKD);

            // clon Verification
            if (client.Tamer.Partner.Digiclone.ATLevel > 0)
                clonDamage = (int)(f1BaseDamage * 0.301);
            else
                clonDamage = 0;

            // Attribute
            if (client.Tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(targetMob.Attribute))
            {
                var attExp = client.Tamer.Partner.GetAttributeExperience();
                var attValue = client.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (targetMob.Attribute.HasAttributeAdvantage(client.Tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier -= 0.25;
            }

            // Element
            if (client.Tamer.Partner.BaseInfo.Element.HasElementAdvantage(targetMob.Element))
            {
                var elementValue = client.Tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * elementValue) / expMax;
            }
            else if (targetMob.Element.HasElementAdvantage(client.Tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier -= 0.25;
            }

            // ---------------------------------------------------------------------------
            foreach (var skillValueIndex in new[] { 1,2 })
            {
                if (skillValue.Count <= skillValueIndex) continue;

                var currentLevel = partnerEvolution.Skills[skillSlot].CurrentLevel;

                if ((int)skillValue[skillValueIndex].Attribute != 39)
                {
                    activationChance += skillValue[skillValueIndex].Chance + currentLevel * 43;
                }
                else
                {
                    activationChance += skillValue[skillValueIndex].Chance + currentLevel * 42;
                }
                if ((int)skillValue[skillValueIndex].Attribute != 37 && (int)skillValue[skillValueIndex].Attribute != 38)
                {
                    durationBuff += currentLevel;
                    skillDuration += currentLevel + 2;
                }


            }

            int attributeBonus = (int)Math.Floor(f1BaseDamage * attributeMultiplier);
            int elementBonus = (int)Math.Floor(f1BaseDamage * elementMultiplier);

            double activationProbability = activationChance / 100.0;
            Random random = new Random();

            bool isActivated = activationProbability >= 1.0 || random.NextDouble() <= activationProbability;

            if (isActivated &&
                ((skillValue.Count > 1 && skillValue[1].Type != 0) ||
                 (skillValue.Count > 2 && skillValue[2].Type != 0)))
            {
                BuffSkill(client,durationBuff,skillDuration,skillSlot);
            }

            int totalDamage =  baseDamage + clonDamage + attributeBonus + elementBonus;

            //_logger.Information($"Skill Damage: {f1BaseDamage} | ClonDamage: {clonDamage}");
            //_logger.Information($"Partner.AT: {client.Tamer.Partner.AT} | Partner.SKD: {client.Tamer.Partner.SKD} | Att Damage: {addedf1Damage}");
            //_logger.Information($"Attribute Damage: {(attributeMultiplier * f1BaseDamage)} | Element Damage: {(elementMultiplier * f1BaseDamage)}");
            //_logger.Information($"Total Single Damage: {totalDamage}\n");

            return totalDamage;
        }

        private int CalculateDamageOrHeal(GameClient client, SummonMobModel? targetMob, DigimonSkillAssetModel? targetSkill, SkillCodeAssetModel? skill, byte skillSlot)
        {
            var SkillValue = skill.Apply.FirstOrDefault(x => x.Type > 0);

            double f1BaseDamage = (SkillValue.Value) + ((client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType).Skills[skillSlot].CurrentLevel) * SkillValue.IncreaseValue);

            double SkillFactor = 0;
            int clonDamage = 0;
            var attributeMultiplier = 0.00;
            var elementMultiplier = 0.00;

            var Percentual = (decimal)client.Partner.SCD / 100;
            SkillFactor = (double)Percentual;

            // -- CLON -------------------------------------------------------------------

            double clonPercent = client.Tamer.Partner.Digiclone.ATValue / 100.0;
            int cloValue = (int)(client.Tamer.Partner.BaseStatus.ATValue * clonPercent);

            double factorFromPF = 144.0 / client.Tamer.Partner.Digiclone.ATValue;
            double cloneFactor = Math.Round(1.0 + (0.43 / factorFromPF), 2);

            // ---------------------------------------------------------------------------

            f1BaseDamage = Math.Floor(f1BaseDamage * cloneFactor);

            double addedf1Damage = Math.Floor(f1BaseDamage * SkillFactor / 100.0);

            // ---------------------------------------------------------------------------

            int baseDamage = (int)Math.Floor(f1BaseDamage + addedf1Damage + client.Tamer.Partner.AT + client.Tamer.Partner.SKD);

            // clon Verification
            if (client.Tamer.Partner.Digiclone.ATLevel > 0)
                clonDamage = (int)(f1BaseDamage * 0.301);
            else
                clonDamage = 0;

            // Attribute
            if (client.Tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(targetMob.Attribute))
            {
                var attExp = client.Tamer.Partner.GetAttributeExperience();
                var attValue = client.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (targetMob.Attribute.HasAttributeAdvantage(client.Tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            // Element
            if (client.Tamer.Partner.BaseInfo.Element.HasElementAdvantage(targetMob.Element))
            {
                var elementValue = client.Tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * elementValue) / expMax;
            }
            else if (targetMob.Element.HasElementAdvantage(client.Tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.25;
            }

            // ---------------------------------------------------------------------------

            int attributeBonus = (int)Math.Floor(f1BaseDamage * attributeMultiplier);
            int elementBonus = (int)Math.Floor(f1BaseDamage * elementMultiplier);

            int totalDamage = baseDamage + clonDamage + attributeBonus + elementBonus;

            return totalDamage;
        }

        private int CalculateDamageOrHealPlayer(GameClient client, DigimonModel? targetMob, DigimonSkillAssetModel? targetSkill, SkillCodeAssetModel? skill, byte skillSlot)
        {

            var SkillValue = skill.Apply.FirstOrDefault(x => x.Type > 0);

            double f1BaseDamage = (SkillValue.Value) + ((client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType).Skills[skillSlot].CurrentLevel) * SkillValue.IncreaseValue);
            double SkillFactor = 0;
            double MultiplierAttribute = 0;

            var Percentual = (decimal)client.Partner.SCD / 100;
            SkillFactor = (double)Percentual; // AttributeMultiplier

            double factorFromPF = 144.0 / client.Tamer.Partner.Digiclone.ATValue;

            double cloneFactor = Math.Round(1.0 + (0.43 / factorFromPF), 2);
            f1BaseDamage = Math.Floor(f1BaseDamage * cloneFactor);

            double addedf1Damage = Math.Floor(f1BaseDamage * SkillFactor / 100.0);

            var attributeVantage = client.Tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(targetMob.BaseInfo.Attribute);
            var elementVantage = client.Tamer.Partner.BaseInfo.Element.HasElementAdvantage(targetMob.BaseInfo.Element);

            var Damage = (int)Math.Floor(f1BaseDamage + addedf1Damage + (client.Tamer.Partner.AT / targetMob.DE) + client.Tamer.Partner.SKD);

            if (client.Partner.AttributeExperience.CurrentAttributeExperience && attributeVantage)
            {

                MultiplierAttribute = (2 + ((client.Partner.ATT) / 200.0));
                Random random = new Random();

                // Gere um valor aleatório entre 0% e 5% a mais do valor original
                double percentagemBonus = random.NextDouble() * 0.05;

                // Calcule o valor final com o bônus
                return (int)((int)Math.Floor(MultiplierAttribute * Damage) * (1.0 + percentagemBonus));

            }
            else if (client.Partner.AttributeExperience.CurrentElementExperience && elementVantage)
            {
                MultiplierAttribute = 2;

                Random random = new Random();

                // Gere um valor aleatório entre 0% e 5% a mais do valor original
                double percentagemBonus = random.NextDouble() * 0.05;

                // Calcule o valor final com o bônus
                return (int)((int)Math.Floor(MultiplierAttribute * Damage) * (1.0 + percentagemBonus));
            }
            else
            {
                Random random = new Random();

                // Gere um valor aleatório entre 0% e 5% a mais do valor original
                double percentagemBonus = random.NextDouble() * 0.05;

                // Calcule o valor final com o bônus
                return (int)(Damage * (1.0 + percentagemBonus));


            }

        }

        // -------------------------------------------------------------------------------------

        private int AoeDamage(GameClient client, MobConfigModel? targetMob, DigimonSkillAssetModel? targetSkill, byte skillSlot, IConfiguration configuration)
        {
            var skillCode = _assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == targetSkill.SkillId);
            var skill = _assets.DigimonSkillInfo.FirstOrDefault(x => x.Type == client.Partner.CurrentType && x.Slot == skillSlot);
            var skillValue = skillCode?.Apply.FirstOrDefault(x => x.Type > 0);
            
            double f1BaseDamage = skillValue.Value + (client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType).Skills[skillSlot].CurrentLevel * skillValue.IncreaseValue);

            double SkillFactor = 0;
            int clonDamage = 0;
            var attributeMultiplier = 0.00;
            var elementMultiplier = 0.00;

            var Percentual = (decimal)client.Partner.SCD / 100;
            SkillFactor = (double)Percentual;

            // -- CLON -------------------------------------------------------------------

            double factorFromPF = 144.0 / client.Tamer.Partner.Digiclone.ATValue;
            double cloneFactor = Math.Round(1.0 + (0.43 / factorFromPF), 2);

            f1BaseDamage = Math.Floor(f1BaseDamage * cloneFactor);

            double addedf1Damage = Math.Floor(f1BaseDamage * SkillFactor / 100.0);

            // ---------------------------------------------------------------------------

            int baseDamage = (int)Math.Floor(f1BaseDamage + addedf1Damage + client.Tamer.Partner.AT + client.Tamer.Partner.SKD);

            // clon Verification
            if (client.Tamer.Partner.Digiclone.ATLevel > 0)
                clonDamage = (int)(f1BaseDamage * 0.301);
            else
                clonDamage = 0;

            // Attribute
            if (client.Tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(targetMob.Attribute))
            {
                var attExp = client.Tamer.Partner.GetAttributeExperience();
                var attValue = client.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (targetMob.Attribute.HasAttributeAdvantage(client.Tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            // Element
            if (client.Tamer.Partner.BaseInfo.Element.HasElementAdvantage(targetMob.Element))
            {
                var elementValue = client.Tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * elementValue) / expMax;
            }
            else if (targetMob.Element.HasElementAdvantage(client.Tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.25;
            }

            // ---------------------------------------------------------------------------

            int attributeBonus = (int)Math.Floor(f1BaseDamage * attributeMultiplier);
            int elementBonus = (int)Math.Floor(f1BaseDamage * elementMultiplier);

            int totalDamage = baseDamage + clonDamage + attributeBonus + elementBonus;

            //_logger.Information($"Skill Damage: {f1BaseDamage} | Att Damage: {addedf1Damage} | Clon Damage: {clonDamage}");
            //_logger.Information($"Partner.AT: {client.Tamer.Partner.AT} | Partner.SKD: {client.Tamer.Partner.SKD}");
            //_logger.Information($"Attribute Damage: {attributeBonus} | Element Damage: {elementBonus}");
            //_logger.Information($"Total Area Damage: {totalDamage}\n");

            return totalDamage;
        }

        private int SummonAoEDamage(GameClient client, SummonMobModel? targetMob, DigimonSkillAssetModel? targetSkill, byte skillSlot, IConfiguration configuration)
        {
            var skillCode = _assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == targetSkill.SkillId);
            var skill = _assets.DigimonSkillInfo.FirstOrDefault(x => x.Type == client.Partner.CurrentType && x.Slot == skillSlot);
            var skillValue = skillCode?.Apply.FirstOrDefault(x => x.Type > 0);

            double f1BaseDamage = skillValue.Value + (client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType).Skills[skillSlot].CurrentLevel * skillValue.IncreaseValue);

            double SkillFactor = 0;
            int clonDamage = 0;
            var attributeMultiplier = 0.00;
            var elementMultiplier = 0.00;

            var Percentual = (decimal)client.Partner.SCD / 100;
            SkillFactor = (double)Percentual;

            // -- CLON -------------------------------------------------------------------

            double factorFromPF = 144.0 / client.Tamer.Partner.Digiclone.ATValue;
            double cloneFactor = Math.Round(1.0 + (0.43 / factorFromPF), 2);

            f1BaseDamage = Math.Floor(f1BaseDamage * cloneFactor);

            double addedf1Damage = Math.Floor(f1BaseDamage * SkillFactor / 100.0);

            // ---------------------------------------------------------------------------

            int baseDamage = (int)Math.Floor(f1BaseDamage + addedf1Damage + client.Tamer.Partner.AT + client.Tamer.Partner.SKD);

            // clon Verification
            if (client.Tamer.Partner.Digiclone.ATLevel > 0)
                clonDamage = (int)(f1BaseDamage * 0.301);
            else
                clonDamage = 0;

            // Attribute
            if (client.Tamer.Partner.BaseInfo.Attribute.HasAttributeAdvantage(targetMob.Attribute))
            {
                var attExp = client.Tamer.Partner.GetAttributeExperience();
                var attValue = client.Partner.ATT / 100.0;
                var attValuePercent = attValue / 100.0;
                var bonusMax = 1;
                var expMax = 10000;

                attributeMultiplier = ((bonusMax + attValuePercent) * attExp) / expMax;
            }
            else if (targetMob.Attribute.HasAttributeAdvantage(client.Tamer.Partner.BaseInfo.Attribute))
            {
                attributeMultiplier = -0.25;
            }

            // Element
            if (client.Tamer.Partner.BaseInfo.Element.HasElementAdvantage(targetMob.Element))
            {
                var elementValue = client.Tamer.Partner.GetElementExperience();
                var bonusMax = 1;
                var expMax = 10000;

                elementMultiplier = (bonusMax * elementValue) / expMax;
            }
            else if (targetMob.Element.HasElementAdvantage(client.Tamer.Partner.BaseInfo.Element))
            {
                elementMultiplier = -0.25;
            }

            // ---------------------------------------------------------------------------

            int attributeBonus = (int)Math.Floor(f1BaseDamage * attributeMultiplier);
            int elementBonus = (int)Math.Floor(f1BaseDamage * elementMultiplier);

            int totalDamage = baseDamage + clonDamage + attributeBonus + elementBonus;

            return totalDamage;
        }

        // -------------------------------------------------------------------------------------
        private void BuffSkill(GameClient client,int duration,int skillDuration,byte skillSlot)
        {
            var skillInfo = _assets.DigimonSkillInfo.FirstOrDefault(x => x.Type == client.Partner.CurrentType && x.Slot == skillSlot);
            Action<long,byte[]> broadcastAction = client.DungeonMap
                ? (id,data) => _dungeonServer.BroadcastForTamerViewsAndSelf(id,data)
                : (id,data) => _mapServer.BroadcastForTamerViewsAndSelf(id,data);

            var skillCode = _assets.SkillCodeInfo.FirstOrDefault(x => x.SkillCode == skillInfo.SkillId);
            var buff = _assets.BuffInfo.FirstOrDefault(x => x.SkillCode == skillCode.SkillCode);
            var skillValue = skillCode.Apply.Where(x => x.Type > 0).Take(3).ToList();
            var partnerEvolution = client.Partner.Evolutions.FirstOrDefault(x => x.Type == client.Partner.CurrentType);
            var selectedMob = client.Tamer.TargetMob;

            if (buff != null)
            {
                var debuffs = new List<SkillCodeApplyAttributeEnum>
        {
            SkillCodeApplyAttributeEnum.CrowdControl,
            SkillCodeApplyAttributeEnum.DOT,
            SkillCodeApplyAttributeEnum.DOT2
        };

                var buffs = new List<SkillCodeApplyAttributeEnum>
        {
            SkillCodeApplyAttributeEnum.MS,
            SkillCodeApplyAttributeEnum.SCD,
            SkillCodeApplyAttributeEnum.CC,
            SkillCodeApplyAttributeEnum.AS,
            SkillCodeApplyAttributeEnum.AT,
            SkillCodeApplyAttributeEnum.HP,
            SkillCodeApplyAttributeEnum.DamageShield,
            SkillCodeApplyAttributeEnum.CA,
            SkillCodeApplyAttributeEnum.Unbeatable,
            SkillCodeApplyAttributeEnum.DR,
            SkillCodeApplyAttributeEnum.EV
        };

                for (int i = 1;i <= 2;i++)
                {
                    if (skillValue.Count > i)
                    {
                        switch (skillValue[i].Attribute)
                        {
                            // Handling Buffs
                            case var attribute when buffs.Contains(attribute):
                                int buffsValue = skillValue[i].Value + (partnerEvolution.Skills[skillSlot].CurrentLevel * skillValue[i].IncreaseValue);
                                client.Tamer.Partner.BuffValueFromBuffSkill = buffsValue;

                                var newDigimonBuff = DigimonBuffModel.Create(buff.BuffId,buff.SkillId,0,skillDuration);
                                var activeBuff = client.Tamer.Partner.BuffList.Buffs.FirstOrDefault(x => x.BuffId == buff.BuffId);
                                switch (attribute)
                                {
                                    case SkillCodeApplyAttributeEnum.DR: //reflect damage
                                        if (activeBuff == null)
                                        {
                                            newDigimonBuff.SetBuffInfo(buff);
                                            client.Tamer.Partner.BuffList.Add(newDigimonBuff);
                                            broadcastAction(client.TamerId,new SkillBuffPacket(
                                                client.Tamer.GeneralHandler,
                                                (int)buff.BuffId,
                                                0,
                                                duration,
                                                (int)skillCode.SkillCode).Serialize());

                                            var reflectDamageInterval = TimeSpan.FromMilliseconds(selectedMob.ASValue);
                                            var reflectDamageDuration = duration;
                                            var buffId = newDigimonBuff.BuffId;

                                            Task.Run(async () =>
                                            {
                                                await Task.Delay(1500);


                                                for (int i = 0;i < reflectDamageDuration;i++)
                                                {
                                                    if (selectedMob == null
                                                        || !client.Tamer.Partner.BuffList.Buffs.Any(b => b.BuffId == buffId)
                                                        || selectedMob.CurrentAction != MobActionEnum.Attack)
                                                    {
                                                        client.Tamer.Partner.BuffList.Remove(newDigimonBuff.BuffId);
                                                        broadcastAction(client.Tamer.Id,
                                                            new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,newDigimonBuff.BuffId).Serialize());
                                                        break;
                                                    }

                                                    var damageValue = selectedMob.ATValue * 3;
                                                    var newHp = selectedMob.ReceiveDamage(damageValue,client.TamerId);

                                                    broadcastAction(client.TamerId,new AddDotDebuffPacket(
                                                        client.Tamer.Partner.GeneralHandler,selectedMob.GeneralHandler,
                                                        newDigimonBuff.BuffId,selectedMob.CurrentHpRate,damageValue,
                                                        (byte)((newHp > 0) ? 0 : 1)).Serialize());

                                                    if (newHp <= 0)
                                                    {
                                                        client.Tamer.Partner.BuffList.Remove(newDigimonBuff.BuffId);
                                                        broadcastAction(client.Tamer.Id,
                                                            new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,newDigimonBuff.BuffId).Serialize());
                                                        selectedMob.Die();
                                                        break;
                                                    }

                                                    await Task.Delay(reflectDamageInterval);
                                                }

                                            });

                                        }
                                        break;
                                    case SkillCodeApplyAttributeEnum.DamageShield:
                                        int shieldHp = skillValue[i].Value + (partnerEvolution.Skills[skillSlot].CurrentLevel * skillValue[i].IncreaseValue);

                                        if (client.Tamer.Partner.DamageShieldHp > 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            newDigimonBuff.SetBuffInfo(buff);
                                            client.Tamer.Partner.BuffList.Add(newDigimonBuff);
                                            client.Tamer.Partner.DamageShieldHp = shieldHp;

                                            broadcastAction(client.TamerId,new SkillBuffPacket(
                                                client.Tamer.GeneralHandler,
                                                (int)buff.BuffId,
                                                partnerEvolution.Skills[skillSlot].CurrentLevel,
                                                duration,
                                                (int)skillCode.SkillCode).Serialize());

                                            Task.Run(async () =>
                                            {
                                                int remainingDuration = skillDuration;
                                                while (remainingDuration > 0)
                                                {
                                                    await Task.Delay(1000);

                                                    if (client.Tamer.Partner.DamageShieldHp <= 0)
                                                    {
                                                        client.Tamer.Partner.DamageShieldHp = 0;
                                                        client.Tamer.Partner.BuffList.Remove(newDigimonBuff.BuffId);
                                                        broadcastAction(client.TamerId,new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,newDigimonBuff.BuffId).Serialize());
                                                        break;
                                                    }

                                                    remainingDuration--;
                                                }

                                                if (client.Tamer.Partner.DamageShieldHp > 0)
                                                {
                                                    client.Tamer.Partner.DamageShieldHp = 0;
                                                    client.Tamer.Partner.BuffList.Remove(newDigimonBuff.BuffId);
                                                    broadcastAction(client.TamerId,new RemoveBuffPacket(client.Tamer.Partner.GeneralHandler,newDigimonBuff.BuffId).Serialize());
                                                }


                                            });
                                        }
                                        break;
                                    case SkillCodeApplyAttributeEnum.Unbeatable:
                                        if (client.Tamer.Partner.IsUnbeatable)
                                        {
                                            break;
                                        }
                                            newDigimonBuff.SetBuffInfo(buff);
                                            client.Tamer.Partner.BuffList.Add(newDigimonBuff);

                                            client.Tamer.Partner.IsUnbeatable = true;

                                            broadcastAction(client.TamerId,new SkillBuffPacket(
                                                client.Tamer.GeneralHandler,
                                                (int)buff.BuffId,
                                                partnerEvolution.Skills[skillSlot].CurrentLevel,
                                                duration,
                                                (int)skillCode.SkillCode).Serialize());
                                        
                                        Task.Delay(skillDuration * 1000).ContinueWith(_ =>
                                        {
                                            client.Tamer.Partner.IsUnbeatable = false;
                                        });
                                        break;
                                    case SkillCodeApplyAttributeEnum.EV:
                                    case SkillCodeApplyAttributeEnum.MS:
                                    case SkillCodeApplyAttributeEnum.SCD:
                                    case SkillCodeApplyAttributeEnum.CA:
                                    case SkillCodeApplyAttributeEnum.AT:
                                    case SkillCodeApplyAttributeEnum.HP:
                                        if (activeBuff == null)
                                        {
                                            newDigimonBuff.SetBuffInfo(buff);
                                            client.Tamer.Partner.BuffList.Add(newDigimonBuff);

                                            broadcastAction(client.TamerId,new SkillBuffPacket(
                                                client.Tamer.GeneralHandler,
                                                (int)buff.BuffId,
                                                partnerEvolution.Skills[skillSlot].CurrentLevel,
                                                duration,
                                                (int)skillCode.SkillCode).Serialize());
                                        }
                                        client.Send(new UpdateStatusPacket(client.Tamer));
                                        break;
                                }
                                break;

                            // Handling Debuffs
                            case var attribute when debuffs.Contains(attribute):

                                var activeDebuff = selectedMob.DebuffList.Buffs.FirstOrDefault(x => x.BuffId == buff.BuffId);
                                var newMobDebuff = MobDebuffModel.Create(buff.BuffId,(int)skillCode.SkillCode,0,skillDuration);

                                newMobDebuff.SetBuffInfo(buff);
                                int debuffsValue = skillValue[i].Value + (partnerEvolution.Skills[skillSlot].CurrentLevel * skillValue[i].IncreaseValue);

                                switch (attribute)
                                {
                                    case SkillCodeApplyAttributeEnum.CrowdControl:
                                        if (activeDebuff == null)
                                        {
                                            selectedMob.DebuffList.Buffs.Add(newMobDebuff);
                                        }

                                        if (selectedMob.CurrentAction != Commons.Enums.Map.MobActionEnum.CrowdControl)
                                        {
                                            selectedMob.UpdateCurrentAction(Commons.Enums.Map.MobActionEnum.CrowdControl);
                                        }

                                        broadcastAction(client.TamerId,new AddStunDebuffPacket(
                                            selectedMob.GeneralHandler,newMobDebuff.BuffId,newMobDebuff.SkillId,duration).Serialize());
                                        break;

                                    case SkillCodeApplyAttributeEnum.DOT:
                                    case SkillCodeApplyAttributeEnum.DOT2:
                                        if (debuffsValue > selectedMob.CurrentHP)
                                            debuffsValue = selectedMob.CurrentHP;

                                        broadcastAction(client.TamerId,new AddBuffPacket(
                                            selectedMob.GeneralHandler,buff,partnerEvolution.Skills[skillSlot].CurrentLevel,duration).Serialize());

                                        if (activeDebuff != null)
                                        {
                                            activeDebuff.IncreaseEndDate(skillDuration);
                                        }
                                        else
                                        {
                                            selectedMob.DebuffList.Buffs.Add(newMobDebuff);
                                        }

                                        Task.Delay(skillDuration * 1000).ContinueWith(_ =>
                                        {
                                            if (selectedMob == null) return;
                                            var newHp = selectedMob.ReceiveDamage(debuffsValue,client.TamerId);

                                            broadcastAction(client.TamerId,new AddDotDebuffPacket(
                                                client.Tamer.Partner.GeneralHandler,selectedMob.GeneralHandler,
                                                newMobDebuff.BuffId,selectedMob.CurrentHpRate,debuffsValue,(byte)((newHp > 0) ? 0 : 1)).Serialize());

                                            if (newHp <= 0)
                                            {
                                                selectedMob.Die();
                                            }
                                        });
                                        break;
                                }
                                break;
                        }
                    }
                }

                _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
            }
        }



        private int GetDurationBySkillId(int skillCode)
        {
            return skillCode switch
            {
                (int)SkillBuffAndDebuffDurationEnum.FireRocket => 5, //38 = attribute enums
                (int)SkillBuffAndDebuffDurationEnum.DynamiteHead => 4, //33
                (int)SkillBuffAndDebuffDurationEnum.BlueThunder => 2, //39
                (int)SkillBuffAndDebuffDurationEnum.NeedleRain => 10, //37  missing packet?
                (int)SkillBuffAndDebuffDurationEnum.MysticBell => 3, //
                (int)SkillBuffAndDebuffDurationEnum.GoldRush => 3, // 39 missing packet petrify?
                (int)SkillBuffAndDebuffDurationEnum.NeedleStinger => 15, //6
                (int)SkillBuffAndDebuffDurationEnum.CurseOfQueen => 10, //24
                (int)SkillBuffAndDebuffDurationEnum.WhiteStatue => 15, //40 //reflect damage packet?
                (int)SkillBuffAndDebuffDurationEnum.RedSun => 10, //24 
                (int)SkillBuffAndDebuffDurationEnum.PlasmaShot => 5, //38
                (int)SkillBuffAndDebuffDurationEnum.ExtremeJihad => 10, //24
                (int)SkillBuffAndDebuffDurationEnum.MomijiOroshi => 15, //8
                (int)SkillBuffAndDebuffDurationEnum.Ittouryoudan => 20, //41
                (int)SkillBuffAndDebuffDurationEnum.ShiningGoldSolarStorm => 6, //33 Invincible Silver Magnamon
                (int)SkillBuffAndDebuffDurationEnum.MagnaAttack => 5, // MagnaAttack Magnamon Worn F1
                (int)SkillBuffAndDebuffDurationEnum.PlasmaRage => 10, // MagnaAttack Magnamon Worn F2
                (int)SkillBuffAndDebuffDurationEnum.KyukyokuSenjin => 1, // AOA Magnamon Worn F2
                (int)SkillBuffAndDebuffDurationEnum.RamapageAlterBF3 => 10, // Alter B Rampage
                _ => 0 
            };
        }


    }
}