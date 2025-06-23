using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Asset;

namespace DigitalWorldOnline.Game.Managers
{
    public class StatusManager
    {
        private readonly AssetsLoader _assets;

        public StatusManager(AssetsLoader assets)
        {
            _assets = assets;
        }

        public CharacterBaseStatusAssetModel GetTamerBaseStatus(CharacterModelEnum characterModel)
        {
            return _assets.TamerBaseInfo.Single(x => x.Type == characterModel);
        }

        public CharacterLevelStatusAssetModel GetTamerLevelStatus(CharacterModelEnum characterModel, byte level)
        {
            return _assets.TamerLevelInfo.Single(x => x.Type == characterModel && x.Level == level);
        }

        public TitleStatusAssetModel? GetTitleStatus(int titleId)
        {
            return _assets.TitleStatus.FirstOrDefault(x => x.Id == titleId);
        }

        public DigimonBaseInfoAssetModel GetDigimonBaseInfo(int type)
        {
            return _assets.DigimonBaseInfo.Single(x => x.Type == type);
        }

        public StatusAssetModel GetDigimonBaseStatus(int type, byte level, short size)
        {
            var baseInfo = _assets.DigimonBaseInfo.Single(x => x.Type == type);
            var statusInfo = _assets.DigimonLevelInfo.Single(x => x.StatusId == (level + baseInfo.ScaleType * 1000));
            var statusApply = _assets.StatusApply.FirstOrDefault(x => x.StageValue == baseInfo.EvolutionType);

            if (statusApply == null)
                return default;

            var sizeMultiplier = (decimal)size / 10000;

            var finalAs = baseInfo.ASValue + statusInfo.ASValue;
            var finalAr = baseInfo.ARValue + statusInfo.ARValue;
            var finalBl = baseInfo.BLValue + statusInfo.BLValue;

            var sizeHp = sizeMultiplier * baseInfo.HPValue + statusInfo.HPValue;
            var applyHp = (int)(statusInfo.HPValue * (statusApply.ApplyValue - 100) * 0.01f);
            var finalHp = (int)Math.Floor(sizeHp + applyHp);

            var sizeDs = baseInfo.DSValue + statusInfo.DSValue;
            var applyDs = (int)(statusInfo.DSValue * (statusApply.ApplyValue - 100) * 0.01f);
            var finalDs = sizeDs + applyDs;

            var sizeAt = sizeMultiplier * baseInfo.ATValue + statusInfo.ATValue;
            var applyAt = (int)(statusInfo.ATValue * (statusApply.ApplyValue - 100) * 0.01f);
            var finalAt = (int)Math.Floor(sizeAt + applyAt);

            var sizeCr = sizeMultiplier * baseInfo.CTValue + statusInfo.CTValue;
            var finalCr = (int)Math.Floor(sizeCr);

            var sizeCd = 100;
            var finalCd = sizeCd;

            var sizeDe = sizeMultiplier * baseInfo.DEValue + statusInfo.DEValue;
            var applyDe = (int)(statusInfo.DEValue * (statusApply.ApplyValue - 100) * 0.01f);
            var finalDe = (int)Math.Floor(sizeDe + applyDe);

            var finalEv = baseInfo.EVValue + statusInfo.EVValue;
            var finalHt = baseInfo.HTValue;
            var finalMs = baseInfo.MSValue + statusInfo.MSValue;
            var finalWs = baseInfo.WSValue + statusInfo.WSValue;

            var baseStatus = new StatusAssetModel(
                finalAs,
                finalAr,
                finalAt,
                finalBl,
                finalCr,
                finalCd,
                finalDe,
                finalDs,
                finalEv,
                finalHp,
                finalHt,
                finalMs,
                finalWs
            );

            return baseStatus;
        }
    }
}
