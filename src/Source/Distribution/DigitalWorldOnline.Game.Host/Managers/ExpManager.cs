using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Utils;
using Serilog;

namespace DigitalWorldOnline.Game.Managers
{
    public class ReceiveExpResult
    {
        public byte LevelGain { get; private set; }
        public bool Success { get; private set; }

        public ReceiveExpResult(byte levelGain, bool success)
        {
            LevelGain = levelGain;
            Success = success;
        }
    }

    public class ExpManager
    {
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;

        public ExpManager(AssetsLoader assets, ILogger logger)
        {
            _assets = assets;
            _logger = logger;
        }

        // --------------------------------------------------------------------------------------------

        public ReceiveExpResult ReceiveMaxTamerExperience(CharacterModel tamer)
        {
            if (tamer.Level >= (int)GeneralSizeEnum.TamerLevelMax) return new ReceiveExpResult(0, false);

            var tamerInfos = _assets.TamerLevelInfo.Where(x => x.Type == tamer.Model).ToList();

            if (tamerInfos == null || !tamerInfos.Any())// || tamerInfos.Count != 120)
            {
                _logger.Error($"Incomplete level config for tamer {tamer.Model}.");
                return new ReceiveExpResult(0, false);
            }

            var levelGain = (byte)((int)GeneralSizeEnum.TamerLevelMax - tamer.Level);
            tamer.LevelUp(levelGain);
            tamer.SetExp(0);

            _logger.Verbose($"Character {tamer.Id} received max exp.");
            return new ReceiveExpResult(levelGain, true);
        }

        public ReceiveExpResult ReceiveTamerExperience(long receivedExp, CharacterModel tamer)
        {
            if (tamer.Level >= (int)GeneralSizeEnum.TamerLevelMax) return new ReceiveExpResult(0, false);

            var tamerInfos = _assets.TamerLevelInfo.Where(x => x.Type == tamer.Model).ToList();

            if (tamerInfos == null || !tamerInfos.Any())// || tamerInfos.Count != (int)GeneralSizeEnum.TamerLevelMax)
            {
                _logger.Error($"Incomplete level config for tamer {tamer.Model}.");
                return new ReceiveExpResult(0, false);
            }

            if (receivedExp < 0) receivedExp = 0;

            var currentLevel = tamerInfos.First(x => x.Level == tamer.Level);
            var expToGain = receivedExp;

            byte levelGain = 0;

            while (expToGain > 0)
            {
                if (expToGain + tamer.CurrentExperience >= currentLevel.ExpValue)
                {
                    var expToReceive = currentLevel.ExpValue - tamer.CurrentExperience;
                    tamer.ReceiveExp(expToReceive);
                    expToGain -= expToReceive;
                    levelGain++;
                    tamer.LevelUp();
                    currentLevel = tamerInfos.First(x => x.Level == tamer.Level);

                    if (tamer.Level == (int)GeneralSizeEnum.TamerLevelMax)
                    {
                        tamer.SetExp(0);
                        expToGain = 0;
                    }
                }
                else
                {
                    tamer.ReceiveExp(expToGain);
                    expToGain = 0;
                }
            }

            if (levelGain > 0)
                _logger.Verbose($"Character {tamer.Id} received {receivedExp} exp and gained {levelGain} levels.");
            else
                _logger.Verbose($"Character {tamer.Id} received {receivedExp} exp.");

            return new ReceiveExpResult(levelGain, true);
        }

        // --------------------------------------------------------------------------------------------

        public ReceiveExpResult ReceiveDigimonExperience(long receivedExp, DigimonModel digimon)
        {
            if (digimon.Level >= (int)GeneralSizeEnum.DigimonLevelMax)
            {
                //_logger.Information($"Digimon on max level");
                return new ReceiveExpResult(0, false);
            }
            else
            {
                var digimonInfos = _assets.DigimonLevelInfo.Where(x => x.ScaleType == digimon.BaseInfo.ScaleType).ToList();

                if (digimonInfos == null || !digimonInfos.Any())
                {
                    _logger.Error($"Incomplete level config for digimon {digimon.Model} {digimon.BaseInfo.ScaleType}.");
                    return new ReceiveExpResult(0, false);
                }

                if (receivedExp < 0) receivedExp = 0;

                var currentLevel = digimonInfos.First(x => x.Level == digimon.Level);
                var expToGain = receivedExp;

                byte levelGain = 0;

                while (expToGain > 0)
                {
                    if (expToGain + digimon.CurrentExperience >= currentLevel.ExpValue)
                    {
                        var expToReceive = currentLevel.ExpValue - digimon.CurrentExperience;
                        digimon.ReceiveExp(expToReceive);
                        expToGain -= expToReceive;
                        levelGain++;
                        digimon.LevelUp();
                        currentLevel = digimonInfos.First(x => x.Level == digimon.Level);

                        if (digimon.Level == (int)GeneralSizeEnum.DigimonLevelMax)
                        {
                            digimon.SetExp(0);
                            expToGain = 0;
                        }
                    }
                    else
                    {
                        digimon.ReceiveExp(expToGain);
                        expToGain = 0;
                    }
                }

                if (levelGain > 0)
                    _logger.Verbose($"Partner {digimon.Id} received {receivedExp} exp and gained {levelGain} levels.");
                else
                    _logger.Verbose($"Partner {digimon.Id} received {receivedExp} exp.");

                return new ReceiveExpResult(levelGain, true);

            }

        }

