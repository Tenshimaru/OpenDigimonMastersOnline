namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum GeneralSizeEnum
    {
        /// <summary>
        /// IDK
        /// </summary>
        SizeID = 30,

        /// <summary>
        /// IDK
        /// </summary>
        SizePass = 120,

        /// <summary>
        /// IDK
        /// </summary>
        SizeAccessToken = 255,

        /// <summary>
        /// Max size for character name.
        /// </summary>
        CharName = 32,

        /// <summary>
        /// Max size for guild name.
        /// </summary>
        GuildName = 48,
        GuildNotice = 255,
        GuildClassName = 32,
        GuildClassNameDesc = 72,
        GuildMemo = 64,

        /// <summary>
        /// Max size for tamer shop.
        /// </summary>
        ShopName = 128, //TODO: consigned or normal?

        /// <summary>
        /// Max size for annotation on friend / block.
        /// </summary>
        FriendOrBlockAnnotation = 255,
        SizeFriendList = 100,
        SizeBlockedList = 100,
        SizeFriends = 20,

        /// <summary>
        /// Max size for tamer inventory.
        /// </summary>
        InventoryMax = 150,

        /// <summary>
        /// Initial tamer inventory size.
        /// </summary>
        InitialInventory = 30,

        /// <summary>
        /// Tamer equipment size.
        /// </summary>
        Equipment = 13, //654=14

        /// <summary>
        /// Max size for secondary password.
        /// Client limits too.
        /// </summary>
        SecondaryPassword = 33,

        /// <summary>
        /// Initial tamer warehouse size.
        /// </summary>
        InitialWarehouse = 21,

        /// <summary>
        /// Max size for tamer warehouse.
        /// </summary>
        WarehouseMax = 245,

        /// <summary>
        /// Initial account warehouse size.
        /// </summary>
        InitialAccountWarehouse = 14,

        /// <summary>
        /// Digivice chipsets size.
        /// </summary>
        Chipsets = 12,

        /// <summary>
        /// Max size for tamer's seals.
        /// </summary>
        Seals = 120,

        SizeTemVault = 7, // ?

        /// <summary>
        /// Max size of trade inventory.
        /// </summary>
        TradeInventory = 5,

        /// <summary>
        /// Shop warehouse size.
        /// </summary>
        ShopWarehouse = 18, //TODO: onde é usado?

        /// <summary>
        /// Gift warehouse size.
        /// </summary>
        GiftWarehouse = 20,

        /// <summary>
        /// Cash warehouse size.
        /// </summary>
        CashWarehouse = 148,

        /// <summary>
        /// Reward warehouse size.
        /// </summary>
        RewardWarehouse = 50, //TODO: check size

        /// <summary>
        /// Digimon archive max size.
        /// </summary>
        ArchiveMax = 200,   // 654= 200

        /// <summary>
        /// Initial digimon archive size.
        /// </summary>
        InitialArchive = 7,

        /// <summary>
        /// Initial digimon archive size.
        /// </summary>
        InitialAcademy = 3,

        /// <summary>
        /// Cash shop buy history size.
        /// </summary>
        CashShopBuyHistory = 255, //1000
        
        /// <summary>
        /// Tamer's digivice.
        /// </summary>
        Digivice = 1,

        /// <summary>
        /// Tamer's digivice's jogress chipset.
        /// </summary>
        JogressChipset = 1,

        /// <summary>
        /// Total size of personal shop items
        /// </summary>
        PersonalShop = 3,
        
        /// <summary>
        /// Total size of consigned shop items
        /// </summary>
        ConsignedShop = 5,

        /// <summary>
        /// Starter amount of active digimon slots.
        /// </summary>
        MinActiveDigimonList = 3,

        /// <summary>
        /// Max value for active digimon slots.
        /// </summary>
        MaxActiveDigimonList = 5,

        /// <summary>
        /// ConsignedShopWarehouse maximum size.
        /// </summary>
        ConsignedShopWarehouse = 18,

        /// <summary>
        /// ConsignedShopWarehouse maximum size.
        /// </summary>
        TradeItems = 5,

        TamerSkill = 5,
        SizeDigimonPartner = 5,
        SizeGuildAuthorities = 5,
        SizeGuildDefaultMembers = 50,
        SizeArenaRankingCompetitors = 100,
        
        SizeDropList = 100,
        SizeMapRegion = 192,
        SizeActiveBuffs = 20,
        MIN_LenID = 6,
        MIN_LenPass = 2,
        MIN_LenCharName = 3,
        MIN_LenGuildName = 3,
        MIN_LenGuildClassName = 3,
        MIN_LenGuildClassNameDesc = 3,
        MIN_LenGuildNotice = 0,
        MIN_LenGuildMemo = 0,
        MIN_LenShopName = 1,
        MIN_LenBuddyMemo = 2,
        MIN_LenMsg = 1,
        MAX_LenID = 24,
        MAX_LenPass = 16,
        MAX_LenCharName = 12,
        MAX_LenGuildName = 14,
        MAX_LenGuildClassName = 12,
        MAX_LenGuildClassNameDesc = 21,
        MAX_LenGuildNotice = 120,
        MAX_LenGuildMemo = 31,
        MAX_LenShopName = 17,
        MAX_LenBuddyMemo = 23,
        MAX_LenMsg = 250,

        EvoStep = 9, // Até 8 etapas (0 a 7), 8ª degeneração degeneração
        MaxAttrInchantLevel = 18,
        MaxInchantLevel = 90,
        MAX_CHANNELING = 5,
        MAX_ItemSkillDigimon = 2,
        DropItem = 35,
        MAX_DIGIMONSKILLLevel = 30,

        ItemSizeInBytes = 68,

        TamerLevelMax = 120,
        DigimonLevelMax = 120,

        StarterDigimonSize = 10000,
        StartDigimonFriendship = 15,

        StartMapLocation = 105,
        StartX = 14834,
        StartY = 9895,

        /// <summary>
        /// Inventory minimal slot value.
        /// </summary>
        InventoryMinSlot = 0,

        /// <summary>
        /// Inventory maximum slot value.
        /// </summary>
        InventoryMaxSlot = 150,

        /// <summary>
        /// Equipment minimal slot value.
        /// </summary>
        EquipmentMinSlot = 1000,

        /// <summary>
        /// Equipment maximum slot value.
        /// </summary>
        EquipmentMaxSlot = 1013,

        /// <summary>
        /// Warehouse minimal slot value.
        /// </summary>
        WarehouseMinSlot = 2000,

        /// <summary>
        /// Warehouse maximum slot value.
        /// </summary>
        WarehouseMaxSlot = 2315,

        /// <summary>
        /// Account warehouse minimal slot value.
        /// </summary>
        AccountWarehouseMinSlot = 9000,

        /// <summary>
        /// Account warehouse maximum slot value.
        /// </summary>
        AccountWarehouseMaxSlot = 9014,

        /// <summary>
        /// Jogress chipset slot value.
        /// </summary>
        JogressChipSetSlot = 3000,

        /// <summary>
        /// Normal chipset minimal slot value.
        /// </summary>
        ChipsetMinSlot = 4000,

        /// <summary>
        /// Normal chipset maximum slot value.
        /// </summary>
        ChipsetMaxSlot = 4007,

        /// <summary>
        /// Digivice slot value.
        /// </summary>
        DigiviceSlot = 5000,
            
        /// <summary>
        /// XAI slot value.
        /// </summary>
        XaiSlot = 1011
    }
}