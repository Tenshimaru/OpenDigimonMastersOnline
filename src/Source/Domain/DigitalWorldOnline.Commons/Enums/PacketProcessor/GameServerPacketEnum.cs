using DigitalWorldOnline.Commons.Interfaces;

namespace DigitalWorldOnline.Commons.Enums.PacketProcessor
{
    public enum GameServerPacketEnum
    {
        /// <summary>
        /// Open the tamer shop.
        /// </summary>
        CashShopReloadBalance = 3404,
        
        /// <summary>
        /// Open the tamer shop.
        /// </summary>
        CashShopOpen = 3412,

        /// <summary>
        /// Open the tamer shop.
        /// </summary>
        CashShopBuy = 3413,

        /// <summary>
        /// Unknown packet
        /// </summary>
        Unknown = -99,

        /// <summary>
        /// To avoid connection break/interrupt, the client sends this often.
        /// </summary>
        KeepConnection = -3,

        /// <summary>
        /// Request connection with the server.
        /// </summary>
        Connection = -1,

        /// <summary>
        /// Equips a new title on the tamer.
        /// </summary>
        SetTitle = 15,

        /// <summary>
        /// Loads all the other game-related information.
        /// </summary>
        ComplementarInformation = 1001,

        /// <summary>
        /// Sent at every tamer or digimon movimentation.
        /// </summary>
        TamerMovimentation = 1004,

        /// <summary>
        /// Sends a message to the target chat.
        /// </summary>
        ChatMessage = 1008,

        /// <summary>
        /// Sends a private message to the target character.
        /// </summary>
        WhisperMessage = 1009,

        /// <summary>
        /// Sends the current partner to perform an attack into the target.
        /// </summary>
        PartnerAttack = 1013,

        /// <summary>
        /// Sends the current partner to perform an skill into the target.
        /// </summary>
        PartnerSkill = 1015,

        /// <summary>
        /// Updates the current tamer target information.
        /// </summary>
        UpdateTarget = 1016,

        /// <summary>
        /// Request confirm dialog after partner defeat.
        /// </summary>
        DieConfirm = 1022,

        /// <summary>
        /// Evolves the current partner
        /// </summary>
        PartnerEvolution = 1028,

        /// <summary>
        /// Calls the current partner back to the tamer.
        /// </summary>
        PartnerStop = 1033,

        /// <summary>
        /// Insert egg on incubator.
        /// </summary>
        HatchInsertEgg = 1036,

        /// <summary>
        /// Try to increase the curent incubator egg.
        /// </summary>
        HatchIncrease = 1037,

        /// <summary>
        /// Ecloacs the current egg.
        /// </summary>
        HatchFinish = 1038,

        /// <summary>
        /// Removes the egg from the incubator.
        /// </summary>
        HatchRemoveEgg = 1039,

        /// <summary>
        /// Switches the current partner.
        /// </summary>
        PartnerSwitch = 1041,

        /// <summary>
        /// Deletes the selected partner.
        /// </summary>
        PartnerDelete = 1042,

        /// <summary>
        /// Unlocks the target map region.
        /// </summary>
        ChangeChannel = 1050,

        /// <summary>
        /// Unlocks the target map region.
        /// </summary>
        UnlockRegion = 1051,

        /// <summary>
        /// Evolution unlock.
        /// </summary>
        EvolutionUnlock = 1055,

        /// <summary>
        /// Sends a shout message to the current map.
        /// </summary>
        ShoutMessage = 1056,

        /// <summary>
        /// Jumps to another map.
        /// </summary>
        JumpBooster = 1057,

        /// <summary>
        /// Tamer fun actions (dance, yellow, etc).
        /// </summary>
        TamerReaction = 1058,

        /// <summary>
        /// Unlocks the ride mode of the target evolution.
        /// </summary>
        EvolutionRideUnlock = 1063,

        /// <summary>
        /// Sends a global megaphone message.
        /// </summary>
        MegaphoneMessage = 1074,

        /// <summary>
        /// Sends the digiclone results
        /// </summary>
        PartnerDigiclone = 1075,

        /// <summary>
        /// Sends the digiclone reset
        /// </summary>
        PartnerDigicloneReset = 1083,

        /// <summary>
        /// Sends the Skill Level Up
        /// </summary>
        SkillLevelUp = 1104,

        /// <summary>
        /// Sends the Summon Player
        /// </summary>
        TamerSummon = 1114,

        /// <summary>
        /// Tamer Change Character Name
        /// </summary>
        TamerChangeName = 1311,

        /// <summary>
        /// Change Digimon Name
        /// </summary>
        DigimonChangeName = 3206,

        /// <summary>
        /// Tamer Change Character Model
        /// </summary>
        TamerChangeModel = 1314,