        public ReceiveExpResult ReceiveMaxDigimonExperience(DigimonModel digimon)
        {
            if (digimon.Level >= (int)GeneralSizeEnum.DigimonLevelMax)
                return new ReceiveExpResult(0, false);

            var digimonInfos = _assets.DigimonLevelInfo.Where(x => x.ScaleType == digimon.BaseInfo.ScaleType).ToList();

            if (digimonInfos == null || !digimonInfos.Any())// || digimonInfos.Count != (int)GeneralSizeEnum.DigimonLevelMax)
            {
                _logger.Error($"Incomplete level config for digimon {digimon.Model} {digimon.BaseInfo.ScaleType}.");
                return new ReceiveExpResult(0, false);
            }

            var levelGain = (byte)((int)GeneralSizeEnum.DigimonLevelMax - digimon.Level);

            digimon.LevelUp(levelGain);
            digimon.SetExp(0);

            _logger.Verbose($"Partner {digimon.Id} received max exp.");
            return new ReceiveExpResult(levelGain, true);
        }

        // --------------------------------------------------------------------------------------------

        internal void ReceiveAttributeExperience(GameClient client, DigimonModel partner, DigimonAttributeEnum targetAttribute, MobExpRewardConfigModel expReward)
        {
            short experience = 0;

            if (partner.BaseInfo.Attribute.HasAttributeAdvantage(targetAttribute))
            {
                _logger.Verbose($"Partner {partner.Id} received {expReward.NatureExperience / 2} nature exp.");
                experience = (short)(expReward.NatureExperience / 2);
            }
            else if (targetAttribute.HasAttributeAdvantage(partner.BaseInfo.Attribute))
            {
                _logger.Verbose($"Partner {partner.Id} lost 25 nature exp.");
                experience = (short)(expReward.NatureExperience * 2);
            }
            else
            {
                _logger.Verbose($"Partner {partner.Id} received {expReward.NatureExperience} nature exp.");
                experience = (short)expReward.NatureExperience;
            }

            int currentExp = partner.GetAttributeExperience();
            int maxExpGain = Math.Max(0,10000 - currentExp);

            if (experience > maxExpGain)
            {
                experience = (short)maxExpGain;
            }

            partner.ReceiveNatureExp(experience);
            
            var a = Enum.Parse(typeof(DigimonAttributePacketEnum),partner.BaseInfo.Attribute.ToString());

            if (partner.BaseInfo.Attribute != DigimonAttributeEnum.Data)
            {
                client.Send(new NatureExpPacket(0,(byte)(int)a,experience));
            }
        }

        internal void ReceiveAttributeExperience(GameClient client, DigimonModel partner, DigimonAttributeEnum targetAttribute, SummonMobExpRewardModel expReward)
        {
            short experience = 0;

            if (partner.BaseInfo.Attribute.HasAttributeAdvantage(targetAttribute))
            {
                experience = (short)(expReward.NatureExperience / 2);
            }
            else if (targetAttribute.HasAttributeAdvantage(partner.BaseInfo.Attribute))
            {
                experience = (short)(expReward.NatureExperience * 2);
            }
            else
            {
                experience = (short)expReward.NatureExperience;
            }

            int currentExp = partner.GetAttributeExperience();
            int maxExpGain = Math.Max(0,10000 - currentExp);

            if (experience > maxExpGain)
            {
                experience = (short)maxExpGain;
            }

            partner.ReceiveNatureExp(experience);
            
            var a = Enum.Parse(typeof(DigimonAttributePacketEnum),partner.BaseInfo.Attribute.ToString());
            if (partner.BaseInfo.Attribute != DigimonAttributeEnum.Data)
            {
                client.Send(new NatureExpPacket(0,(byte)(int)a,experience));
            }

        }

        // --------------------------------------------------------------------------------------------

        internal void ReceiveElementExperience(GameClient client, DigimonModel partner, DigimonElementEnum targetElement, MobExpRewardConfigModel expReward)
        {
            short experience = 0;

            if (partner.BaseInfo.Element.HasElementAdvantage(targetElement))
            {
                experience = (short)(expReward.ElementExperience / 2);
            }
            else if (targetElement.HasElementAdvantage(partner.BaseInfo.Element))
            {
                experience = (short)(expReward.ElementExperience * 2);
            }
            else
            {
                experience = (short)expReward.ElementExperience;
            }

            int currentExp = partner.GetElementExperience();
            int maxExpGain = Math.Max(0,10000 - currentExp);

            if (experience > maxExpGain)
            {
                experience = (short)maxExpGain;
            }

            partner.ReceiveElementExp(experience);
            
            var e = Enum.Parse(typeof(DigimonElementPacketEnum),partner.BaseInfo.Element.ToString());
            client.Send(new NatureExpPacket(1,(byte)(int)e,experience));

        }


        internal void ReceiveElementExperience(GameClient client, DigimonModel partner, DigimonElementEnum targetElement, SummonMobExpRewardModel expReward)
        {
            short experience = 0;

            if (partner.BaseInfo.Element.HasElementAdvantage(targetElement))
            {
                experience = (short)(expReward.ElementExperience / 2);
            }
            else if (targetElement.HasElementAdvantage(partner.BaseInfo.Element))
            {
                experience = (short)(expReward.ElementExperience * 2);
            }
            else
            {
                experience = (short)expReward.ElementExperience;
            }

            int currentExp = partner.GetElementExperience();
            int maxExpGain = Math.Max(0,10000 - currentExp);

            if (experience > maxExpGain)
            {
                experience = (short)maxExpGain;
            }

            partner.ReceiveElementExp(experience);
            var e = Enum.Parse(typeof(DigimonElementPacketEnum),partner.BaseInfo.Element.ToString());
            client.Send(new NatureExpPacket(1,(byte)(int)e,experience));
        }

        // --------------------------------------------------------------------------------------------
    }
}
