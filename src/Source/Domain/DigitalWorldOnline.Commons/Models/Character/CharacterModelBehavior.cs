using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Utils;
using System;
using System.Buffers.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using DigitalWorldOnline.Commons.Models.Config.Events;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterModel
    {
        //Temp
        public bool TempRecalculate { get; set; } //TODO: Remover após abstração da movimentação
        public bool TempCalculating { get; set; } //TODO: Remover após abstração da movimentação
        public DateTime TempUpdating { get; set; } = DateTime.Now; //TODO: Remover após abstração da movimentação

        private int _baseMs => BaseStatus.MSValue + LevelingStatus.MSValue;
        private int _baseHp => LevelingStatus == null ? 0 : LevelingStatus.HPValue;
        private int _baseDs => LevelingStatus == null ? 0 : LevelingStatus.DSValue;
        private int _baseAt => LevelingStatus == null ? 0 : LevelingStatus.ATValue;
        private int _baseDe => LevelingStatus == null ? 0 : LevelingStatus.DEValue;
        private int _baseExp => 1;

        private int _handlerValue;
        private int _targetHandler;
        private int _currentHP = 0;
        private int _currentDS = 0;

        /// <summary>
        /// Current character health points.
        /// </summary>
        public int CurrentHp
        {
            get { return _currentHP > HP ? HP : _currentHP < 0 ? 0 : _currentHP; }

            private set { _currentHP = value; }
        }

        /// <summary>
        /// Current character digi soul.
        /// </summary>
        public int CurrentDs
        {
            get { return _currentDS > DS ? DS : _currentDS < 0 ? 0 : _currentDS; }

            private set { _currentDS = value; }
        }

        /// <summary>
        /// Returns the current character health rate.
        /// </summary>
        public byte HpRate => (byte)(CurrentHp * 255 / HP);

        /// <summary>
        /// Returns the condition result for partner miss hit.
        /// </summary>
        public bool CanMissHit()
        {
            var debuff = TargetMob?.DebuffList.ActiveBuffs.Where(buff => buff.BuffInfo.SkillInfo.Apply.Any(apply =>
                    apply.Attribute == Commons.Enums.SkillCodeApplyAttributeEnum.CrowdControl || !buff.DebuffExpired))
                .ToList();

            if (debuff.Any())
            {
                return false;
            }

            try
            {
                if (TargetMob == null)
                    return true;

                var rand = new Random();

                double TargetEvasion = (double)TargetMob?.EVValue;
                double AttackerHitRate = (double)Partner.HT;

                int levelDifference = Partner.Level - TargetMob.Level;

                if (AttackerHitRate > TargetEvasion || levelDifference > 15)
                {
                    return false; // O Tamer acerta o hit
                }
                else if (levelDifference <= 20)
                {
                    if (Partner.Level >= TargetMob.Level)
                        return false;

                    double attributeAdvantage = 1.5; // Defina o valor do attributeAdvantage conforme necessário

                    if (Partner.BaseInfo.Attribute.HasAttributeAdvantage(TargetMob.Attribute) ||
                        Partner.BaseInfo.Element.HasElementAdvantage(TargetMob.Element))
                        attributeAdvantage = 2.0;

                    if (TargetMob.Attribute.HasAttributeAdvantage(Partner.BaseInfo.Attribute) ||
                        TargetMob.Element.HasElementAdvantage(Partner.BaseInfo.Element))
                        attributeAdvantage = 1.0;

                    double adjustedPercent = CalcularProbabilidadeAcerto(AttackerHitRate, Partner.Level,
                        TargetMob.Level, TargetEvasion, attributeAdvantage);

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

        public bool CanMissHit(bool summon)
        {
            if (TargetSummonMob == null)
                return true;

            var rand = new Random();

            double TargetEvasion = (double)TargetSummonMob?.EVValue;
            double AttackerHitRate = (double)Partner.HT;

            int levelDifference = Partner.Level - TargetSummonMob.Level;

            if (AttackerHitRate > TargetEvasion || levelDifference > 15)
            {
                return false; // O Tamer acerta o hit
            }
            else if (levelDifference <= 20)
            {
                if (Partner.Level >= TargetSummonMob.Level)
                    return false;

                double attributeAdvantage = 1.5; // Defina o valor do attributeAdvantage conforme necessário

                if (Partner.BaseInfo.Attribute.HasAttributeAdvantage(TargetMob.Attribute) ||
                    Partner.BaseInfo.Element.HasElementAdvantage(TargetMob.Element))
                    attributeAdvantage = 2.0;

                if (TargetMob.Attribute.HasAttributeAdvantage(Partner.BaseInfo.Attribute) ||
                    TargetMob.Element.HasElementAdvantage(Partner.BaseInfo.Element))
                    attributeAdvantage = 1.0;

                double adjustedPercent = CalcularProbabilidadeAcerto(AttackerHitRate, Partner.Level,
                    TargetSummonMob.Level, TargetEvasion, attributeAdvantage);

                if (adjustedPercent <= 1.0)
                    adjustedPercent = 0;

                if (adjustedPercent > 100.0)
                    adjustedPercent = 100;

                var TotalChance = (int)adjustedPercent;

                if (TotalChance <= 10)
                    return true;

                return rand.Next(1, 100) >= TotalChance; // Ajuste esse valor conforme necessário
            }


            return true;
        }

        public static double CalcularProbabilidadeAcerto(double seuHitRate, int seuNivel, int nivelDoMonstro, double evDoMonstro, double attributeAdvantage)
        {
            double diferencaDeNiveis = seuNivel - nivelDoMonstro;
            double levelMultiplier = 1 / (1 + Math.Exp(-diferencaDeNiveis / 9.0));
            double probabilidade = levelMultiplier * (seuHitRate / evDoMonstro) * attributeAdvantage * 100;

            return probabilidade;
        }

        /// <summary>
        /// Flag for no active threats.
        /// </summary>
        public bool NoThreats =>
            !TargetMobs.Any(x => x.TargetTamer != null && x.TargetTamer.GeneralHandler == GeneralHandler);

        /// <summary>
        /// Flag for tamer riding state;
        /// </summary>
        public bool Riding => CurrentCondition == ConditionEnum.Ride;

        /// <summary>
        /// Returns the current character target mob.
        /// </summary>
        public MobConfigModel? TargetMob => TargetMobs.FirstOrDefault(x => x.GeneralHandler == _targetHandler);

        /// <summary>
        /// Returns the current character target mob.
        /// </summary>
        public SummonMobModel? TargetSummonMob => TargetSummonMobs.FirstOrDefault(x => x.GeneralHandler == _targetHandler);

        /// <summary>
        /// Returns the current character target mob.
        /// </summary>
        public EventMobConfigModel? TargetEventMob => TargetEventMobs.FirstOrDefault(x => x.GeneralHandler == _targetHandler);

        /// <summary>
        /// Returns the current character target partner.
        /// </summary>
        public DigimonModel? TargetPartner => TargetPartners.FirstOrDefault(x => x.GeneralHandler == _targetHandler);

        /// <summary>
        /// Indicates that the tamer has a valid Aura equiped.
        /// </summary>
        public bool HasAura => Aura.ItemId > 0 &&
                               (Aura.ItemInfo?.UseTimeType == 0 ||
                                (Aura.ItemInfo?.UseTimeType > 0 && Aura.RemainingMinutes() > 0 ||
                                 Aura.RemainingMinutes() != 0xFFFFFFFF));

        /// <summary>
        /// Gets the Aura equipment slot.
        /// </summary>
        public ItemModel Aura => Equipment.Items[10];

        /// <summary>
        /// Returns the tamer equipment items.
        /// </summary>
        public ItemListModel Equipment => ItemList.First(x => x.Type == ItemListEnum.Equipment);

        /// <summary>
        /// Returns the tamer skill list (client receives as Item).
        /// </summary>
        public ItemListModel TamerSkill => ItemList.First(x => x.Type == ItemListEnum.TamerSkill);

        /// <summary>
        /// Returns the tamer inventory items.
        /// </summary>
        public ItemListModel Inventory => ItemList.First(x => x.Type == ItemListEnum.Inventory);


        /// <summary>
        /// Returns the tamer warehouse items.
        /// </summary>
        public ItemListModel Warehouse => ItemList.First(x => x.Type == ItemListEnum.Warehouse);

        /// <summary>
        /// Returns the tamer digivice's chipsets.
        /// </summary>
        public ItemListModel ChipSets => ItemList.First(x => x.Type == ItemListEnum.Chipsets);

        /// <summary>
        /// Returns the tamer digivice's jogress chipset.
        /// </summary>
        public ItemListModel JogressChipSet => ItemList.First(x => x.Type == ItemListEnum.JogressChipset);

        /// <summary>
        /// Returns the tamer digivice.
        /// </summary>
        public ItemListModel Digivice => ItemList.First(x => x.Type == ItemListEnum.Digivice);

        /// <summary>
        /// Returns the tamer reward warehouse items.
        /// </summary>
        public ItemListModel RewardWarehouse => ItemList.First(x => x.Type == ItemListEnum.RewardWarehouse);

        /// <summary>
        /// Returns the tamer gift warehouse items.
        /// </summary>
        public ItemListModel GiftWarehouse => ItemList.First(x => x.Type == ItemListEnum.GiftWarehouse);

        /// <summary>
        /// Returns the character consigned warehouse.
        /// </summary>
        public ItemListModel ConsignedWarehouse => ItemList.First(x => x.Type == ItemListEnum.ConsignedWarehouse);

        /// <summary>
        /// Returns the account warehouse items.
        /// </summary>
        public ItemListModel AccountWarehouse => ItemList.FirstOrDefault(x => x.Type == ItemListEnum.AccountWarehouse);

        /// <summary>
        /// Returns the account cash shop warehouse items.
        /// </summary>
        public ItemListModel AccountShopWarehouse => ItemList.FirstOrDefault(x => x.Type == ItemListEnum.ShopWarehouse);

        /// <summary>
        /// Returns the account cash shop items history.
        /// </summary>
        public ItemListModel AccountBuyHistory => ItemList.FirstOrDefault(x => x.Type == ItemListEnum.BuyHistory);

        /// <summary>
        /// Returns the account cash warehouse items.
        /// </summary>
        public ItemListModel AccountCashWarehouse => ItemList.FirstOrDefault(x => x.Type == ItemListEnum.CashWarehouse);

        /// <summary>
        /// Returns the tamer shop items.
        /// </summary>
        public ItemListModel TamerShop => ItemList.First(x => x.Type == ItemListEnum.TamerShop);

        /// <summary>
        /// Returns the consigned shop items.
        /// </summary>
        public ItemListModel ConsignedShopItems => ItemList.First(x => x.Type == ItemListEnum.ConsignedShop);

        /// <summary>
        /// Returns the tamer's current active digimon.
        /// </summary>
        public DigimonModel Partner => Digimons.OrderBy(x => x.Slot).First();

        /// <summary>
        /// Returns all the tamer's active digimons.
        /// </summary>
        public List<DigimonModel> ActiveDigimons => Digimons.Where(x => x.Id != Partner.Id).ToList();

        /// <summary>
        /// Swaps the current partner.
        /// </summary>
        /// <param name="targetDigimonIndex">New partner</param>
        public void SwitchPartner(byte targetDigimonIndex)
        {
            var currentPartner = Digimons.First(x => x.Slot == 0);
            var targetPartner = Digimons.First(x => x.Slot == targetDigimonIndex);

            currentPartner.SetSlot(targetDigimonIndex);
            targetPartner.SetSlot(0);
        }

        /// <summary>
        /// Returns the correct movement speed value.
        /// </summary>
        public short ProperMS => (short)MS;

        /// <summary>
        /// Gets the tamer's appearence handler, used for visual synchronization packets.
        /// </summary>
        public short AppearenceHandler
        {
            get
            {
                byte[] byteValue = new byte[] { (byte)(_handlerValue >> 32 & 255), 0 };
                return BitConverter.ToInt16(byteValue, 0);
            }
        }

        /// <summary>
        /// Gets the tamer's general handler, used for general packets.
        /// </summary>
        public ushort GeneralHandler
        {
            get
            {
                byte[] b = new byte[] { (byte)((_handlerValue >> 32) & 255), 128 };
                return BitConverter.ToUInt16(b, 0);
            }
        }

        /// <summary>
        /// Flag for alive status;
        /// </summary>
        public bool Alive => Partner.CurrentHp > 0;

        /// <summary>
        /// Returns the flag for updating character resources.
        /// </summary>
        public bool SaveResourcesTime => DateTime.Now >= LastSaveResources;

        /// <summary>
        /// Returns the flag for syncing character resources.
        /// </summary>
        public bool SyncResourcesTime => DateTime.Now >= LastSyncResources;

        /// <summary>
        /// Returns the flag for syncing character resources.
        /// </summary>
        public bool DebuffTime => DateTime.Now >= LastDebuffUpdate;

        /// <summary>
        /// Returns the flag for syncing character daily quests.
        /// </summary>
        public bool ResetDailyQuestsTime => DateTime.Now >= LastDailyQuestCheck;

        /// <summary>
        /// Returns the flag for verifying character buffs.
        /// </summary>
        public bool CheckBuffsTime => DateTime.Now >= LastBuffsCheck;

        public bool CheckExpiredItemsTime => DateTime.Now >= LastExpiredItemsCheck;

        public bool HaveActiveCashSkill =>
            ActiveSkill.FirstOrDefault(x => x.Type == TamerSkillTypeEnum.Cash || x.SkillId > 0) != null;

        public bool IsSpecialMapActive { get; set; }

        /// <summary>
        /// Returns true if the active evolution has broken.
        /// </summary>
        public bool BreakEvolution
        {
            get
            {
                if (IsSpecialMapActive)
                {
                    return true;
                }
                else
                {
                    return (ActiveEvolution.DsPerSecond > 0 && CurrentDs == 0) ||
                           (ActiveEvolution.XgPerSecond > 0 && XGauge == 0);
                }
            }
        }

        /// <summary>
        /// Informs if the tamer has an equipped XAI.
        /// </summary>
        public bool HasXai => Xai != null && Xai.ItemId > 0;

        /// <summary>
        /// Tamer fatigue level (disabled).
        /// </summary>
        public static int Fatigue => 0;

        /// <summary>
        /// Calculates the value for tamer model. //TODO: private?
        /// </summary>
        private int ProperModel
        {
            get
            {
                var modelValue = (Model.GetHashCode() - CharacterModelEnum.MarcusDamon.GetHashCode()) * 128 + 10240160;

                return modelValue << 8;
            }
        }

        // --------------------------------------------------------------------------------------------------

        public int HP => _baseHp +
                         EquipmentAttribute(_baseHp, SkillCodeApplyAttributeEnum.MaxHP) +
                         //SocketAttribute(AccessoryStatusTypeEnum.HP, _baseHp) +
                         BuffAttribute(_baseHp, SkillCodeApplyAttributeEnum.MaxHP) +
                         DigiviceAccessoryStatus(AccessoryStatusTypeEnum.HP, _baseHp) +
                         ChipsetStatus(_baseHp, SkillCodeApplyAttributeEnum.MaxHP);

        public int DS => _baseDs +
                         EquipmentAttribute(_baseDs, SkillCodeApplyAttributeEnum.MaxDS) +
                         //SocketAttribute(AccessoryStatusTypeEnum.DS, _baseDs) +
                         BuffAttribute(_baseDs, SkillCodeApplyAttributeEnum.MaxDS) +
                         DigiviceAccessoryStatus(AccessoryStatusTypeEnum.DS, _baseDs) +
                         ChipsetStatus(_baseDs, SkillCodeApplyAttributeEnum.MaxDS);

        public short AT => (short)
            (_baseAt +
             EquipmentAttribute(_baseAt, SkillCodeApplyAttributeEnum.AT, SkillCodeApplyAttributeEnum.DA) +
             //SocketAttribute(AccessoryStatusTypeEnum.AT, _baseAt) +
             BuffAttribute(_baseAt, SkillCodeApplyAttributeEnum.AT, SkillCodeApplyAttributeEnum.DA) +
             DigiviceAccessoryStatus(AccessoryStatusTypeEnum.AT, _baseAt) +
             ChipsetStatus(_baseAt, SkillCodeApplyAttributeEnum.AT, SkillCodeApplyAttributeEnum.DA));

        public short DE => (short)
            (_baseDe +
             EquipmentAttribute(_baseDe, SkillCodeApplyAttributeEnum.DP) +
             //SocketAttribute(AccessoryStatusTypeEnum.DE, _baseDe) +
             BuffAttribute(_baseDe, SkillCodeApplyAttributeEnum.DP) +
             DigiviceAccessoryStatus(AccessoryStatusTypeEnum.DE, _baseDe) +
             ChipsetStatus(_baseDe, SkillCodeApplyAttributeEnum.DP));

        public int MS
        {
            get
            {
                int calculatedMS = _baseMs +
                                   EquipmentAttribute(_baseMs, SkillCodeApplyAttributeEnum.MS,
                                       SkillCodeApplyAttributeEnum.MovementSpeedComparisonCorrectionBuff,
                                       SkillCodeApplyAttributeEnum.MovementSpeedIncrease) +
                                   //SocketAttribute(AccessoryStatusTypeEnum.MS, _baseMs) +
                                   BuffAttribute(_baseMs, SkillCodeApplyAttributeEnum.MS,
                                       SkillCodeApplyAttributeEnum.MovementSpeedComparisonCorrectionBuff,
                                       SkillCodeApplyAttributeEnum.MovementSpeedIncrease);

                return Math.Min(calculatedMS, 15000); // Max value of MS = 3500
            }
        }

        public int KillExp => _baseExp +
                              EquipmentAttribute(_baseExp, SkillCodeApplyAttributeEnum.EXP) +
                              BuffAttribute(_baseExp, SkillCodeApplyAttributeEnum.EXP);

        public short BonusEXP
        {
            get
            {
                int calculatedBonusEXP = (0 +
                                          EquipmentAttribute(0, SkillCodeApplyAttributeEnum.EXP) +
                                          BuffAttribute(0, SkillCodeApplyAttributeEnum.EXP));

                return (short)calculatedBonusEXP;
            }
        }

        // --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets the default basic character information.
        /// </summary>
        private void SetBaseData()
        {
            StartTimers();

            CurrentCondition = ConditionEnum.Default;

            Location = CharacterLocationModel.Create(
                (short)GeneralSizeEnum.StartMapLocation,
                (short)GeneralSizeEnum.StartX,
                (short)GeneralSizeEnum.StartY);

            Level = 1;
            CurrentHp = 100;
            CurrentDs = 100;
            Size = (short)GeneralSizeEnum.StarterDigimonSize;

            State = CharacterStateEnum.Disconnected;
            EventState = CharacterEventStateEnum.None;

            DigimonSlots = (byte)GeneralSizeEnum.MinActiveDigimonList;

            for (int i = 0; i < 192; i++)
            {
                MapRegions.Add(new CharacterMapRegionModel());
            }
        }

        private void StartTimers()
        {
            LastSyncResources = DateTime.Now.AddSeconds(10);
            LastDailyQuestCheck = DateTime.Now.AddSeconds(60);
            LastBuffsCheck = DateTime.Now.AddSeconds(10);
            LastFatigueUpdate = DateTime.Now.AddSeconds(30);
            LastRegenUpdate = DateTime.Now.AddSeconds(30);
            LastActiveEvolutionUpdate = DateTime.Now.AddSeconds(30);
            LastMovementUpdate = DateTime.Now.AddSeconds(30);
            LastSaveResources = DateTime.Now.AddSeconds(30);
            TimeRewardUpdate = DateTime.Now.AddSeconds(30);
            LastExpiredItemsCheck = DateTime.Now.AddSeconds(30);
            LastExpiredBuffsCheck = DateTime.Now.AddSeconds(30);
            EventQueueInfoTime = DateTime.Now;
            LastAfkNotification = DateTime.Now;
        }

        public bool SetAfk => CurrentCondition != ConditionEnum.Away && AfkNotifications >= 18;

        public void AddAfkNotifications(byte notifications)
        {
            AfkNotifications += notifications;
            LastAfkNotification = DateTime.Now;
        }

        public void AddPoints(int points)
        {
            if (DailyPoints.InsertDate.Day == DateTime.Now.Day)
            {
                DailyPoints.AddPoints(points);
            }
            else
            {
                DailyPoints = new CharacterArenaDailyPointsModel(DateTime.Now, points);
            }
        }


        /// <summary>
        /// Updates the current size.
        /// </summary>
        /// <param name="value">New size</param>
        public void SetSize(short value) => Size = value;

        /// <summary>
        /// Adds a sold item to the repurchase list.
        /// </summary>
        /// <param name="soldItem">The item that has been sold</param>
        public void AddRepurchaseItem(ItemModel soldItem)
        {
            RepurchaseList.Add(soldItem);
        }

        public void UpdateEventQueueInfoTime(int seconds = 30) => EventQueueInfoTime = DateTime.Now.AddSeconds(seconds);

        /// <summary>
        /// Creates a new character object.
        /// </summary>
        /// <param name="accountId">Target account id.</param>
        /// <param name="name">Character name.</param>
        /// <param name="model">Character model enumeration.</param>
        /// <param name="position">Character list position.</param>
        /// <param name="serverId">Target server id.</param>
        public static CharacterModel Create(
            long accountId, string name,
            int model, byte position,
            long serverId)
        {
            var character = new CharacterModel()
            {
                AccountId = accountId,
                Name = name,
                Model = (CharacterModelEnum)model,
                Position = position,
                ServerId = serverId
            };

            character.SetBaseData();

            return character;
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Engages battle with target (mob / Summon / Digimon).
        /// </summary>
        public void StartBattle(MobConfigModel mobConfig)
        {
            if (mobConfig == null)
                return;
            if (!TargetMobs.Any(x => x.Id == mobConfig.Id))
                TargetMobs.Add(mobConfig);

            if (!InBattle)
            {
                _targetHandler = mobConfig.GeneralHandler;
                InBattle = true;
            }
        }

        public void StartBattle(SummonMobModel mobConfig)
        {
            if (TargetSummonMobs == null)
                TargetSummonMobs = new List<SummonMobModel>();

            if (!TargetSummonMobs.Any(x => x.Id == mobConfig.Id))
                TargetSummonMobs.Add(mobConfig);

            if (!InBattle)
            {
                _targetHandler = mobConfig.GeneralHandler;
                InBattle = true;
            }
        }

        public void StartBattle(EventMobConfigModel mobConfig)
        {
            if (TargetEventMobs == null)
                TargetEventMobs = new List<EventMobConfigModel>();

            if (!TargetEventMobs.Any(x => x.Id == mobConfig.Id))
                TargetEventMobs.Add(mobConfig);

            if (!InBattle)
            {
                _targetHandler = mobConfig.GeneralHandler;
                InBattle = true;
            }
        }

        public void StartBattle(DigimonModel enemyPartner)
        {
            if (!TargetPartners.Any(x => x.Id == enemyPartner.Id))
                TargetPartners.Add(enemyPartner);

            if (!InBattle)
            {
                _targetHandler = enemyPartner.GeneralHandler;
                InBattle = true;
            }
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Disables active battle flags.
        /// </summary>
        public void StopBattle()
        {
            _targetHandler = 0;
            TargetMobs.Clear();
            InBattle = false;
        }

        public void StopBattle(bool summon)
        {
            _targetHandler = 0;
            TargetSummonMobs.Clear();
            InBattle = false;
        }

        public void StopBattleDigimon()
        {
            _targetHandler = 0;
            TargetPartners.Clear();
            InBattle = false;
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates the character target (mob / Summon / Digimon).
        /// </summary>
        public void UpdateTarget(MobConfigModel mobConfig)
        {
            if (!TargetMobs.Any(x => x.Id == mobConfig.Id))
                TargetMobs.Add(mobConfig);

            _targetHandler = mobConfig.GeneralHandler;
        }

        public void UpdateTarget(SummonMobModel mobConfig)
        {
            if (!TargetSummonMobs.Any(x => x.Id == mobConfig.Id))
                TargetSummonMobs.Add(mobConfig);

            _targetHandler = mobConfig.GeneralHandler;
        }

        public void UpdateTarget(DigimonModel enemyPartner)
        {
            if (!TargetPartners.Any(x => x.Id == enemyPartner.Id))
                TargetPartners.Add(enemyPartner);

            _targetHandler = enemyPartner.GeneralHandler;
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates the character target (mob / Summon) when use skill.
        /// </summary>
        public void UpdateTargetWithSkill(List<MobConfigModel> mobs, SkillTypeEnum skillType)
        {
            mobs.ForEach(mob =>
            {
                if (!TargetMobs.Any(x => x.Id == mob.Id))
                    TargetMobs.Add(mob);
            });

            switch (skillType)
            {
                case SkillTypeEnum.Single:
                case SkillTypeEnum.TargetArea:
                    _targetHandler = mobs.First().GeneralHandler;
                    break;
            }
        }

        public void UpdateTargetWithSkill(List<SummonMobModel> mobs, SkillTypeEnum skillType)
        {
            mobs.ForEach(mob =>
            {
                if (!TargetSummonMobs.Any(x => x.Id == mob.Id))
                    TargetSummonMobs.Add(mob);
            });

            switch (skillType)
            {
                case SkillTypeEnum.Single:
                case SkillTypeEnum.TargetArea:
                    _targetHandler = mobs.First().GeneralHandler;
                    break;
            }
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Enter partner ride mode.
        /// </summary>
        public void StartRideMode()
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Ride;

            Partner.StartRideMode();

            ResetAfkNotifications();
        }

        /// <summary>
        /// Ends partner ride mode.
        /// </summary>
        public void StopRideMode()
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Default;

            Partner.StopRideMode();

            ResetAfkNotifications();
        }

        // -------------------------------------------------------------------------------------------------

        public void StartBattleWithSkill(List<MobConfigModel> mobs, SkillTypeEnum skillType)
        {
            mobs.ForEach(mob =>
            {
                if (!TargetMobs.Any(x => x.Id == mob.Id))
                    TargetMobs.Add(mob);
            });

            switch (skillType)
            {
                case SkillTypeEnum.Single:
                case SkillTypeEnum.TargetArea:
                    _targetHandler = mobs.First().GeneralHandler;
                    break;
            }

            InBattle = true;
        }

        public void StartBattleWithSkill(List<SummonMobModel> mobs, SkillTypeEnum skillType)
        {
            if (TargetSummonMobs == null)
                TargetSummonMobs = new List<SummonMobModel>();

            mobs.ForEach(mob =>
            {
                if (!TargetSummonMobs.Any(x => x.Id == mob.Id))
                    TargetSummonMobs.Add(mob);
            });

            switch (skillType)
            {
                case SkillTypeEnum.Single:
                case SkillTypeEnum.TargetArea:
                    _targetHandler = mobs.First().GeneralHandler;
                    break;
            }

            InBattle = true;
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Removes the character target mob.
        /// </summary>
        public void RemoveTarget(MobConfigModel mobConfig)
        {
            if (_targetHandler == mobConfig.GeneralHandler)
                _targetHandler = 0;

            TargetMobs.RemoveAll(x => x.Id == mobConfig.Id);

            if (!TargetMobs.Any())
                InBattle = false;
        }

        public void RemoveTarget(SummonMobModel mobConfig)
        {
            if (TargetSummonMobs == null)
                TargetSummonMobs = new List<SummonMobModel>();

            if (_targetHandler == mobConfig.GeneralHandler)
                _targetHandler = 0;

            TargetSummonMobs.RemoveAll(x => x.Id == mobConfig.Id);

            if (!TargetSummonMobs.Any())
                InBattle = false;
        }

        public void RemoveTarget(DigimonModel mobConfig)
        {
            if (_targetHandler == mobConfig.GeneralHandler)
                _targetHandler = 0;

            TargetPartners.RemoveAll(x => x.Id == mobConfig.Id);

            if (!TargetPartners.Any())
                InBattle = false;
        }

        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Updates current exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveExp(long value) => CurrentExperience += value;

        /// <summary>
        /// Increase the character level.
        /// </summary>
        /// <param name="levels">Levels to increase.</param>
        public void LevelUp(byte levels = 1)
        {
            if (Level + levels <= (int)GeneralSizeEnum.TamerLevelMax)
            {
                Level += levels;
                CurrentExperience = 0;

                FullHeal();
            }
        }

        /// <summary>
        /// Updates the tamer current level.
        /// </summary>
        /// <param name="level">The new level</param>
        public void SetLevel(byte level) => Level = level;

        /// <summary>
        /// Decreases character experience.
        /// </summary>
        /// <param name="value">Value to decrease.</param>
        /// <param name="levelDegree">Levels to decrease.</param>
        public void LooseExp(long value, bool levelDegree = false)
        {
            CurrentExperience -= value;

            if (CurrentExperience < 0) CurrentExperience = 0;

            if (levelDegree && Level >= 2) Level--;
        }

        /// <summary>
        /// Sets the character current experience.
        /// </summary>
        /// <param name="value">New value</param>
        public void SetExp(long value)
        {
            CurrentExperience = value;
        }

        /// <summary>
        /// Returns the target attribute equipment value.
        /// </summary>
        /// <param name="baseValue">Base character attribute value.</param>
        /// <param name="attributes">Target attribute params.</param>
        public int EquipmentAttribute(int baseValue, params SkillCodeApplyAttributeEnum[] attributes)
        {
            var totalValue = 0;

            foreach (var item in Equipment.EquippedItems)
            {
                if (item.ItemInfo == null || item.ItemInfo.SkillInfo == null || item.RemainingMinutes() == 0xFFFFFFFF)
                    continue;

                foreach (var apply in item.ItemInfo.SkillInfo.Apply)
                {
                    if (attributes.Any(x => x == apply.Attribute))
                    {
                        switch (apply.Type)
                        {
                            case SkillCodeApplyTypeEnum.Default:
                            {
                                totalValue += apply.Value;
                            }
                                break;

                            case SkillCodeApplyTypeEnum.Unknown105:
                            {
                                totalValue += apply.Value;
                            }
                                break;

                            case SkillCodeApplyTypeEnum.Percent:
                                totalValue += (int)(baseValue * (decimal)apply.Value / 100);
                                break;

                            case SkillCodeApplyTypeEnum.AlsoPercent:
                            {
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.EXP)
                                    totalValue += (apply.Value * item.ItemInfo.TypeN);
                                else
                                    totalValue += (int)(baseValue * 0.10);
                            }
                                break;
                        }
                    }
                }
            }

            return totalValue;
        }

        /// <summary>
        /// Returns the target socket attribute equipment value.
        /// </summary>
        public int SocketAttribute(AccessoryStatusTypeEnum type, int baseValue)
        {
            int totalValue = 0;

            foreach (var item in Equipment.EquippedItems)
            {
                if (item.ItemInfo == null || item.ItemInfo.SkillInfo == null || item.RemainingMinutes() == 0xFFFFFFFF)
                    continue;

                var socketList = item.SocketStatus.Where(socket => socket.Value > 0).ToList();
                var accList = item.AccessoryStatus.Where(acc => acc.Value > 0).ToList();

                if (socketList.Any())
                {
                    bool summedAccValues = false;

                    foreach (var socket in socketList)
                    {
                        //Console.WriteLine($"Socket Item Type: {socket.Type}");
                        //Console.WriteLine($"AttributeID: {socket.AttributeId} | Value: {socket.Value} | Slot: {socket.Slot}");

                        var applyAtt = (double)item.ItemInfo.ApplyElement / 100;

                        if (socket.Type != AccessoryStatusTypeEnum.AS)
                        {
                            if (!summedAccValues)
                            {
                                foreach (var accValue in accList)
                                {
                                    //Console.WriteLine($"Accessory Item Type: {accValue.Type} | Value: {accValue.Value} | Slot: {accValue.Slot}");

                                    var socketValue = (int)(accValue.Value * applyAtt);
                                    totalValue += socketValue;

                                    //Console.WriteLine($"socketValue: {socketValue} | totalValue: {totalValue}");
                                }

                                summedAccValues = true;
                            }
                        }
                        else
                        {
                            foreach (var accValue in accList)
                            {
                                //Console.WriteLine($"AS Socket");

                                var socketValue = (int)(accValue.Value * applyAtt);
                                double socketValuePercent = (double)socketValue / 100;

                                double socketMS = baseValue * socketValuePercent;

                                totalValue += (int)socketMS;
                            }
                        }
                    }
                }
            }

            return totalValue;
        }

        /// <summary>
        /// Returns the target  chipset status value.
        /// </summary>
        /// <param name="type">Target status type.</param>
        public int ChipsetStatus(int baseValue, params SkillCodeApplyAttributeEnum[] attributes)
        {
            int totalValue = 0;

            foreach (var item in ChipSets.EquippedItems)
            {
                if (item.ItemInfo == null || item.ItemInfo.SkillInfo == null)
                    continue;

                if (!IsSameFamily(item))
                    continue;

                foreach (var apply in item.ItemInfo.SkillInfo.Apply)
                {
                    if (attributes.Any(x => x == apply.Attribute))
                    {
                        switch (apply.Type)
                        {
                            case SkillCodeApplyTypeEnum.Default:
                                totalValue += apply.Value;
                                break;

                            case SkillCodeApplyTypeEnum.Unknown105:
                            case SkillCodeApplyTypeEnum.Percent:
                                totalValue += (int)(baseValue * (decimal)apply.Value / 100);
                                break;

                            case SkillCodeApplyTypeEnum.AlsoPercent:
                            {
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.EXP)
                                    totalValue += (apply.Value * item.ItemInfo.TypeN);
                                else
                                    totalValue += (int)(baseValue * 0.10);
                            }
                                break;
                        }
                    }
                }
            }

            return totalValue;
        }

        public bool IsSameFamily(ItemModel item)
        {
            if ((DigimonFamilyEnum)item.FamilyType == Partner.BaseInfo.Family1 ||
                (DigimonFamilyEnum)item.FamilyType == Partner.BaseInfo.Family2 ||
                (DigimonFamilyEnum)item.FamilyType == Partner.BaseInfo.Family3)
            {
                return true;
            }

            if ((DigimonFamilyEnum)item.FamilyType == DigimonFamilyEnum.All)
            {
                return true;
            }

            return false;
        }

        public short DigiviceAccessoryStatus(AccessoryStatusTypeEnum type, int baseValue)
        {
            short totalValue = 0;

            foreach (var item in Digivice.EquippedItems)
            {
                if (!item.HasAccessoryStatus && Level >= item.ItemInfo.TamerMinLevel &&
                    Partner.Level >= item.ItemInfo.DigimonMinLevel)
                    continue;

                foreach (var statusValue in item.AccessoryStatus.Where(x => x.Type == type).Select(x => x.Value))
                {
                    var percent = (decimal)item.Power / 100;

                    if (type == AccessoryStatusTypeEnum.AS || type >= AccessoryStatusTypeEnum.Data)
                    {
                        if (type >= AccessoryStatusTypeEnum.Data)
                        {
                            bool IsPossible = HasAcessoryAttribute(Partner.BaseInfo.Attribute, type) ||
                                              HasAcessoryElement(Partner.BaseInfo.Element, type);
                            if (!IsPossible)
                                break;
                        }

                        var digiPower = (decimal)item.Power / 1000;
                        var reductionValue = baseValue * digiPower; // 1800 * 0,102 = 183,6
                        var newBaseValue = baseValue - (int)reductionValue; // 1800 - 183,6 = 1616,4

                        totalValue += (short)reductionValue;
                        //totalValue += statusValue;
                    }
                    else if (type == AccessoryStatusTypeEnum.CT || type == AccessoryStatusTypeEnum.EV ||
                             type == AccessoryStatusTypeEnum.ATT)
                    {
                        totalValue += (short)(statusValue * percent * 100);
                    }
                    else if (type == AccessoryStatusTypeEnum.CD)
                    {
                        //totalValue = (short)(statusValue / 100);
                        totalValue += (short)((statusValue / 100) * item.Power);
                    }
                    else
                    {
                        totalValue += (short)(percent * statusValue);
                    }
                }
            }

            return totalValue;
        }

        /// <summary>
        /// Returns the target accessory status value.
        /// </summary>
        /// <param name="type">Target status type.</param>
        public short AccessoryStatus(AccessoryStatusTypeEnum type, int baseValue)
        {
            short totalValue = 0;

            foreach (var item in Equipment.EquippedItems)
            {
                if (!item.HasAccessoryStatus && Level >= item.ItemInfo.TamerMinLevel &&
                    Partner.Level >= item.ItemInfo.DigimonMinLevel)
                    continue;

                foreach (var statusValue in item.AccessoryStatus.Where(x => x.Type == type).Select(x => x.Value))
                {
                    decimal percent = 0;

                    if (item.HasSocketStatus)
                        percent = 0;
                    else
                        percent = (decimal)item.Power / 100;

                    if (type == AccessoryStatusTypeEnum.AS || type >= AccessoryStatusTypeEnum.Data)
                    {
                        if (type >= AccessoryStatusTypeEnum.Data)
                        {
                            if (!HasAcessoryAttribute(Partner.BaseInfo.Attribute, type) ||
                                !HasAcessoryElement(Partner.BaseInfo.Element, type))
                                break;
                        }

                        var percentValue = (decimal)statusValue / 100;

                        totalValue += (short)((percent * percentValue * baseValue) / 100);
                    }
                    else if (type == AccessoryStatusTypeEnum.CT || type == AccessoryStatusTypeEnum.EV ||
                             type == AccessoryStatusTypeEnum.ATT)
                    {
                        totalValue += (short)(statusValue * percent * 100);
                    }
                    else if (type == AccessoryStatusTypeEnum.CD)
                    {
                        totalValue += (short)((statusValue / 100) * item.Power);
                    }
                    else
                    {
                        totalValue += (short)(percent * statusValue);
                    }
                }
            }

            return totalValue;
        }

        public static bool HasAcessoryAttribute(DigimonAttributeEnum hitter, AccessoryStatusTypeEnum accessory)
        {
            switch (accessory)
            {
                case AccessoryStatusTypeEnum.Data:
                    return hitter == DigimonAttributeEnum.Data;
                case AccessoryStatusTypeEnum.Vacina:
                    return hitter == DigimonAttributeEnum.Vaccine;
                case AccessoryStatusTypeEnum.Virus:
                    return hitter == DigimonAttributeEnum.Virus;
                case AccessoryStatusTypeEnum.Unknown:
                    return hitter == DigimonAttributeEnum.Unknown;
                default:
                    return false;
            }
        }

        public static bool HasAcessoryElement(DigimonElementEnum hitter, AccessoryStatusTypeEnum accessory)
        {
            return accessory == AccessoryStatusTypeEnum.Ice && hitter == DigimonElementEnum.Ice ||
                   accessory == AccessoryStatusTypeEnum.Water && hitter == DigimonElementEnum.Water ||
                   accessory == AccessoryStatusTypeEnum.Fire && hitter == DigimonElementEnum.Fire ||
                   accessory == AccessoryStatusTypeEnum.Earth && hitter == DigimonElementEnum.Land ||
                   accessory == AccessoryStatusTypeEnum.Wind && hitter == DigimonElementEnum.Wind ||
                   accessory == AccessoryStatusTypeEnum.Wood && hitter == DigimonElementEnum.Wood ||
                   accessory == AccessoryStatusTypeEnum.Light && hitter == DigimonElementEnum.Light ||
                   accessory == AccessoryStatusTypeEnum.Dark && hitter == DigimonElementEnum.Dark ||
                   accessory == AccessoryStatusTypeEnum.Thunder && hitter == DigimonElementEnum.Thunder ||
                   accessory == AccessoryStatusTypeEnum.Steel && hitter == DigimonElementEnum.Steel;
        }

        /// <summary>
        /// Returns the target attribute buffs value.
        /// </summary>
        /// <param name="baseValue">Base character attribute value.</param>
        /// <param name="attributes">Target attribute params.</param>
        public int BuffAttribute(int baseValue, params SkillCodeApplyAttributeEnum[] attributes)
        {
            var totalValue = 0.0;
            var SomaValue = 0.0;

            foreach (var buff in BuffList.ActiveBuffs)
            {
                if (buff.BuffInfo == null || buff.BuffInfo.SkillInfo == null)
                    continue;

                foreach (var apply in buff.BuffInfo.SkillInfo.Apply)
                {
                    //Console.WriteLine($"apply.Type: {apply.Type} | apply.Attribute: {apply.Attribute}");

                    if (attributes.Any(x => x == apply.Attribute))
                    {
                        switch (apply.Type)
                        {
                            case SkillCodeApplyTypeEnum.Default:
                                totalValue += apply.Value;
                                break;

                            case SkillCodeApplyTypeEnum.AlsoPercent:
                            case SkillCodeApplyTypeEnum.Percent:
                            {
                                SomaValue += apply.Value + (buff.TypeN) * apply.IncreaseValue;

                                if (apply.Attribute == SkillCodeApplyAttributeEnum.SCD)
                                {
                                    totalValue = SomaValue * 100;
                                    break;
                                }
                                else if (apply.Attribute == SkillCodeApplyAttributeEnum.CAT ||
                                         apply.Attribute == SkillCodeApplyAttributeEnum.EXP)
                                {
                                    totalValue = SomaValue;
                                    break;
                                }

                                //totalValue = (int)Math.Ceiling((double)(SomaValue) / 100 * baseValue);    // arredonda valores
                                totalValue += (SomaValue / 100.0) * baseValue;
                            }
                                break;
                        }
                    }
                }
            }

            return (int)totalValue;
        }

        /// <summary>
        /// Sets the character as dead.
        /// </summary>
        public void Die()
        {
            InBattle = false;
            Dead = true;

            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Die;

            CurrentHp = 0;
            CurrentDs = 0;
            ActiveEvolution.SetDs(0);
            ActiveEvolution.SetXg(0);

            Partner.Die();
        }

        /// <summary>
        /// Sets the character as alive.
        /// </summary>
        public void Revive()
        {
            if (!Alive)
            {
                Dead = false;

                CurrentHp = HP / 4;
                CurrentDs = DS / 5;
                Partner.RestoreHp(Partner.HP / 3);
                Partner.RestoreDs(Partner.DS / 4);
            }

            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Default;

            Partner.Revive();
        }

        /// <summary>
        /// Adds a new digimon to the list.
        /// </summary>
        /// <param name="digimon">The digimon to add.</param>
        public void AddDigimon(DigimonModel digimon) => Digimons.Add(digimon);

        /// <summary>
        /// Update target digimon by slot.
        /// </summary>
        /// <param name="digimon">New digimon</param>
        /// <param name="slot">Target slot</param>
        public void UpdateDigimon(DigimonModel digimon, long digimonId)
        {
            var target = Digimons.First(x => x.Id == digimonId);
            target = digimon;
        }

        /// <summary>
        /// Adds a new friend to the list.
        /// </summary>
        /// <param name="friend">The friend to add.</param>
        public void AddFriend(CharacterFriendModel friend) => Friends.Add(friend);

        /// <summary>
        /// Updates the current tamer title.
        /// </summary>
        /// <param name="newTitleId">The new title id</param>
        public void UpdateCurrentTitle(short newTitleId) => CurrentTitle = newTitleId;

        /// <summary>
        /// Updates the current tamer deck buff id.
        /// </summary>
        /// <param name="deckBuffId">Set the new deck buff</param>
        /// <param name="deckBuffModel"></param>
        public void UpdateDeckBuffId(int? deckBuffId, DeckBuffModel? deckBuffModel)
        {
            DeckBuffId = deckBuffId;
            DeckBuff = deckBuffModel;
        }

        /// <summary>
        /// Updates the current tamer title.
        /// </summary>
        /// <param name="sendOnceSent">The new title id</param>
        public void UpdateInitialPacketSentOnceSent(bool sendOnceSent) => InitialPacketSentOnceSent = sendOnceSent;

        /// <summary>
        /// Adds a new ItemList to the current object list.
        /// </summary>
        /// <param name="itemList">The new ItemList</param>
        public void AddItemList(ItemListModel itemList) => ItemList.Add(itemList);

        /// <summary>
        /// Verifies every item expiration time and sets the
        /// proper duration value for expired items.
        /// </summary>
        public void CheckExpiredItems()
        {
        }

        /// <summary>
        /// Set the handler value based on the current map.
        /// </summary>
        /// <param name="mapHandler">The current map handler value.</param>
        /// <returns>The character instance.</returns>
        public void SetHandlerValue(short mapHandler)
        {
            _handlerValue = ProperModel + mapHandler;
        }

        public void SetLastExpiredItemsCheck() => LastExpiredItemsCheck = DateTime.Now.AddSeconds(60);

        /// <summary>
        /// Restores the previous character condition.
        /// </summary>
        public void RestorePreviousCondition() => CurrentCondition = PreviousCondition;

        /// <summary>
        /// Updates the character current condiction and saves the previous one.
        /// </summary>
        /// <param name="condition"></param>
        public void UpdateCurrentCondition(ConditionEnum condition)
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = condition;
        }

        /// <summary>
        /// Updates character target handler.
        /// </summary>
        /// <param name="handler">New handler.</param>
        public void UpdateTargetHandler(int handler) => TargetHandler = handler;

        /// <summary>
        /// Updates character shop item id.
        /// </summary>
        /// <param name="shopItemId">New shop item id.</param>
        public void UpdateShopItemId(int shopItemId) => ShopItemId = shopItemId > 0 ? shopItemId : ShopItemId;

        /// <summary>
        /// Updates character's shop name.
        /// </summary>
        /// <param name="shopName">New shop name.</param>
        public void UpdateShopName(string shopName) => ShopName = shopName;

        /// <summary>
        /// Updates the character's save resources timer.
        /// </summary>
        public void UpdateSaveResourcesTime(int seconds = 20) => LastSaveResources = DateTime.Now.AddSeconds(seconds);

        /// <summary>
        /// Updates the character's sync resources timer.
        /// </summary>
        public void UpdateSyncResourcesTime() => LastSyncResources = DateTime.Now.AddSeconds(5);

        public void UpdateDebuffTime() => LastDebuffUpdate = DateTime.Now.AddSeconds(5);

        /// <summary>
        /// Updates the character's daily quest sync timer.
        /// </summary>
        public void UpdateDailyQuestsSyncTime() => LastDailyQuestCheck = DateTime.Now.AddSeconds(60);

        /// <summary>
        /// Updates the character's buffs check timer.
        /// </summary>
        public void UpdateBuffsCheckTime() => LastBuffsCheck = DateTime.Now.AddSeconds(2);


        /// <summary>
        /// Passive resources regeneration.
        /// </summary>
        public void AutoRegen()
        {
            if (!Dead && !InBattle && DateTime.Now >= LastRegenUpdate.AddSeconds(10)) //TODO: externalizar?
            {
                LastRegenUpdate = DateTime.Now;

                if (CurrentHp < HP)
                {
                    CurrentHp += (int)Math.Ceiling(HP * 0.01);
                    if (CurrentHp > HP) CurrentHp = HP;
                }

                if (CurrentDs < DS)
                {
                    CurrentDs += (int)Math.Ceiling(DS * 0.01);
                    if (CurrentDs > DS) CurrentDs = DS;
                }

                Partner.AutoRegen();
            }
        }

        /// <summary>
        /// Fully heals character's HP and DS.
        /// </summary>
        public void FullHeal()
        {
            CurrentHp = HP;
            CurrentDs = DS;
        }

        /// <summary>
        /// Recover character HP.
        /// </summary>
        public void RecoverHp(int hpToRecover)
        {
            if (CurrentHp + hpToRecover <= HP)
                CurrentHp += hpToRecover;
            else
                CurrentHp = HP;
        }

        /// <summary>
        /// Recover character DS.
        /// </summary>
        public void RecoverDs(int dsToRecover)
        {
            if (CurrentDs + dsToRecover <= DS)
                CurrentDs += dsToRecover;
            else
                CurrentDs = DS;
        }

        /// <summary>
        /// Resources reduction for character's active evolution.
        /// </summary>
        public void ActiveEvolutionReduction()
        {
            if (!Dead && DateTime.Now >= LastActiveEvolutionUpdate.AddSeconds(5))
            {
                LastActiveEvolutionUpdate = DateTime.Now;

                if (ActiveEvolution.DsPerSecond > 0 && !HasAura)
                {
                    if (ActiveEvolution.DsPerSecond > CurrentDs)
                        ConsumeDs(CurrentDs);
                    else
                        ConsumeDs(ActiveEvolution.DsPerSecond);
                }

                if (ActiveEvolution.XgPerSecond > 0)
                {
                    if (ActiveEvolution.XgPerSecond > XGauge)
                        ConsumeXg(XGauge);
                    else
                        ConsumeXg(ActiveEvolution.XgPerSecond);
                }
            }
        }

        /// <summary>
        /// Reduces character DS.
        /// </summary>
        /// <param name="value">Value to reduce.</param>
        /// <returns>True if it's possible to reduce, false if it's not.</returns>
        public bool ConsumeDs(int value)
        {
            if (CurrentDs >= value)
            {
                CurrentDs -= value;
                if (CurrentDs <= 0) CurrentDs = 0;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reduces character XGauge.
        /// </summary>
        /// <param name="value">Value to reduce.</param>
        /// <returns>True if it's possible to reduce, false if it's not.</returns>
        public bool ConsumeXg(int value)
        {
            if (XGauge >= value)
            {
                XGauge -= value;
                if (XGauge <= 0)
                {
                    XGauge = 0;
                    return false;
                }
                else return true;
            }

            return false;
        }

        public bool ConsumeXCrystal(short value)
        {
            if (XCrystals >= value)
            {
                XCrystals -= value;
                if (XCrystals <= 0)
                {
                    XCrystals = 0;
                    return false;
                }
                else return true;
            }

            return false;
        }


        /// <summary>
        /// Gets the tamer's passive skill buff.
        /// </summary>
        /// <returns>The passive buff of the tamer relative to the current partner</returns>
        public void SetPartnerPassiveBuff(CharacterModelEnum model = CharacterModelEnum.Unknow)
        {
            if (model == CharacterModelEnum.Unknow)
                model = Model;

            switch (model)
            {
                case CharacterModelEnum.MarcusDamon:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Data:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40212, 8000131));
                            break;

                        case DigimonAttributeEnum.Vaccine:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40211, 8000121));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.ThomasNorstein:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Data:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40214, 8000221));
                            break;

                        case DigimonAttributeEnum.Virus:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40215, 8000231));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.YoshinoFujieda:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Data:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40217, 8000321));
                            break;

                        case DigimonAttributeEnum.Vaccine:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40218, 8000331));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.KeenanKrier:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40221, 8000431));
                            break;

                        case DigimonAttributeEnum.Vaccine:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40220, 8000421));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.TaiKamiya:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40224, 8000531));
                            break;

                        case DigimonAttributeEnum.Vaccine:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40223, 8000521));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.MimiTachikawa:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Data:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40226, 8000621));
                            break;

                        case DigimonAttributeEnum.None:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40227, 8000631));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.MattIshida:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40229, 8000721));
                            break;

                        case DigimonAttributeEnum.Data:
                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40230, 8000731));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.TakeruaKaishi:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40232, 8000821));
                            break;

                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40233, 8000831));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.HikariKamiya:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40235, 8000921));
                            break;

                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40236, 8000931));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.SoraTakenoushi:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40238, 8001021));
                            break;

                        case DigimonAttributeEnum.Unknown:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40239, 8001031));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.IzzyIzumi:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40275, 8001421));
                            break;

                        case DigimonAttributeEnum.Unknown:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40276, 8001431));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.JoeKido:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40278, 8001521));
                            break;

                        case DigimonAttributeEnum.Unknown:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40279, 8001531));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.TakatoMatsuki:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40282, 8001621));
                            break;

                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40283, 8001631));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.RikaNonaka:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Data:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40285, 8001721));
                            break;

                        case DigimonAttributeEnum.Virus:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40286, 8001731));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.HenryWong:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40288, 8001821));
                            break;

                        case DigimonAttributeEnum.Data:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40289, 8001831));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.KatoJeri:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40294, 8001921));
                            break;

                        case DigimonAttributeEnum.Unknown:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40295, 8001931));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.AkiyamaRyo:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40297, 8002021));
                            break;

                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(
                                DigimonBuffModel.Create(40298, 8002031));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.HiroAmanokawa:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40300, 8002121));
                            break;

                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40301, 8002131));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.RuliTsukiyono:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40303, 8002221));
                            break;

                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40304, 8002231));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.KiyoshirouHigashimitarai:
                {
                    switch (Partner.BaseInfo.Attribute)
                    {
                        case DigimonAttributeEnum.Vaccine:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40306, 8002321));
                            break;

                        case DigimonAttributeEnum.None:

                            Partner.BuffList.Add(DigimonBuffModel.Create(40307, 8002331));
                            break;
                    }
                }
                    break;

                case CharacterModelEnum.KanbaraTakuya:
                    break;

                case CharacterModelEnum.KoichiKimura:
                    break;

                case CharacterModelEnum.Cleiton:
                    break;
            }
        }

        public void RemovePartnerPassiveBuff()
        {
            var targetPassiveBuff = Partner.BuffList.TamerBaseSkill();

            if (targetPassiveBuff != null)
            {
                Partner.BuffList.ForceExpired(targetPassiveBuff.BuffId);
                Partner.BuffList.Remove(targetPassiveBuff.BuffId);
            }
        }

        /// <summary>
        /// Set the tamer's current channel.
        /// </summary>
        /// <param name="channel">The target channel</param>
        public void SetCurrentChannel(byte? channel) => Channel = channel ?? 0;

        /// <summary>
        /// Set the tamer's current view state as hidden.
        /// </summary>
        /// <param name="hidden">The view state</param>
        public void SetHidden(bool hidden) => Hidden = hidden;

        /// <summary>
        /// Set the tamer's god mode state.
        /// </summary>
        /// <param name="enabled">The god mode state</param>
        public void SetGodMode(bool enabled) => GodMode = enabled;

        /// <summary>
        /// Updates the tamer state.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void UpdateState(CharacterStateEnum state) => State = state;

        /// <summary>
        /// Updates the tamer event state.
        /// </summary>
        /// <param name="state">The new event state.</param>
        public void UpdateEventState(CharacterEventStateEnum state) => EventState = state;

        public void UpdateName(string name) => Name = name;

        /// <summary>
        /// Updates the tamer base status values.
        /// </summary>
        /// <param name="status">The status to be updated</param>
        public void SetBaseStatus(CharacterBaseStatusAssetModel status) => BaseStatus = status;

        /// <summary>
        /// Updates the tamer level status values.
        /// </summary>
        /// <param name="status">The status to be updated</param>
        public void SetLevelStatus(CharacterLevelStatusAssetModel status) => LevelingStatus = status;

        /// <summary>
        /// Updates the tamer guild.
        /// </summary>
        /// <param name="guild">Guild to update</param>
        public void SetGuild(GuildModel guild = null) => Guild = guild;

        /// <summary>
        /// Updates the location time tracker.
        /// </summary>
        public void MovementUpdated() => LastMovementUpdate = DateTime.Now;

        /// <summary>
        /// Updates the location.
        /// </summary>
        public void NewLocation(int x, int y, float z = 0)
        {
            Location.SetX(x);
            Location.SetY(y);
            Location.SetZ(z);
        }

        /// <summary>
        /// Updates the view location.
        /// </summary>
        public void NewViewLocation(int x, int y)
        {
            ViewLocation.SetX(x);
            ViewLocation.SetY(y);
        }

        /// <summary>
        /// Set the current tamer's XAI details.
        /// </summary>
        public void SetXai(CharacterXaiModel? xai) => Xai = xai;

        public void SetXGauge(int xGauge)
        {
            XGauge += xGauge;

            if (XGauge > Xai.XGauge)
                XGauge = Xai.XGauge;
        }

        public void SetXCrystals(int xCrystals)
        {
            XCrystals += (short)xCrystals;

            if (XCrystals > Xai.XCrystals)
                XCrystals = Xai.XCrystals;
        }

        /// <summary>
        /// Updates the current reward and time of the time reward.
        /// </summary>
        public void UpdateTimeReward()
        {
            TimeReward.RewardIndex++;
            TimeReward.CurrentTime = 0;
        }

        /// <summary>
        /// Character movimentation logic. //TODO: Remake
        /// </summary>
        /// <param name="wait">Wait cycles.</param>
        /// <param name="newX">New X position.</param>
        /// <param name="newY">new Y position.</param>
        public void Move(int wait, int newX, int newY)
        {
            //TODO: Ficou feio, refazer
            if (wait > 0)
            {
                var baseSplitter = 32;

                var octers = wait / baseSplitter;

                if (octers > 0)
                {
                    var qtd = baseSplitter;
                    while (qtd > 0)
                    {
                        if (TempRecalculate)
                            break;

                        Thread.Sleep(octers);
                        qtd--;

                        if (ViewLocation.X > newX)
                        {
                            var diffX = (ViewLocation.X - newX) / baseSplitter;
                            ViewLocation.SetX(ViewLocation.X - diffX);
                        }
                        else
                        {
                            var diffX = (newX - ViewLocation.X) / baseSplitter;
                            ViewLocation.SetX(ViewLocation.X + diffX);
                        }

                        if (ViewLocation.Y > newY)
                        {
                            var diffY = (ViewLocation.Y - newY) / baseSplitter;
                            ViewLocation.SetY(ViewLocation.Y - diffY);
                        }
                        else
                        {
                            var diffY = (newY - ViewLocation.Y) / baseSplitter;
                            ViewLocation.SetY(ViewLocation.Y + diffY);
                        }

                        if (qtd <= 0)
                        {
                            ViewLocation.SetX(newY);
                            ViewLocation.SetY(newY);
                        }
                    }
                }
                else
                    Thread.Sleep(wait);
            }
            else
            {
                Thread.Sleep(500);
                ViewLocation.SetX(newY);
                ViewLocation.SetY(newY);
            }

            TempCalculating = false;
        }

        /// <summary>
        /// Updates the character current location.
        /// </summary>
        /// <param name="mapId">New map id.</param>
        /// <param name="x">New X position.</param>
        /// <param name="y">New Y position.</param>
        public void NewLocation(int mapId, int x, int y, bool toEvent = false)
        {
            if (toEvent)
            {
                BeforeEvent.SetMapId(Location.MapId);
                BeforeEvent.SetX(Location.X);
                BeforeEvent.SetY(Location.Y);
            }

            Location.SetMapId((short)mapId);
            Location.SetX(x);
            Location.SetY(y);
        }

        /// <summary>
        /// Serializes the map regions objects.
        /// </summary>
        public byte[] SerializeMapRegion()
        {
            using var m = new MemoryStream();
            foreach (var region in MapRegions)
                m.Write(region.ToArray(), 0, 1);

            return m.ToArray();
        }

        public CharacterModel SetDeckBuff(DeckBuffModel? deckBuffModel)
        {
            DeckBuff = deckBuffModel;
            return this;
        }
    }
}
