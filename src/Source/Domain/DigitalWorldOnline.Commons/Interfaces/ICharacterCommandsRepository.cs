using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Model.Character;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Events;
using DigitalWorldOnline.Commons.Models.Mechanics;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface ICharacterCommandsRepository
    {
        Task<long> AddCharacterAsync(CharacterModel character);

        Task<DigimonDTO> AddDigimonAsync(DigimonModel digimon);

        Task<CharacterFriendDTO> AddFriendAsync(CharacterFriendModel friend);

        //Task<AccountBlockDTO> AddBanAsync(AccountBlockModel ban);

        Task<DeleteCharacterResultEnum> DeleteCharacterByAccountAndPositionAsync(long accountId, byte characterPosition);

        Task UpdateDigicloneAsync(DigimonDigicloneModel digiclone);

        Task UpdateCharacterBasicInfoAsync(CharacterModel character);

        Task UpdateCharacterActiveEvolutionAsync(CharacterActiveEvolutionModel activeEvolution);

        Task UpdateCharacterBuffListAsync(CharacterBuffListModel buffList);

        Task UpdateDigimonBuffListAsync(DigimonBuffListModel buffList);

        Task UpdateCharacterProgressCompleteAsync(CharacterProgressModel progress);

        Task UpdateCharacterSealsAsync(CharacterSealListModel sealList);

        Task UpdateCharacterTitleByIdAsync(long characterId, short titleId);

        Task UpdatePartnerCurrentTypeAsync(DigimonModel digimon);

        Task AddChatMessageAsync(ChatMessageModel chatMessage);

        Task UpdateDigimonExperienceAsync(DigimonModel digimon);

        Task UpdateCharacterExperienceAsync(long tamerId, long currentExperience, byte level);

        Task UpdateCharacterResourcesAsync(CharacterModel tamer);

        Task UpdateCharacterStateByIdAsync(long characterId, CharacterStateEnum state);

        Task UpdateCharacterLocationAsync(CharacterLocationModel location);

        Task UpdateDigimonLocationAsync(DigimonLocationModel location);

        Task UpdateCharactersStateAsync(CharacterStateEnum state);

        Task UpdateCharacterChannelByIdAsync(long characterId, byte channel);

        Task UpdateItemListBitsAsync(long itemListId, long bits);

        Task UpdateItemsAsync(List<ItemModel> items);

        Task UpdateItemAccessoryStatusAsync(ItemModel item);

        Task UpdateItemAsync(ItemModel item);

        Task UpdateItemListSizeAsync(long itemListId, byte newSize);

        Task AddInventorySlotsAsync(List<ItemModel> items);

        Task UpdateCharacterEventStateByIdAsync(long characterId, CharacterEventStateEnum state);

        Task UpdateEvolutionAsync(DigimonEvolutionModel evolution);

        Task UpdateIncubatorAsync(CharacterIncubatorModel incubator);

        Task UpdateCharacterMapRegionAsync(CharacterMapRegionModel mapRegion);

        Task UpdateDigimonSizeAsync(long digimonId, short size);

        Task UpdateCharacterSizeAsync(long characterId, short size);

        Task UpdateCharacterInitialPacketSentOnceSentAsync(long characterId, bool sendOnceSent);

        Task UpdateDigimonGradeAsync(long digimonId, DigimonHatchGradeEnum grade);

        Task UpdateCharacterDigimonsOrderAsync(CharacterModel character);

        Task DeleteDigimonAsync(long digimonId);

        Task UpdateCharacterDigimonArchiveItemAsync(CharacterDigimonArchiveItemModel characterDigimonArchiveItem);

        Task UpdateDigimonSlotAsync(long digimonId, byte digimonSlot);

        Task UpdateCharacterXaiAsync(CharacterXaiModel xai);

        Task AddDigimonArchiveSlotAsync(Guid archiveId, CharacterDigimonArchiveItemModel archiveItem);

        Task UpdateCharacterDigimonSlotsAsync(long characterId, byte slots);

        Task AddInventorySlotAsync(ItemModel newSlot);

        Task<CharacterDTO> ChangeTamerModelAsync(long characterId, CharacterModelEnum model);

        Task<CharacterDTO> ChangeCharacterNameAsync(long characterId, string newCharacterName);

        Task<CharacterDTO> ChangeCharacterIdTpAsync(long characterId, int newTargetTamerIdTP);

        Task<DigimonDTO> ChangeDigimonNameAsync(long characterId, string newDigimonName);

        Task UpdateTamerSkillCooldownAsync(CharacterTamerSkillModel activeSkill);

        Task UpdateItemSocketStatusAsync(ItemModel item);

        Task UpdateCharacterArenaPointsAsync(CharacterArenaPointsModel points);

        Task UpdateCharacterInProgressAsync(InProgressQuestModel progress);

        Task AddCharacterProgressAsync(CharacterProgressModel progress);

        Task UpdateTamerAttendanceRewardAsync(AttendanceRewardModel attendanceRewardModel);

        Task UpdateTamerTimeRewardAsync(TimeRewardModel timeRewardModel);

        Task UpdateCharacterArenaDailyPointsAsync(CharacterArenaDailyPointsModel points);

        Task UpdateCharacterDeckBuffAsync(CharacterModel character);

        Task<CharacterEncyclopediaModel> CreateCharacterEncyclopediaAsync(CharacterEncyclopediaModel characterEncyclopedia);

        Task UpdateCharacterEncyclopediaAsync(CharacterEncyclopediaModel characterEncyclopedia);

        Task UpdateCharacterEncyclopediaEvolutionsAsync(CharacterEncyclopediaEvolutionsModel characterEncyclopedia);

        Task UpdateCharacterFriendsAsync(CharacterModel? character, bool connected);
    }
}
