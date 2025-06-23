using System.Diagnostics;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonModel
    {
        private int _handlerValue = 0;
        private int _currentHP = 0;
        private int _currentDS = 0;
        private int _currentAS = 0;
        private int _currentMHP = 0;

        private int _properModel => (CurrentType * 128 + 16) << 8;

        private int _fsHp => Character == null ? 0 : Character.HP * FS / 100;
        private int _fsDs => Character == null ? 0 : Character.DS * FS / 100;
        private int _fsDe => Character == null ? 0 : Character.DE * FS / 100;
        private int _fsAt => Character == null ? 0 : Character.AT * FS / 100;
        private int _fsMs => Character == null ? BaseStatus.MSValue : Character.MS;

        private int _baseAs => BaseStatus.ASValue;
        private int _baseAr => BaseStatus.ARValue;
        private int _baseHp => BaseStatus.HPValue;
        private int _baseDs => BaseStatus.DSValue;
        private int _baseAt => BaseStatus.ATValue;
        private short _baseBl => (short)BaseStatus.BLValue;
        private int _baseCc => BaseStatus.CTValue;
        private int _baseCd => 10000; // 100% Critical Damage
        private short _baseAtt => 0;
        private short _baseDe => (short)BaseStatus.DEValue;
        private int _baseEv => BaseStatus.EVValue;
        private short _baseHt => (short)BaseStatus.HTValue;

        public int BuffValueFromBuffSkill { get; set; }
        
        public int DamageShieldHp { get; set; }


        /// <summary>
        /// Current health points.
        /// </summary>
        public int CurrentHp
        {
            get { return _currentHP > HP ? HP : _currentHP < 0 ? 0 : _currentHP; }

            private set { _currentHP = value; }
        }

        /// <summary>
        /// Current digi-soul points.
        /// </summary>
        public int CurrentDs
        {
            get { return _currentDS > DS ? DS : _currentDS < 0 ? 0 : _currentDS; }

            private set { _currentDS = value; }
        }

        /// <summary>
        /// Returns the digimon current general handler.
        /// </summary>
        public ushort GeneralHandler
        {
            get
            {
                byte[] b = new byte[] { (byte)(_handlerValue >> 32 & 0xFF), 0x40 };
                return BitConverter.ToUInt16(b, 0);
            }
        }

        /// <summary>
        /// Returns the current health rate (255 = 100%).
        /// </summary>
        public byte HpRate => (byte)(CurrentHp * 255 / HP);

        /// <summary>
        /// Flag for alive partner.
        /// </summary>
        public bool Alive => CurrentHp > 0;


        /// <summary>
        /// Flag for digimon performing attack.
        /// </summary>
        public bool IsAttacking => EndAttacking > DateTime.Now;

        /// <summary>
        /// Flag for digimon performing skill.
        /// </summary>
        public bool CastingSkill => EndCasting > DateTime.Now;

        /// <summary>
        /// Returns the flag for verifying partner skill.
        /// </summary>
        public bool CheckSkillsTime => DateTime.Now >= LastSkillsCheck;

        public bool IsRaremonType => BaseType == 45172;

        public bool SameType(int baseType) => BaseType == baseType;

        public bool PossibleTranscendence => TranscendenceExperience >= 140000;
        
        public bool IsUnbeatable { get; set; }

        //TODO: deck, encyclopedia
        /// <summary>
        /// Final friendship value.
        /// </summary>
        public byte FS
        {
            get
            {
                var totalFs =
                    Friendship +
                    (Character?.EquipmentAttribute(Friendship, SkillCodeApplyAttributeEnum.FS) ?? 0) +
                    Character?.BuffAttribute(Friendship, SkillCodeApplyAttributeEnum.FS) +
                    BuffAttribute(Friendship,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.FS);

                return (byte)(totalFs > 60 ? 60 : totalFs);
            }
        }

        public int AS
        {
            get
            {
                int calculatedAS = _baseAs -
                                   GetSealStatus(StatusTypeEnum.AS) -
                                   (Character?.AccessoryStatus(AccessoryStatusTypeEnum.AS, _baseAs) ?? 0) -
                                   BuffAttribute(_baseAs,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.AS) -
                                   DeckBuffCalculation(DeckBookInfoTypesEnum.AS, _baseAs) -
                                   (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.AS, _baseAs) ?? 0);

                return Math.Max(calculatedAS, 300); // Limite para que AS seja no mínimo 300
            }
            set { _currentAS = value; }
        }

        public short AR => (short)_baseAr;

        public int AT
        {
            get
            {
                int intValue =
                    _baseAt +
                    (_baseAt * Digiclone.ATValue / 100) +
                    _fsAt +
                    GetSealStatus(StatusTypeEnum.AT) +
                    GetTitleStatus(StatusTypeEnum.AT) +
                    (Character?.AccessoryStatus(AccessoryStatusTypeEnum.AT, 0) ?? 0) +
                    (Character?.ChipsetStatus(_baseAt, SkillCodeApplyAttributeEnum.AT,
                        SkillCodeApplyAttributeEnum.DA) ?? 0) +
                    BuffAttribute(_baseAt,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.AT, SkillCodeApplyAttributeEnum.DA) +
                    (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.AT, _baseAt) ?? 0) +
                    DeckBuffCalculation(DeckBookInfoTypesEnum.AT, _baseAt);

                return intValue;
            }
        }

        public short BL => (short)
            (_baseBl +
             (Digiclone.BLValue) +
             GetSealStatus(StatusTypeEnum.BL) +
             (Character?.AccessoryStatus(AccessoryStatusTypeEnum.BL, 0) ?? 0) +
             BuffAttribute(_baseBl,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.BL));

        public int CC =>
            (_baseCc +
             (_baseCc * Digiclone.CTValue / 100) +
             GetSealStatus(StatusTypeEnum.CT) +
             GetTitleStatus(StatusTypeEnum.CT) +
             (Character?.AccessoryStatus(AccessoryStatusTypeEnum.CT, 0) ?? 0) +
             (Character?.ChipsetStatus(_baseCc, SkillCodeApplyAttributeEnum.CA) ?? 0) +
             BuffAttribute(_baseCc,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.CA));

        public int CD
        {
            get
            {
                int critValue =
                    _baseCd +
                    (Character?.AccessoryStatus(AccessoryStatusTypeEnum.CD, 0) ?? 0) +
                    BuffAttribute(_baseCd,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.CAT) +
                    (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.CD, _baseCd) ?? 0) +
                    DeckBuffCalculation(DeckBookInfoTypesEnum.CD, _baseCd);

                return critValue;
            }
        }

        public int ATT
        {
            get
            {
                int attValue =
                    _baseAtt +
                    (Character?.AccessoryStatus(AccessoryStatusTypeEnum.ATT, 0) ?? 0) +
                    (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.ATT, _baseAtt) ?? 0);

                return attValue;
            }
        }

        public short DE => (short)
            (_baseDe +
             _fsDe +
             GetSealStatus(StatusTypeEnum.DE) +
             GetTitleStatus(StatusTypeEnum.DE) +
             (Character?.AccessoryStatus(AccessoryStatusTypeEnum.DE, 0) ?? 0) +
             (Character?.ChipsetStatus(_baseDe, SkillCodeApplyAttributeEnum.DP) ?? 0) +
             BuffAttribute(_baseDe,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.DP) +
             (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.DE, _baseDe) ?? 0));

        public int DS =>
            _baseDs +
            _fsDs +
            GetSealStatus(StatusTypeEnum.DS) +
            GetTitleStatus(StatusTypeEnum.DS) +
            (Character?.AccessoryStatus(AccessoryStatusTypeEnum.DS, 0) ?? 0) +
            (Character?.ChipsetStatus(_baseDs, SkillCodeApplyAttributeEnum.MaxDS, SkillCodeApplyAttributeEnum.DS) ??
             0) +
            BuffAttribute(_baseDs,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.MaxDS, SkillCodeApplyAttributeEnum.DS) +
            (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.DS, _baseDs) ?? 0);

        public int EV =>
            (_baseEv +
             (_baseEv * Digiclone.EVValue / 100) +
             GetSealStatus(StatusTypeEnum.EV) +
             GetTitleStatus(StatusTypeEnum.EV) +
             (Character?.AccessoryStatus(AccessoryStatusTypeEnum.EV,0) ?? 0) +
             (Character?.ChipsetStatus(_baseEv,SkillCodeApplyAttributeEnum.EV,SkillCodeApplyAttributeEnum.ER) ?? 0) +
             BuffAttribute(_baseEv,BuffValueFromBuffSkill,SkillCodeApplyAttributeEnum.EV,SkillCodeApplyAttributeEnum.ER));


        public short HT => (short)
            (_baseHt +
             GetSealStatus(StatusTypeEnum.HT) +
             GetTitleStatus(StatusTypeEnum.HT) +
             (Character?.AccessoryStatus(AccessoryStatusTypeEnum.HT, 0) ?? 0) +
             (Character?.ChipsetStatus(_baseHt, SkillCodeApplyAttributeEnum.HT) ?? 0) +
             BuffAttribute(_baseHt,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.HT) +
             (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.HT, _baseHt) ?? 0));

        public short SKD
        {
            get
            {
                int calculated = 0 +
                                 (Character?.AccessoryStatus(AccessoryStatusTypeEnum.SCD, 0) ?? 0) +
                                 (Character?.ChipsetStatus(0, SkillCodeApplyAttributeEnum.SkillDamageByAttribute) ?? 0);

                int sentCalculation = calculated == 0 ? 10000 : calculated;
                return (short)(calculated + DeckBuffCalculation(DeckBookInfoTypesEnum.SC, sentCalculation));
            }
        }

        public short SCD
        {
            get
            {
                int calculated = (BuffAttribute(0,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.SCD) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Data, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Vacina, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Virus, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Unknown, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Ice, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Water, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Fire, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Earth, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Wind, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Wood, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Light, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Dark, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Thunder, 0) ?? 0) +
                                  (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.Steel, 0) ?? 0));

                int sentCalculation = calculated == 0 ? 10000 : calculated;
                return (short)(calculated + DeckBuffCalculation(DeckBookInfoTypesEnum.SK, sentCalculation));
            }
        }

        public int HP
        {
            get => _baseHp +
                   _fsHp +
                   (_baseHp * Digiclone.HPValue / 100) +
                   GetSealStatus(StatusTypeEnum.HP) +
                   GetTitleStatus(StatusTypeEnum.HP) +
                   (Character?.AccessoryStatus(AccessoryStatusTypeEnum.HP, 0) ?? 0) +
                   (Character?.ChipsetStatus(_baseHp, SkillCodeApplyAttributeEnum.MaxHP) ?? 0) +
                   BuffAttribute(_baseHp,BuffValueFromBuffSkill, SkillCodeApplyAttributeEnum.MaxHP) +
                   DeckBuffCalculation(DeckBookInfoTypesEnum.HP, _baseHp) +
                   (Character?.DigiviceAccessoryStatus(AccessoryStatusTypeEnum.HP, _baseHp) ?? 0);
            set { _currentMHP = value; }
        }

        public int MS => _fsMs;

        /// <summary>
        /// Returns the current evolution of the partner.
        /// </summary>
        public DigimonEvolutionModel CurrentEvolution => Evolutions.First(x => x.Type == CurrentType);

        /// <summary>
        /// Returns the target attribute buffs value.
        /// </summary>
        /// <param name="baseValue">Base character attribute value.</param>
        /// <param name="attributes">Target attribute params.</param>
             private int BuffAttribute(int baseValue,int buffSkillValue, params SkillCodeApplyAttributeEnum[] attributes)
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
                                totalValue += apply.Value + (buff.TypeN) * apply.AdditionalValue;
                                break;

                            case SkillCodeApplyTypeEnum.Unknown206:
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.SCD)
                                    totalValue += buffSkillValue;
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.CA)
                                    totalValue += buffSkillValue;
                                break;
                            case SkillCodeApplyTypeEnum.Unknown207:
                            case SkillCodeApplyTypeEnum.Unknown208:
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.EV)
                                    totalValue += buffSkillValue * 100;
                                if (apply.Attribute == SkillCodeApplyAttributeEnum.SCD)
                                    totalValue = buffSkillValue * 100;
                                break;

                            case SkillCodeApplyTypeEnum.AlsoPercent:
                            case SkillCodeApplyTypeEnum.Percent:
                            case SkillCodeApplyTypeEnum.Unknown105:
                            {
                                SomaValue += apply.Value + (buff.TypeN) * apply.IncreaseValue;

                                if (apply.Attribute == SkillCodeApplyAttributeEnum.SCD ||
                                    apply.Attribute == SkillCodeApplyAttributeEnum.CAT)
                                {
                                    totalValue = SomaValue * 100;
                                    break;
                                }
                                //else if (apply.Attribute == SkillCodeApplyAttributeEnum.CAT)
                                //{
                                //totalValue = SomaValue;
                                //break;
                                //}

                                //totalValue += (int)Math.Ceiling((double)(SomaValue) / 100 * baseValue);   // Arredonda os valores
                                totalValue += (SomaValue / 100.0) * baseValue;
                            }
                                break;
                        }
                    }
                }
            }

            return (int)totalValue;
        }


        private int DeckBuffCalculation(DeckBookInfoTypesEnum deckBookInfoType, int baseValue)
        {
            var totalValue = 0.0;

            if (Character != null)
            {
                DeckBuffModel? characterDeckBuff = Character.DeckBuff;
                if (characterDeckBuff != null)
                {
                    DeckBuffOptionModel? option =
                        characterDeckBuff.Options.FirstOrDefault(x =>
                            x.DeckBookInfo?.Type == deckBookInfoType &&
                            x.Condition == DeckBuffConditionsEnum.Passive);
                    if (option != null && option.DeckBookInfo != null)
                    {
                        totalValue = baseValue * (option.Value / 100.0);
                        return (int)totalValue;
                    }

                    return 0;
                }

                return 0;
            }

            return 0;
        }

        public int DeckBuffHpCalculation()
        {
            return HP + DeckBuffCalculation(DeckBookInfoTypesEnum.HP, _baseHp);
        }

        public int DeckBuffAsCalculation()
        {
            return AS - DeckBuffCalculation(DeckBookInfoTypesEnum.AS, _baseAs);
        }

        public void SetHp(int value)
        {
            HP = value;
        }

        public void SetAs(int value)
        {
            AS = value;
        }

        /// <summary>
        /// Sets the default basic character information.
        /// </summary>
        private void SetBaseData()
        {
            Level = 1;
            CurrentHp = 5000; // 250
            CurrentDs = 5000; // 100
            LastHitTime = DateTime.MinValue;
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Sets the base information.
        /// </summary>
        public void SetBaseInfo(DigimonBaseInfoAssetModel baseInfo)
        {
            BaseInfo = baseInfo;
        }

        /// <summary>
        /// Updates the current type of the partner.
        /// </summary>
        /// <param name="newType">The new type to be set.</param>
        public void UpdateCurrentType(int newType) => CurrentType = newType;

        public void UpdateDigimonName(string name) => Name = name;

        /// <summary>
        /// Updates the partner model.
        /// </summary>
        /// <param name="newModel">The new model to be set.</param>
        public void UpdateModel(int newModel) => Model = newModel;

        /// <summary>
        /// Creates a new digimon object.
        /// </summary>
        /// <param name="name">Digimon name.</param>
        /// <param name="model">Digimon model.</param>
        /// <param name="type">Digimon type.</param>
        /// <param name="hatchGrade">Digimon hatch grade.</param>
        /// <param name="size">Digimon size.</param>
        public static DigimonModel Create(string name, int model, int type, DigimonHatchGradeEnum hatchGrade,
            short size, byte slot)
        {
            var digimon = new DigimonModel()
            {
                HatchGrade = hatchGrade,
                CurrentType = type,
                Model = model,
                Name = name,
                BaseType = type,
                Size = size,
                Slot = slot,
                Friendship = (byte)GeneralSizeEnum.StartDigimonFriendship
            };

            digimon.SetBaseData();
            digimon.Location = DigimonLocationModel.Create(
                (short)GeneralSizeEnum.StartMapLocation,
                (short)GeneralSizeEnum.StartX,
                (short)GeneralSizeEnum.StartY);

            return digimon;
        }

        /// <summary>
        /// Inserts new evolutions into the list.
        /// </summary>
        /// <param name="evolution">The evolutions to add</param>
        public void AddEvolutions(EvolutionAssetModel evolution)
        {
            var i = 2;
            foreach (var evolutionLine in evolution.Lines)
            {
                Evolutions.Add(new DigimonEvolutionModel(evolutionLine.Type));

                if (i > 0) Evolutions.Last().Unlock();
                i--;
            }
        }

        /// <summary>
        /// Inserts new evolutions into the list.
        /// </summary>
        /// <param name="evolution">The evolutions to add</param>
        public void AddEvolutions(EvolutionAssetDTO evolution)
        {
            var i = 2;
            foreach (var evolutionLine in evolution.Lines)
            {
                Evolutions.Add(new DigimonEvolutionModel(evolutionLine.Type));

                if (i > 0) Evolutions.Last().Unlock();
                i--;
            }
        }

        /// <summary>
        /// Returns title status value from the target attribute.
        /// </summary>
        /// <param name="status">Target attribute.</param>
        private int GetTitleStatus(StatusTypeEnum status)
        {
            if (TitleStatus == null)
                return 0;

            return status switch
            {
                StatusTypeEnum.AT => TitleStatus.ATValue,
                StatusTypeEnum.CT => TitleStatus.CTValue,
                StatusTypeEnum.DE => TitleStatus.DEValue,
                StatusTypeEnum.DS => TitleStatus.DSValue,
                StatusTypeEnum.EV => TitleStatus.EVValue,
                StatusTypeEnum.HP => TitleStatus.HPValue,
                StatusTypeEnum.HT => TitleStatus.HTValue,
                StatusTypeEnum.MS => TitleStatus.MSValue,
                _ => 0,
            };
        }




        

        /// <summary>
        /// Returns seal status value from the target attribute.
        /// </summary>
        /// <param name="status">Target attribute.</param>
        private int GetSealStatus(StatusTypeEnum status)
        {
            return status switch
            {
                StatusTypeEnum.AS => SealStatusList.Sum(x => x.ASValue),
                StatusTypeEnum.AT => SealStatusList.Sum(x => x.ATValue),
                StatusTypeEnum.BL => SealStatusList.Sum(x => x.BLValue) / 10000,
                StatusTypeEnum.CT => SealStatusList.Sum(x => x.CTValue / 100),
                StatusTypeEnum.DE => SealStatusList.Sum(x => x.DEValue),
                StatusTypeEnum.DS => SealStatusList.Sum(x => x.DSValue),
                StatusTypeEnum.EV => SealStatusList.Sum(x => x.EVValue / 100),
                StatusTypeEnum.HP => SealStatusList.Sum(x => x.HPValue),
                StatusTypeEnum.HT => SealStatusList.Sum(x => x.HTValue),
                StatusTypeEnum.MS => SealStatusList.Sum(x => x.MSValue),
                _ => 0,
            };
        }

        /// <summary>
        /// Returns attribute experience value.
        /// </summary>
        public int GetAttributeExperience()
        {
            return BaseInfo.Attribute switch
            {
                DigimonAttributeEnum.None => 0,
                DigimonAttributeEnum.Data => AttributeExperience.Data,
                DigimonAttributeEnum.Vaccine => AttributeExperience.Vaccine,
                DigimonAttributeEnum.Virus => AttributeExperience.Virus,
                DigimonAttributeEnum.Unknown => AttributeExperience.Unknown,
                _ => 0,
            };
        }

        /// <summary>
        /// Returns element experience value.
        /// </summary>
        public int GetElementExperience()
        {
            return BaseInfo.Element switch
            {
                DigimonElementEnum.Ice => AttributeExperience.Ice,
                DigimonElementEnum.Water => AttributeExperience.Water,
                DigimonElementEnum.Fire => AttributeExperience.Fire,
                DigimonElementEnum.Land => AttributeExperience.Land,
                DigimonElementEnum.Wind => AttributeExperience.Wind,
                DigimonElementEnum.Wood => AttributeExperience.Wood,
                DigimonElementEnum.Light => AttributeExperience.Light,
                DigimonElementEnum.Dark => AttributeExperience.Dark,
                DigimonElementEnum.Thunder => AttributeExperience.Thunder,
                DigimonElementEnum.Steel => AttributeExperience.Steel,
                _ => 0,
            };
        }

        /// <summary>
        /// Resources passive regeneration.
        /// </summary>
        public void AutoRegen()
        {
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
        }

        /// <summary>
        /// Receives damage.
        /// </summary>
        /// <param name="damage">Damage to receive.</param>
        /// <returns>Remaining HP.</returns>
        public int ReceiveDamage(int damage)
        {
            if (DamageShieldHp > 0)
            {
                DamageShieldHp -= damage;
                return CurrentHp;
            }

            if (IsUnbeatable)
            {
                return CurrentHp;
            }

            CurrentHp -= damage;
            if (CurrentHp < 0) CurrentHp = 0;

            return CurrentHp;
        }

        /// <summary>
        /// Consume digimon DS.
        /// </summary>
        /// <param name="value">Value to consume.</param>
        /// <returns>Remaining DS.</returns>
        public int UseDs(int value)
        {
            CurrentDs -= value;
            if (CurrentDs < 0) CurrentDs = 0;

            return CurrentDs;
        }

        /// <summary>
        /// Updates the current location position.
        /// </summary>
        /// <param name="newX">New X position.</param>
        /// <param name="newY">New Y position.</param>
        public void NewLocation(int x, int y, float z = 0)
        {
            Location.SetX(x);
            Location.SetY(y);
            Location.SetZ(z);
        }

        /// <summary>
        /// Updates the current location.
        /// </summary>
        /// <param name="mapId">New MapId.</param>
        /// <param name="newX">New X position.</param>
        /// <param name="newY">New Y position.</param>
        public void NewLocation(int mapId, int newX, int newY, bool toEvent = false)
        {
            if (toEvent)
            {
                BeforeEvent.SetMapId(Location.MapId);
                BeforeEvent.SetX(Location.X);
                BeforeEvent.SetY(Location.Y);
            }

            Location.SetMapId((short)mapId);
            Location.SetX(newX);
            Location.SetY(newY);
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
        /// Updates the digimon handler value.
        /// </summary>
        /// <param name="handler">Current map static handler.</param>
        public void SetHandlerValue(short handler) => _handlerValue = _properModel + handler;

        /// <summary>
        /// Transcends the current partner.
        /// </summary>
        public void Transcend() => HatchGrade = DigimonHatchGradeEnum.Transcend;

        /// <summary>
        /// Updates digimon last hit time.
        /// </summary>
        /// <param name="time">Last hit time (in milliseconds).</param>
        public void UpdateLastHitTime(int time = 0) =>
            LastHitTime = DateTime.Now.AddMilliseconds(time); //TODO: NextHitTime?

        public void UpdateTranscendenceExp(long Exp)
        {
            TranscendenceExperience = Exp;
        }

        /// <summary>
        /// Updates the current digimon base status value.
        /// </summary>
        /// <param name="status">New base status</param>
        public void SetBaseStatus(StatusAssetModel status)
        {
            BaseStatus = status;
        }

        /// <summary>
        /// Updates the digimon seal status values.
        /// </summary>
        /// <param name="status">The status to be updated</param>
        public void SetSealStatus(List<SealDetailAssetModel> statusList)
        {
            if (Character == null || !statusList.Any()) return;

            SealStatusList.Clear();

            foreach (var tamerSeal in Character.SealList.Seals)
            {
                var status = statusList
                    .FirstOrDefault(x => x.SealId == tamerSeal.SealId &&
                                         x.RequiredAmount <= tamerSeal.Amount);

                if (status != null) SealStatusList.Add(status);
            }
        }

        /// <summary>
        /// Updates the digimon title status values.
        /// </summary>
        /// <param name="title">The title status to be updated</param>
        public void SetTitleStatus(TitleStatusAssetModel? title)
        {
            if (Character == null) return;

            TitleStatus = title;
        }

        public void SetId(long id)
        {
            Id = id;
        }

        /// <summary>
        /// Updates the digimon's tamer.
        /// </summary>
        /// <param name="tamer">Digimon's tamer.</param>
        public void SetTamer(CharacterModel tamer)
        {
            Character = tamer;
            CharacterId = tamer.Id;
        }

        /// <summary>
        /// Increases current exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveExp(long value) => CurrentExperience += value;

        /// <summary>
        /// Increases current skill exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveSkillExp(int value) => CurrentEvolution.IncreaseSkillExperience(value);

        /// <summary>
        /// Reset current skill exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ResetSkillExp(int value) => CurrentEvolution.RestartSkillExperience(value);

        /// <summary>
        /// Increases current skill points.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveSkillPoint(byte value = 2) => CurrentEvolution.IncreaseSkillPoints(value);

        /// <summary>
        /// Increases current nature exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveNatureExp(short value)
        {
            switch (BaseInfo.Attribute)
            {
                case DigimonAttributeEnum.None: //Não ganha
                    break;
                case DigimonAttributeEnum.Data:
                    break;
                case DigimonAttributeEnum.Vaccine:
                    AttributeExperience.IncreaseAttributeExperience(value, BaseInfo.Attribute);
                    break;
                case DigimonAttributeEnum.Virus:
                    AttributeExperience.IncreaseAttributeExperience(value, BaseInfo.Attribute);
                    break;
                case DigimonAttributeEnum.Unknown:
                {
                    switch (CurrentType)
                    {
                        //TODO: fixo com base na forma atual + equipamento
                        // Rookie 35% | Champion 45% | Ultimate 55% | Mega 65% | Burst Mode is 75%
                        // +equipment (max 100%)

                        default:
                            break;
                    }
                }
                    break;
            }
        }

        /// <summary>
        /// Increases current element exp value.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void ReceiveElementExp(short value)
        {
            AttributeExperience.IncreaseElementExperience(value, BaseInfo.Element);
        }

        /// <summary>
        /// Start partner automatic attack.
        /// </summary>
        public void StartAutoAttack() => AutoAttack = true;

        /// <summary>
        /// Stop partner automatic attack.
        /// </summary>
        public void StopAutoAttack() => AutoAttack = false;

        /// <summary>
        /// Set attack end time.
        /// </summary>
        public void SetEndAttacking(int value = 500) => EndAttacking = DateTime.Now.AddMilliseconds(AS);

        /// <summary>
        /// Set skill cast end time.
        /// </summary>
        /// <param name="time">Timestamp for the skill end</param>
        public void SetEndCasting(int time) => EndCasting = DateTime.Now.AddMilliseconds(500 + time);

        /// <summary>
        /// Increase the digimon level.
        /// </summary>
        /// <param name="levels">Levels to increase.</param>
        public void LevelUp(byte levels = 1)
        {
            if (Level + levels <= (int)GeneralSizeEnum.DigimonLevelMax)
            {
                Level += levels;
                CurrentExperience = 0;

                FullHeal();
            }
        }

        /// <summary>
        /// Updates the partner current HP and DS.
        /// </summary>
        public void FullHeal()
        {
            CurrentHp = HP;
            CurrentDs = DS;
        }

        /// <summary>
        /// Recover digimon HP.
        /// </summary>
        public void RecoverHp(int hpToRecover)
        {
            if (CurrentHp + hpToRecover <= HP)
                CurrentHp += hpToRecover;
            else
                CurrentHp = HP;
        }

        /// <summary>
        /// Recover digimon DS.
        /// </summary>
        public void RecoverDs(int dsToRecover)
        {
            if (CurrentDs + dsToRecover <= DS)
                CurrentDs += dsToRecover;
            else
                CurrentDs = DS;
        }

        /// <summary>
        /// Adjust current HP and current DS upon evolution.
        /// </summary>
        /// <param name="previousHp">Previous current HP</param>
        /// <param name="previousMaxHp">Previous max HP</param>
        /// <param name="previousDs">Previous current DS</param>
        /// <param name="previousMaxDs">Previous max DS</param>
        public void AdjustHpAndDs(
            int previousHp,
            int previousMaxHp,
            int previousDs,
            int previousMaxDs)
        {
            if (previousHp == 0 && previousMaxHp == 0)
                CurrentHp = HP;
            else
                CurrentHp = (previousHp * HP) / previousMaxHp;

            if (previousDs == 0 && previousMaxDs == 0)
                CurrentDs = DS;
            else
                CurrentDs = (previousDs * DS) / previousMaxDs;
        }

        /// <summary>
        /// Restores digimon's HP.
        /// </summary>
        public void RestoreHp(int hp)
        {
            CurrentHp += hp;
        }

        /// <summary>
        /// Restores digimon's DS.
        /// </summary>
        public void RestoreDs(int ds)
        {
            CurrentDs += ds;
        }

        /// <summary>
        /// Decreases digimon experience.
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
        /// Sets the digimon current experience.
        /// </summary>
        /// <param name="value">New value</param>
        public void SetExp(long value)
        {
            CurrentExperience = value;
        }

        /// <summary>
        /// Digimon movimentation logic. //TODO: Remake
        /// </summary>
        /// <param name="wait">Wait cycles.</param>
        /// <param name="newX">New X position.</param>
        /// <param name="newY">new Y position.</param>
        public Task Move(int wait, int newX, int newY)
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
                        if (tempRecalculate)
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
                            ViewLocation.SetX(newX);
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
                ViewLocation.SetX(newX);
                ViewLocation.SetY(newY);
            }

            tempCalculating = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the digimon current level.
        /// </summary>
        /// <param name="level">Current level.</param>
        public void SetLevel(byte level) => Level = level;

        /// <summary>
        /// Enter partner ride mode.
        /// </summary>
        public void StartRideMode()
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Ride;
        }

        /// <summary>
        /// Ends partner ride mode.
        /// </summary>
        public void StopRideMode()
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Default;
        }

        internal void Die()
        {
            UpdateCurrentType(BaseType);
            ReceiveDamage(CurrentHp);
            UseDs(CurrentDs);

            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Die;
        }

        internal void Revive()
        {
            PreviousCondition = CurrentCondition;
            CurrentCondition = ConditionEnum.Default;
        }

        /// <summary>
        /// Updates the current size.
        /// </summary>
        /// <param name="value">New size</param>
        public void SetSize(short value) => Size = value;

        /// <summary>
        /// Updates the digimon current digivice slot.
        /// </summary>
        /// <param name="slot">Target slot. MaxValue for archive</param>
        public void SetSlot(byte slot)
        {
            Slot = slot;
        }
    }
}