        /// <summary>
        /// Turns partner into ride mode.
        /// </summary>
        PartnerRideModeStart = 1325,

        /// <summary>
        /// Ends partner ride mode.
        /// </summary>
        PartnerRideModeStop = 1326,

        /// <summary>
        /// Send  TamerSkillRequest.
        /// </summary>
        TamerSkillRequest = 1327,

        ActiveTamerCashSkillRemove = 1332,

        /// <summary>
        /// Sets the target seal as favorite
        /// </summary>
        SetSealFavorite = 1334,

        /// <summary>
        /// Send Request Trade
        /// </summary>
        TradeRequestSend = 1501,

        /// <summary>
        /// Trade Request Accept
        /// </summary>
        TradeRequestAccept = 1502,

        /// <summary>
        /// Trade Confirmation 
        /// </summary>
        TradeConfirmation = 1503,

        /// <summary>
        /// Trade Inventory Unlock 
        /// </summary>
        TradeInventoryUnlock = 1505,

        /// <summary>
        /// Trade Refuse 
        /// </summary>
        TradeRefuse = 1506,

        /// <summary>
        /// Trade Add Item
        /// </summary>
        TradeAddItem = 1508,

        /// <summary>
        /// Trade Add Money
        /// </summary>
        TradeAddMoney = 1509,

        /// <summary>
        /// Shows the incoming personal shop window.
        /// </summary>
        PersonalShopPrepare = 1510,

        /// <summary>
        /// Open the tamer shop.
        /// </summary>
        TamerShopOpen = 1511,

        /// <summary>
        /// Tamer Close Shop
        /// </summary>
        TamerShopClose = 1512,

        /// <summary>
        /// Send buyer
        /// </summary>
        TamerShopBuy = 1513,

        /// <summary>
        /// Send to owner sell Item.
        /// </summary>
        TamerShopSell = 1514,

        /// <summary>
        /// Show List item Personal Shop
        /// </summary>
        TamerShopList = 1515,

        /// <summary>
        /// Open the consigned shop.
        /// </summary>
        ConsignedShopOpen = 1516,

        /// <summary>
        /// Retrieves the items from consigned shop and disables the shop.
        /// </summary>
        ConsignedShopRetrieve = 1517,

        /// <summary>
        /// Purchase's an item from the consigned shop item list.
        /// </summary>
        ConsignedShopPurchaseItem = 1518,

        /// <summary>
        /// Shows the target consigned shop items.
        /// </summary>
        ConsignedSell = 1519,

        /// <summary>
        /// Shows the target consigned shop items.
        /// </summary>
        ConsignedShopView = 1520,

        /// <summary>
        /// Trade Add Money
        /// </summary>
        TradeRemoveItem = 1531,

        /// <summary>
        /// Retrieves item and bits from the consigned shop warehouse.
        /// </summary>
        ConsignedWarehouseRetrieve = 1521,

        /// <summary>
        /// Open the running consigned shop item list.
        /// </summary>
        ConsignedWarehouse = 1523,

        /// <summary>
        /// Trade Inventory lock 
        /// </summary>
        TradeInventorylock = 1532,

        /// <summary>
        /// Confirm the channel switch.
        /// </summary>
        ChannelSwitchConfirm = 1703,

        /// <summary>
        /// Loads the base information about the tamer and digimons.
        /// </summary>
        InitialInformation = 1706,

        /// <summary>
        /// Teleports the tamer to another location.
        /// </summary>
        WarpGate = 1709,

        /// <summary>
        /// Loads the available channels
        /// </summary>
        Channels = 1713,

        /// <summary>
        /// Sends a guild message to the entire guild.
        /// </summary>
        GuildMessage = 2114,

        /// <summary>
        /// Sends an invite to party.
        /// </summary>
        PartyRequestSend = 2301,

        /// <summary>
        /// Sends a response to the party invite.
        /// </summary>
        PartyRequestResponse = 2302,

        /// <summary>
        /// Sends a message to the entire party.
        /// </summary>
        PartyMessage = 2304,

        /// <summary>
        /// Kicks a member of the party.
        /// </summary>
        PartyMemberKick = 2306,

        /// <summary>
        /// Member quit from the party.
        /// </summary>
        PartyMemberLeave = 2307,

        /// <summary>
        /// Changes the party leader.
        /// </summary>
        PartyLeaderChange = 2308,

        /// <summary>
        /// Changes the party config.
        /// </summary>
        PartyConfigChange = 2309,

        /// <summary>
        /// Party Disconnect
        /// </summary>
        PartyMemberDisconnect = 2312,

        /// <summary>
        /// Party Disconnect
        /// </summary>
        PartyMemberDismiss = 2317,

        /// <summary>
        /// Loads the friend information.
        /// </summary>
        AddFriend = 2401,

