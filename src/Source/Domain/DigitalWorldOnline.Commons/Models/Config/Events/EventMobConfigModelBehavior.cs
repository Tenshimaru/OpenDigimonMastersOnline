using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Map;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Utils;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed partial class EventMobConfigModel
    {
        private const int HandlerRange = 65538;
        private int _targetHandler;
        private bool _giveUp;

        #region Target

        public DigimonModel? Target => TargetTamer?.Partner;
        public CharacterModel? TargetTamer => TargetTamers.FirstOrDefault(x => x.GeneralHandler == _targetHandler);
        public int TargetHandler => Target?.GeneralHandler ?? 0;
        public bool TargetAlive => Target != null && Target.Alive;

        #endregion

        public bool CanAct => DateTime.Now > LastActionTime;

        public bool CanHeal => DateTime.Now > LastHealTime;

        public bool CanMissHit()
        {
            try
            {
                if (Target == null)
                    return true;

                var rand = new Random();

                double TargetEvasion = (double)Target?.EV;
                double AttackerHitRate = (double)HTValue;

                int levelDifference = Level - Target.Level;

                if (AttackerHitRate > TargetEvasion || levelDifference > 15)
                {
                    return false; // O Mob acerta o hit
                }
                else if (levelDifference <= 20)
                {
                    if (Level >= Target.Level)
                        return false;

                    double attributeAdvantage = 1.5; // Defina o valor do attributeAdvantage conforme necessário

                    if (Attribute.HasAttributeAdvantage(Target.BaseInfo.Attribute) ||
                        Element.HasElementAdvantage(Target.BaseInfo.Element))
                        attributeAdvantage = 2.0;

                    if (Target.BaseInfo.Attribute.HasAttributeAdvantage(Attribute) ||
                        Target.BaseInfo.Element.HasElementAdvantage(Element))
                        attributeAdvantage = 1.0;

                    double adjustedPercent = CalcularProbabilidadeAcerto(AttackerHitRate, Level, Target.Level,
                        TargetEvasion, attributeAdvantage);

                    if (adjustedPercent <= 1.0)
                        adjustedPercent = 0;

                    if (adjustedPercent > 100.0)
                        adjustedPercent = 100;

                    var TotalChance = (int)adjustedPercent;

                    if (TotalChance <= 10)
                        return true;

                    return rand.Next(1, 100) >= TotalChance; // Ajuste esse valor conforme necessário
                }
            }
            catch (Exception ex)
            {
                return true;
            }

            return true;
        }

        public static double CalcularProbabilidadeAcerto(double seuHitRate, int seuNivel, int nivelDoMonstro,
            double evDoMonstro, double attributeAdvantage)
        {
            double diferencaDeNiveis = seuNivel - nivelDoMonstro;
            double levelMultiplier = 1 / (1 + Math.Exp(-diferencaDeNiveis / 9.0));
            double probabilidade = levelMultiplier * (seuHitRate / evDoMonstro) * attributeAdvantage * 100;

            return probabilidade;
        }

        public bool Alive => CurrentHP > 1;

        public byte CurrentHpRate
        {
            get
            {
                try
                {
                    return (byte)((CurrentHP * 255L) / HPValue);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public byte HpPercent
        {
            get
            {
                try
                {
                    return (byte)(CurrentHP * 100 / HPValue);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public bool Chasing => ChaseEndTime > DateTime.Now;

        public bool SkillTime => DateTime.Now >= LastSkillTryTime.AddSeconds(10) &&
                                 DateTime.Now >= LastSkillTime.AddMilliseconds(Cooldown);


        /// <summary>
        /// Updates the digimon unique identifier.
        /// </summary>
        /// <param name="id">New id</param>
        public void SetId(long id) => Id = id;

        /// <summary>
        /// Updates the digimon name.
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string name) => Name = name;

        /// <summary>
        /// Updates the digimon type.
        /// </summary>
        /// <param name="type">New type</param>
        public void SetType(int type) => Type = type;

        /// <summary>
        /// Updates the digimon model.
        /// </summary>
        /// <param name="model">New model</param>
        public void SetModel(int model) => Model = model;

        /// <summary>
        /// Updates the digimon level.
        /// </summary>
        /// <param name="level">New level</param>
        public void SetLevel(byte level) => Level = level;

        /// <summary>
        /// Updates the digimon attribute.
        /// </summary>
        /// <param name="attr">New attribute</param>
        public void SetAttribute(DigimonAttributeEnum attr) => Attribute = attr;

        /// <summary>
        /// Updates the digimon element.
        /// </summary>
        /// <param name="element">New element</param>
        public void SetElement(DigimonElementEnum element) => Element = element;

        /// <summary>
        /// Updates the digimon main family.
        /// </summary>
        /// <param name="family">New family</param>
        public void SetMainFamily(DigimonFamilyEnum family) => Family1 = family;

        /// <summary>
        /// Updates the digimon second family.
        /// </summary>
        /// <param name="family">New family</param>
        public void SetSecondFamily(DigimonFamilyEnum family) => Family2 = family;

        /// <summary>
        /// Updates the digimon third family.
        /// </summary>
        /// <param name="family">New family</param>
        public void SetThirdFamily(DigimonFamilyEnum family) => Family3 = family;

        /// <summary>
        /// Updates the digimon reaction type.
        /// </summary>
        /// <param name="reactionType">New reaction type</param>
        public void SetReactionType(DigimonReactionTypeEnum reactionType) => ReactionType = reactionType;

        /// <summary>
        /// Updates the digimon view range.
        /// </summary>
        /// <param name="value">New value</param>
        public void SetViewRange(int value) => ViewRange = value;

        /// <summary>
        /// Updates the digimon hunt range.
        /// </summary>
        /// <param name="value">New value</param>
        public void SetHuntRange(int value) => HuntRange = value;

        /// <summary>
        /// Updates mob location.
        /// </summary>
        /// <param name="location">New location</param>
        public void SetLocation(MobLocationConfigModel location) => Location = location;

        /// <summary>
        /// Updates mob location.
        /// </summary>
        /// <param name="location">New location</param>
        public void SetLocation(short mapId, int x, int y)
        {
            Location = new MobLocationConfigModel();
            Location.SetMapId(mapId);
            Location.SetX(x);
            Location.SetY(y);
        }

        /// <summary>
        /// Updates mob drop reward.
        /// </summary>
        /// <param name="config">New reward</param>
        public void SetDropReward(MobDropRewardConfigModel config) => DropReward = config;

        /// <summary>
        /// Updates mob exp reward.
        /// </summary>
        /// <param name="config">New reward</param>
        public void SetExpReward(MobExpRewardConfigModel config) => ExpReward = config;

        public void SetSkillCooldown(int cooldown) => Cooldown = cooldown;

        public void UpdateChaseTime(DateTime chaseEnd) => ChaseEndTime = chaseEnd;

        public void UpdateLastHit() => LastHitTime = DateTime.Now;

        public void UpdateLastHitTry() => LastHitTryTime = DateTime.Now;

        public void UpdateLastSkill() => LastSkillTime = DateTime.Now;

        public void UpdateLastSkillTry() => LastSkillTryTime = DateTime.Now;

        public void UpdateLastHeal() => LastHealTime = DateTime.Now.AddMilliseconds(7500);

        public void UpdateCurrentAction(MobActionEnum action) => CurrentAction = action;

        public void UpdateCheckSkill(bool condition) => CheckSkill = condition;

        public int ReceiveDamage(int damage, long tamerId)
        {
            if (!RaidDamage.ContainsKey(tamerId))
                RaidDamage.Add(tamerId, damage);
            else
                RaidDamage[tamerId] += damage;

            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;

            return CurrentHP;
        }

        public void UpdateCurrentHp(int newValue) => CurrentHP = newValue;

        public void SetHandlerValue(short mapHandler) => GeneralHandler = HandlerRange + mapHandler;

        public void SetNextAction()
        {
            switch (CurrentAction)
            {
                case MobActionEnum.Wait:
                {
                    if (InBattle)
                    {
                        if (SkillTime && !CheckSkill && IsPossibleSkill)
                        {
                            CurrentAction = MobActionEnum.UseAttackSkill;
                        }
                        else
                        {
                            CurrentAction = MobActionEnum.Attack;
                        }
                    }
                    else if (Dead)
                    {
                        CurrentAction = MobActionEnum.Reward;
                    }
                    else if (DateTime.Now > NextWalkTime)
                    {
                        CurrentAction = MobActionEnum.Walk;
                    }
                }
                    break;

                case MobActionEnum.Reward:
                {
                    CurrentAction = MobActionEnum.Respawn;
                    LastActionTime = DateTime.Now.AddSeconds(3 + RespawnInterval);
                }
                    break;

                case MobActionEnum.Walk:
                {
                    if (InBattle)
                    {
                        CurrentAction = MobActionEnum.Attack;
                    }
                    else if (Dead)
                    {
                        CurrentAction = MobActionEnum.Reward;
                        LastActionTime = DateTime.Now;
                    }
                    else
                    {
                        CurrentAction = MobActionEnum.Wait;
                    }
                }
                    break;

                case MobActionEnum.Attack:
                {
                    if (_giveUp)
                    {
                        CurrentAction = MobActionEnum.GiveUp;
                    }
                    else if (Dead)
                    {
                        CurrentAction = MobActionEnum.Reward;
                    }
                    else if (InBattle && !TargetAlive && TargetTamers.Count <= 1)
                    {
                        CurrentAction = MobActionEnum.GiveUp;
                    }
                }
                    break;

                case MobActionEnum.UseAttackSkill:
                {
                    if (_giveUp)
                    {
                        CurrentAction = MobActionEnum.GiveUp;
                    }
                    else if (Dead)
                    {
                        CurrentAction = MobActionEnum.Reward;
                    }
                    else if (InBattle && !TargetAlive && TargetTamers.Count <= 1)
                    {
                        CurrentAction = MobActionEnum.GiveUp;
                    }
                }
                    break;

                case MobActionEnum.GiveUp:
                {
                    if (Dead)
                    {
                        CurrentAction = MobActionEnum.Reward;
                        LastActionTime = DateTime.Now;
                    }
                    else
                    {
                        CurrentAction = MobActionEnum.Walk;
                        LastActionTime = DateTime.Now.AddSeconds(6);
                    }
                }
                    break;

                case MobActionEnum.Respawn:
                {
                    CurrentAction = MobActionEnum.Wait;
                    LastActionTime = DateTime.Now;
                }
                    break;
            }
        }

        public void ResetLocation()
        {
            CurrentLocation.SetX(InitialLocation.X);
            CurrentLocation.SetY(InitialLocation.Y);
        }

        public void Reset(bool keepViews = false)
        {
            _giveUp = false;

            _targetHandler = 0;

            Dead = false;

            CurrentHP = HPValue;

            TargetTamers.ForEach(targetTamer =>
            {
                if (targetTamer?.TargetMobs != null)
                {
                    targetTamer.TargetMobs.RemoveAll(x => x.Id == Id);
                }
            });

            TargetTamers.Clear();

            if (!keepViews)
                TamersViewing.Clear();

            InBattle = false;
        }

        public void NextTarget()
        {
            TargetTamers.RemoveAll(x => x.GeneralHandler == _targetHandler);

            if (TargetTamers.Count > 0)
                _targetHandler = TargetTamers.First().GeneralHandler;
        }

        public void SetAwaitingKillSpawn(bool awaitingKillSpawn = true) => AwaitingKillSpawn = awaitingKillSpawn;

        public void SetInitialLocation()
        {
            if (InitialLocation == null)
                InitialLocation = new Models.Location(Location.MapId, Location.X, Location.Y);

            if (CurrentLocation == null)
                CurrentLocation = new Models.Location(Location.MapId, Location.X, Location.Y);

            if (PreviousLocation == null)
                PreviousLocation = new Models.Location(Location.MapId, Location.X, Location.Y);
        }

        public void Destroy()
        {
            _targetHandler = 0;
            TargetTamers.Clear();
            InBattle = false;

            CurrentAction = MobActionEnum.Destroy;
        }

        public void StartBattle(CharacterModel tamer)
        {
            LastHitTryTime = DateTime.Now;

            if (!TargetTamers.Any())
                _targetHandler = tamer.GeneralHandler;

            if (!TargetTamers.Any(x => x.Id == tamer.Id))
                TargetTamers.Add(tamer);

            InBattle = true;
        }

        public void AddTarget(CharacterModel tamer)
        {
            if (!TargetTamers.Any(x => x.Id == tamer.Id))
                TargetTamers.Add(tamer);
        }

        /// <summary>
        /// Updates the Respawn flag.
        /// </summary>
        public void SetRespawn()
        {
            if (Respawn)
            {
                RaidDamage.Clear();
                Respawn = false;
            }
        }

        public void SetRespawn(bool respawn) => Respawn = respawn;

        public void SetDie() => DieTime = DateTime.Now;

        public void Die()
        {
            DieTime = DateTime.Now;
            InBattle = false;
            Dead = true;
            Respawn = true;
        }

        public void MoveTo(int x, int y)
        {
            PreviousLocation.SetX(CurrentLocation.X);
            PreviousLocation.SetY(CurrentLocation.Y);

            CurrentLocation.SetX(x);
            CurrentLocation.SetY(y);
        }

        public void Move()
        {
            SetInitialLocation();

            if (MoveCount != 0) //TODO: externalizar
            {
                CurrentLocation = new Models.Location(InitialLocation.MapId, InitialLocation.X, InitialLocation.Y);
                MoveCount = 0;
            }
            else
            {
                PreviousLocation.SetX(CurrentLocation.X);
                PreviousLocation.SetY(CurrentLocation.Y);

                double centerX = CurrentLocation.X;
                double centerY = CurrentLocation.Y;
                double radius = 500;

                double angle = UtilitiesFunctions.RandomDouble() * Math.PI * 2;
                var newx = (int)Math.Floor(centerX + radius * Math.Cos(angle));
                var newy = (int)Math.Floor(centerY + radius * Math.Sin(angle));

                CurrentLocation.SetX(newx);
                CurrentLocation.SetY(newy);
                MoveCount++;
            }

            NextWalkTime = DateTime.Now.AddMilliseconds(UtilitiesFunctions.RandomInt(7500, 14000));
        }

        public void SetNextWalkTime(int seconds = 0)
        {
            NextWalkTime = DateTime.Now.AddSeconds(seconds);
        }

        public void SetAgressiveCheckTime(int seconds = 0)
        {
            AgressiveCheckTime = DateTime.Now.AddSeconds(seconds);
        }

        public void SetViewCheckTime(int seconds = 0)
        {
            ViewCheckTime = DateTime.Now.AddSeconds(seconds);
        }

        public void GiveUp()
        {
            RaidDamage.Clear();

            _giveUp = true;
        }

        public void UpdateDeathAndResurrectionTime()
        {
            if (Class == 8)
            {
                DeathTime = DateTime.Now;
                ResurrectionTime = DeathTime.Value.AddSeconds(RespawnInterval);
            }
        }

        public void SetDeathAndResurrectionTime(DateTime? deathTime, DateTime? resurrectionTime)
        {
            DeathTime = deathTime;
            ResurrectionTime = resurrectionTime;
        }
    }
}