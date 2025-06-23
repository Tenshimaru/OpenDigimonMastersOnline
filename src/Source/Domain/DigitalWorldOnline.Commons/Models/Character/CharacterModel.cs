using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Model.Character;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Config.Events;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Events;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Models.TamerShop;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Account identifier.
        /// </summary>
        public long AccountId { get; private set; }

        /// <summary>
        /// Character selection position.
        /// </summary>
        public byte Position { get; private set; }

        /// <summary>
        /// Character model.
        /// </summary>
        public CharacterModelEnum Model { get; private set; }

        /// <summary>
        /// Character creation date.
        /// </summary>
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// Current level.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Character name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Character size.
        /// </summary>
        public short Size { get; private set; }

        /// <summary>
        /// Character connection state.
        /// </summary>
        public CharacterStateEnum State { get; private set; }

        /// <summary>
        /// Character event state.
        /// </summary>
        public CharacterEventStateEnum EventState { get; private set; }

        /// <summary>
        /// Character server reference.
        /// </summary>
        public long ServerId { get; private set; }

        /// <summary>
        /// Current experience value.
        /// </summary>
        public long CurrentExperience { get; private set; }

        /// <summary>
        /// Current connected channel.
        /// </summary>
        public byte Channel { get; private set; }

        /// <summary>
        /// Current connected channel.
        /// </summary>
        public int TargetTamerIdTP { get; private set; }

        /// <summary>
        /// Total active digimon slots.
        /// </summary>
        public byte DigimonSlots { get; private set; }

        /// <summary>
        /// Current XGauge amount.
        /// </summary>
        public int XGauge { get; private set; }

        /// <summary>
        /// Current XCrystals amount.
        /// </summary>
        public short XCrystals { get; private set; }

        /// <summary>
        /// Current display tittle.
        /// </summary>
        public short CurrentTitle { get; private set; }

        /// <summary>
        /// Current deck buff id
        /// </summary>
        public int? DeckBuffId { get; private set; }

        /// <summary>
        /// Current display InitialPacketSentOnceSent.
        /// </summary>
        public bool InitialPacketSentOnceSent { get; private set; }

        /// <summary>
        /// Character inventories.
        /// </summary>
        public List<ItemListModel> ItemList { get; private set; }


        /// <summary>
        /// Owned digimons.
        /// </summary>
        public List<DigimonModel> Digimons { get; private set; }

        /// <summary>
        /// Character friends.
        /// </summary>
        public List<CharacterFriendModel> Friends { get; private set; }

        /// <summary>
        /// Character friends.
        /// </summary>
        public List<CharacterFriendModel> Friended { get; private set; }

        /// <summary>
        /// Character Arena Points.
        /// </summary>
        public CharacterArenaPointsModel Points { get; private set; }


        /// <summary>
        /// Character blocked tamers.
        /// </summary>
        public List<CharacterFoeModel> Foes { get; private set; }


        /// <summary>
        /// Character blocked tamers.
        /// </summary>
        public List<CharacterFoeModel> Foed { get; private set; }

        /// <summary>
        /// Current buff list.
        /// </summary>
        public CharacterBuffListModel BuffList { get; private set; }

        /// <summary>
        /// Current location.
        /// </summary>
        public CharacterLocationModel Location { get; private set; }

        /// <summary>
        /// Character incubator details.
        /// </summary>
        public CharacterIncubatorModel Incubator { get; private set; }

        /// <summary>
        /// Character map region collection.
        /// </summary>
        public List<CharacterMapRegionModel> MapRegions { get; private set; }

        /// <summary>
        /// Character base status details.
        /// </summary>
        public CharacterBaseStatusAssetModel BaseStatus { get; private set; }

        /// <summary>
        /// Character leveling status details.
        /// </summary>
        public CharacterLevelStatusAssetModel? LevelingStatus { get; private set; }

        /// <summary>
        /// Character seal list.
        /// </summary>
        public CharacterSealListModel SealList { get; private set; }

        /// <summary>
        /// Character XAI item.
        /// </summary>
        public CharacterXaiModel Xai { get; private set; }

        /// <summary>
        /// Character progression details.
        /// </summary>
        public CharacterProgressModel Progress { get; private set; }

        /// <summary>
        /// Character time reward event details.
        /// </summary>
        public TimeRewardModel TimeReward { get; private set; }

        public AttendanceRewardModel AttendanceReward { get; set; }

        public CharacterArenaDailyPointsModel DailyPoints { get; set; }

        /// <summary>
        /// Character consigned shop details.
        /// </summary>
        public ConsignedShop ConsignedShop { get; private set; }

        /// <summary>
        /// Character's partner active evolution.
        /// </summary>
        public CharacterActiveEvolutionModel ActiveEvolution { get; private set; }

        /// <summary>
        /// Character's digimon archive.
        /// </summary>
        public CharacterDigimonArchiveModel DigimonArchive { get; private set; }

        /// <summary>
        /// Character's digimon academy.
        /// </summary>
        public CharacterDigimonAcademyModel DigimonAcademy { get; private set; }

        /// <summary>
        /// Belonging guild.
        /// </summary>
        public GuildModel? Guild { get; private set; }

        public List<CharacterTamerSkillModel> ActiveSkill { get; private set; }

        //Dynamics
        public DateTime LastRegenUpdate { get; private set; }
        public DateTime LastActiveEvolutionUpdate { get; private set; }
        public DateTime LastFatigueUpdate { get; private set; }
        public DateTime LastMovementUpdate { get; private set; }
        public DateTime LastSaveResources { get; private set; }
        public DateTime LastSyncResources { get; private set; }
        public DateTime LastDebuffUpdate { get; private set; }
        public DateTime LastDailyQuestCheck { get; private set; }
        public DateTime LastBuffsCheck { get; private set; }
        public DateTime LastExpiredItemsCheck { get; private set; }
        public DateTime LastExpiredBuffsCheck { get; private set; }
        public DateTime TimeRewardUpdate { get; private set; }
        public DateTime EventQueueInfoTime { get; private set; }
        public DateTime LastAfkNotification { get; private set; }
        public byte AfkNotifications { get; private set; }

        public bool TradeCondition = false;

        public int TargetTradeGeneralHandle = 0;

        public bool TradeConfirm = false; //Criar Model para Trade TradeModel (Trade Inventory e Propriedades)

        public ItemListModel TradeInventory { get; private set; }

        public ConditionEnum CurrentCondition { get; private set; }
        public ConditionEnum PreviousCondition { get; private set; }
        public bool InBattle { get; private set; }
        public bool Dead { get; private set; }
        public int ShopItemId { get; private set; }
        public string ShopName { get; private set; }
        public List<MobConfigModel> TargetMobs { get; private set; }
        public List<SummonMobModel> TargetSummonMobs { get; private set; }
        public List<EventMobConfigModel> TargetEventMobs { get; private set; }
        public List<DigimonModel> TargetPartners { get; private set; }
        public int TargetHandler { get; private set; }
        public Location ViewLocation { get; private set; }
        public Location BeforeEvent { get; private set; }
        public bool Hidden { get; private set; }
        public bool GodMode { get; private set; }
        public bool Processing { get; set; }
        public List<long> MobsInView { get; private set; }
        public ushort GenericHandler { get; private set; }
        public List<ItemModel> RepurchaseList { get; private set; }
        public bool TempShowFullMap { get; set; }
        public List<CharacterEncyclopediaModel> Encyclopedia { get; private set; }
        
        public DeckBuffModel? DeckBuff { get; private set; }

        public CharacterModel()
        {
            Size = 10000;
            Xai = new CharacterXaiModel();
            Incubator = new CharacterIncubatorModel();
            BuffList = new CharacterBuffListModel();
            SealList = new CharacterSealListModel();
            Foes = new List<CharacterFoeModel>();
            Foed = new List<CharacterFoeModel>();
            Digimons = new List<DigimonModel>();
            TimeReward = new TimeRewardModel();
            Friends = new List<CharacterFriendModel>();
            Friended = new List<CharacterFriendModel>();
            Points = new CharacterArenaPointsModel();
            MapRegions = new List<CharacterMapRegionModel>();
            ViewLocation = new Location();
            BeforeEvent = new Location();
            ActiveEvolution = new CharacterActiveEvolutionModel();
            DigimonArchive = new CharacterDigimonArchiveModel();
            Progress = new CharacterProgressModel();
            TargetPartners = new List<DigimonModel>();
            TargetMobs = new List<MobConfigModel>();
            TargetSummonMobs = new List<SummonMobModel>();
            TargetEventMobs = new List<EventMobConfigModel>();
            MobsInView = new List<long>();
            RepurchaseList = new List<ItemModel>();
            AttendanceReward = new AttendanceRewardModel();
            DailyPoints = new CharacterArenaDailyPointsModel(DateTime.Now, 0);
            ActiveSkill = new List<CharacterTamerSkillModel>()
            {
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue),
              new CharacterTamerSkillModel(TamerSkillTypeEnum.Normal, 0,0,DateTime.MaxValue)
            };
         

            ItemList = new List<ItemListModel>()
            {
                new ItemListModel(ItemListEnum.Equipment),
                new ItemListModel(ItemListEnum.Inventory),
                new ItemListModel(ItemListEnum.Warehouse),
                new ItemListModel(ItemListEnum.Chipsets),
                new ItemListModel(ItemListEnum.JogressChipset),
                new ItemListModel(ItemListEnum.Digivice),
                new ItemListModel(ItemListEnum.TamerSkill),
                new ItemListModel(ItemListEnum.RewardWarehouse),
                new ItemListModel(ItemListEnum.GiftWarehouse),
                new ItemListModel(ItemListEnum.ConsignedWarehouse),


                new ItemListModel(ItemListEnum.TamerShop),
                new ItemListModel(ItemListEnum.ConsignedShop)
            };

            Encyclopedia = new List<CharacterEncyclopediaModel>();
            StartTimers();
        }

        public void ResetAfkNotifications()
        {
            AfkNotifications = 0;
            LastAfkNotification = DateTime.Now;
        }

        public void RemoveDigimon(byte slot, bool updateSlots = true)
        {
            Digimons.RemoveAll(x => x.Slot == slot);

            if (updateSlots)
            {
                var count = Digimons.Count;

                Digimons.OrderByDescending(x => x.Slot).ToList().ForEach(digimon =>
                {
                    count--;
                });
            }
        }

        public void UpdateSlots()
        {
            var count = Digimons.Count;

            Digimons.OrderByDescending(x => x.Slot).ToList().ForEach(digimon =>
            {
                count--;
                digimon.SetSlot((byte)count);
            });
        }

        public void SetGenericHandler(ushort generalHandler)
        {
            GenericHandler = generalHandler;
        }

        public void AddDigimonSlots(byte amount = 1)
        {
            DigimonSlots += amount;
        }

        public void SetTrade(bool Condition = false, int targetTradeGeneralHandler = 0)
        {
            TargetTradeGeneralHandle = targetTradeGeneralHandler;
            TradeCondition = Condition;
            TradeInventory = new ItemListModel(ItemListEnum.TradeItems);
        }

        public void SetTamerTP(long targetTamerId = 0)
        {
            TargetTamerIdTP = (int)targetTamerId;
        }
        public void SetTradeConfirm(bool confirm)
        {
            TradeConfirm = confirm;
        }

        public void ClearTrade()
        {
            TradeCondition = false;
            TargetTradeGeneralHandle = 0;
            TradeConfirm = false;
            TradeInventory = null;

        }
    }
}