        /// <summary>
        /// Loads the friend information.
        /// </summary>
        RemoveFriend = 2402,

        FriendList = 2404,

        /// <summary>
        /// Loads the friend information.
        /// </summary>
        MemoFriend = 2405,

        /// <summary>
        /// Loads the friend information.
        /// </summary>
        FriendInformation = 3129,

        /// <summary>
        /// Loads the Burning Event information.
        /// </summary>
        BurningEvent = 3132,

        /// <summary>
        /// Sets the target seal as leader
        /// </summary>
        SetSealLeader = 3232,

        /// <summary>
        /// Removes the current seal leader
        /// </summary>
        RemoveSealLeader = 3233,

        /// <summary>
        /// Adds a digimon to archive/storage.
        /// </summary>
        DigimonArchiveInsert = 3201,

        /// <summary>
        /// Adds a digimon to archive/storage.
        /// </summary>
        DigimonArchiveSwap = 3243,

        /// <summary>
        /// Loads the digimon archive/storage.
        /// </summary>
        ArchiveAcademyList = 3226,

        /// <summary>
        /// Loads the digimon archive/storage.
        /// </summary>
        ArchiveAcademyInsert = 3227,

        ArchiveAcademyError = 3228,

        ArchiveAcademyExtraction = 3229,

        ArchiveAcademySaveExp = 3230,

        ArchiveAcademySlotChange = 3231,

        /// <summary>
        /// Loads the digimon archive/storage.
        /// </summary>
        DigimonArchive = 3204,

        /// <summary>
        /// Loads the remaining membership timer.
        /// </summary>
        MembershipInformation = 3414,

        /// <summary>
        ///  HatchSpiritEvolution.
        /// </summary>
        HatchSpiritEvolution = 3239,

        /// <summary>
        ///  Spirit Craft.
        /// </summary>
        SpiritCraft = 3240,

        /// <summary>
        /// Moves an item to another position/storage.
        /// </summary>
        MoveItem = 3904,

        /// <summary>
        /// Split inventory items
        /// </summary>
        SplitItem = 3907,

        /// <summary>
        /// Removes and item from the inventory
        /// </summary>
        ItemRemove = 3909,

        /// <summary>
        /// Sends the tamer to loot the item on the ground.
        /// </summary>
        LootItem = 3910,

        /// <summary>
        /// Purchases an item from the NPC store.
        /// </summary>
        NpcItemPurchase = 3915,

        /// <summary>
        /// Sells an item to the NPC store.
        /// </summary>
        NpcItemSell = 3916,

        /// <summary>
        /// Item Socket In Attribute
        /// </summary>
        ItemSocketIn = 3926,

        /// <summary>
        /// Item Socket Out Attribute
        /// </summary>
        ItemSocketOut = 3927,

        /// <summary>
        /// Sells an item to the NPC store.
        /// </summary>
        ItemSocketIdentify = 3929,

        /// <summary>
        /// Event RareMachine
        /// </summary>
        RareMachineOpen = 3956,

        /// <summary>
        /// Event RareMachine
        /// </summary>
        RareMachineRun = 3957,

        /// <summary>
        /// Opens the target seal
        /// </summary>
        OpenSeal = 3971,

        /// <summary>
        /// Closes the target seal
        /// </summary>
        CloseSeal = 3972,

        /// <summary>
        /// Repurchase an item from the NPC
        /// </summary>
        RepurchaseItem = 3978,

        /// <summary>
        /// Crafts an item.
        /// </summary>
        ItemCraft = 3982,

        /// <summary>
        /// Sorts the inventory items.
        /// </summary>
        InventorySort = 3986,

        /// <summary>
        /// Dungeon Warp packet.
        /// </summary>
        WarpGateDungeon = 4119,

        /// <summary>
        /// Arena current Stage Next.
        /// </summary>
        DungeonArenaStageNext = 4126,

        /// <summary>
        /// Arena surrender request.
        /// </summary>
        DungeonArenaSurrender = 4127,

        /// <summary>
        /// Arena daily ranking load.
        /// </summary>
        ArenaDailyRankingLoad = 4130,

        /// <summary>
        /// Arena daily insert points.
        /// </summary>
        ArenaDailyInsertPoints = 4131,

        /// <summary>
        /// Update the quest status.
        /// </summary>
        QuestUpdate = 11001,

        /// <summary>
        /// Accepts a quest.
        /// </summary>
        QuestAccept = 11002,

        /// <summary>
        /// Give up a quest.
        /// </summary>
        QuestGiveUp = 11003,

        /// <summary>
        /// Deliver a quest.
        /// </summary>
        QuestDeliver = 11004,

        /// <summary>
        /// Receives a new achievement.
        /// </summary>
        ProgressUpdate = 11007,


        /// <summary>
        /// Consumes an item.
        /// </summary>
        ConsumeItem = 3901,

