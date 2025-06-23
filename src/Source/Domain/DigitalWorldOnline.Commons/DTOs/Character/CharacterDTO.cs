using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterDTO
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public byte Position { get; set; }
        public CharacterModelEnum Model { get; set; }
        public byte Level { get; set; }
        public string Name { get; set; }
        public short Size { get; set; }
        public CharacterStateEnum State { get; set; }
        public CharacterEventStateEnum EventState { get; set; }
        public long ServerId { get; set; }
        public long CurrentExperience { get; set; }
        public byte Channel { get; set; }
        public int TargetTamerIdTP { get; set; }
        public byte DigimonSlots { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentDs { get; set; }
        public int XGauge { get; set; }
        public short XCrystals { get; set; }
        public short CurrentTitle { get; set; }
        public int? DeckBuffId { get; set; }
        public bool InitialPacketSentOnceSent { get; set; }
        public DateTime CreateDate { get; set; }

        //Refs
        public List<ItemListDTO> ItemList { get; set; }
        public List<DigimonDTO> Digimons { get; set; }
        public List<CharacterFriendDTO> Friends { get; set; }
        
        public List<CharacterFriendDTO> Friended { get; set; }
        public List<CharacterFoeDTO> Foes { get; set; }
        public List<CharacterFoeDTO> Foed { get; set; }
        public CharacterArenaPointsDTO Points { get; set; }
        public List<CharacterMapRegionDTO> MapRegions { get; set; }
        public CharacterBuffListDTO BuffList { get; set; }
        public CharacterIncubatorDTO Incubator { get; set; }
        public CharacterLocationDTO Location { get; set; }
        public CharacterSealListDTO SealList { get; set; }
        public CharacterXaiDTO Xai { get; set; } //TODO: verificar se precisa mesmo
        public TimeRewardDTO TimeReward { get; set; }
        public AttendanceRewardDTO AttendanceReward { get; set; }
        public ConsignedShopDTO ConsignedShop { get; set; }
        public CharacterProgressDTO Progress { get; set; }
        public CharacterActiveEvolutionDTO ActiveEvolution { get; set; }
        public CharacterDigimonArchiveDTO DigimonArchive { get; set; }
        public CharacterArenaDailyPointsDTO DailyPoints { get; set; }
        public List<CharacterTamerSkillDTO> ActiveSkill { get; set; }
        public GuildDTO? Guild { get; set; }
        public List<CharacterEncyclopediaDTO> Encyclopedia { get; set; }
        
        public DeckBuffAssetDTO? DeckBuff { get; set; }
    }
}