        /// <summary>
        /// sends the loading Tamer Account Warehouse.
        /// </summary>
        LoadAccountWarehouse = 3930,

        /// <summary>
        /// sends the loading Tamer Account Warehouse.
        /// </summary>
        RetrivieAccountWarehouseItem = 3931,
        /// <summary>
        /// sends the loading Tamer Gift Storage.
        /// </summary>
        LoadGiftStorage = 3935,

        /// <summary>
        /// sends the Gift Storage Item Retrieve.
        /// </summary>
        GiftStorageItemRetrieve = 3936,
        /// <summary>
        /// Join event queue. (Custom)
        /// </summary>
        JoinEventQueue = 3124,

        /// <summary>
        /// Loads NPC repurchase list.
        /// </summary>
        LoadNpcRepurchaseList = 3979,

        /// <summary>
        /// Removes an active buff manually.
        /// </summary>
        RemoveBuff = 4005,

        /// <summary>
        /// Updates the current guild message.
        /// </summary>
        GuildNoticeUpdate = 2126,

        /// <summary>
        /// Updates the current guild historic.
        /// </summary>
        GuildHistoric = 2128,

        /// <summary>
        /// Updates an authority title description.
        /// </summary>
        GuildTitleChange = 2129,

        /// <summary>
        /// Creates a new guild.
        /// </summary>
        CreateGuild = 2101,

        /// <summary>
        /// Deletes a guild.
        /// </summary>
        GuildDelete = 2102,

        /// <summary>
        /// Sends an invite to join the guild.
        /// </summary>
        GuildInvite = 2109,

        /// <summary>
        /// Refuses a guild invite.
        /// </summary>
        GuildInviteDeny = 2105,

        /// <summary>
        /// Accepts a guild invite.
        /// </summary>
        GuildInviteAccept = 2103,

        /// <summary>
        /// Guild member authority change.
        /// </summary>
        GuildAuthorityChangeToMaster = 2119,

        /// <summary>
        /// Guild member authority change.
        /// </summary>
        GuildAuthorityChangeToSubMaster = 2118,

        /// <summary>
        /// Guild member authority change.
        /// </summary>
        GuildAuthorityChangeToDatsMember = 2117,

        /// <summary>
        /// Guild member authority change.
        /// </summary>
        GuildAuthorityChangeToMember = 2116,

        /// <summary>
        /// Guild member authority change.
        /// </summary>
        GuildAuthorityChangeToNewMember = 2115,

        /// <summary>
        /// Guild member quit.
        /// </summary>
        GuildMemberLeave = 2107,

        /// <summary>
        /// Guild member kick.
        /// </summary>
        GuildMemberKick = 2106,

        /// <summary>
        /// Load encyclopedia data.
        /// </summary>
        EncyclopediaLoad = 3234,

        /// <summary>
        /// Receive encyclopedia reward.
        /// </summary>
        EncyclopediaGetReward = 3235,

        /// <summary>
        /// Use encyclopedia Deck buff.
        /// </summary>
        EncyclopediaDeckBuffUse = 3236,

        /// <summary>
        /// Get encyclopedia Deck buff effetct.
        /// </summary>
        EncyclopediaDeckBuffEffect = 3237,

        /// <summary>
        /// Return an item.
        /// </summary>
        ItemReturn = 3923,

        /// <summary>
        /// Insert backup disk on incubator.
        /// </summary>
        HatchInsertBackup = 3946,

        /// <summary>
        /// Removes the backup disk from incubator.
        /// </summary>
        HatchRemoveBackup = 3947,

        /// <summary>
        /// Identify an item.
        /// </summary>
        ItemIdentify = 3968,

        /// <summary>
        /// Reroll item status.
        /// </summary>
        ItemReroll = 3969,

        /// <summary>
        /// Guild member kick.
        /// </summary>
        ItemScan = 3987,

        /// <summary>
        /// REWARD STORAGE.
        /// </summary>
        RewardStorage = 16001,

        /// <summary>
        /// Arena all ranking request info.
        /// </summary>
        ArenaRankingAllRequestInfo = 16023,

        /// <summary>
        /// Charge X-Crystal.
        /// </summary>
        TamerChargeXCrystal = 16030,

        /// <summary>
        /// Consume X-Crystal.
        /// </summary>
        TamerConsumeXCrystal = 16031,

        /// <summary>
        /// Trancendence Partner  Exp Result.
        /// </summary>
        TranscendenceReceiveExpResult = 16039,

        /// <summary>
        /// Trancendence Partner  Exp Result.
        /// </summary>
        TranscendenceSuccess = 16040,

        /// <summary>
        /// Item Recharge NPC Result.
        /// </summary>
        TimeChargeResult = 16042,
        
    }

}