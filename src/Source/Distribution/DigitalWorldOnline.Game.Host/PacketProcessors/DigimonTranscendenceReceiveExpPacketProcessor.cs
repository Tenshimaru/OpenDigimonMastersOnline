using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.Infrastructure.Migrations;
using MediatR;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Sockets;
using System.Reflection;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigimonTranscendenceExpPacketProcessor : IGamePacketProcessor
    {
        private readonly double[,] cloneValues = {
        { 7.17, 8.03, 8.89, 9.74, 10.6, 11.46, 12.31, 13.17, 14.03, 14.89, 15.74 },
        { 7.43, 8.29, 9.14, 10, 10.86, 11.71, 12.57, 13.43, 14.29, 15.14, 16 },
        { 7.71, 8.57, 9.43, 10.29, 11.14, 12, 12.86, 13.71, 14.57, 15.43, 16.29 },
        { 8, 8.86, 9.71, 10.57, 11.43, 12.29, 13.14, 14, 14.86, 15.71, 16.57 },
        { 8.29, 9.14, 10, 10.86, 11.71, 12.57, 13.43, 14.29, 15.14, 16, 16.86 },
        { 8.57, 9.43, 10.29, 11.14, 12, 12.86, 13.71, 14.57, 15.43, 16.29, 17.14 },
        { 8.86, 9.71, 10.57, 11.43, 12.29, 13.14, 14, 14.86, 15.71, 16.57, 17.43 },
        { 9.14, 10, 10.86, 11.71, 12.57, 13.43, 14.29, 15.14, 16, 16.86, 17.71 },
        { 9.43, 10.29, 11.14, 12, 12.86, 13.71, 14.57, 15.43, 16.29, 17.14, 18 },
        { 9.71, 10.57, 11.43, 12.29, 13.14, 14, 14.86, 15.71, 16.57, 17.43, 18.29 },
        { 9.97, 10.83, 11.69, 12.54, 13.4, 14.26, 15.11, 15.97, 16.83, 17.68, 18.54 },
        { 10, 10.86, 11.71, 12.57, 13.43, 14.29, 15.14, 16, 16.86, 17.71, 18.57 },
        { 10.29, 11.14, 12, 12.86, 13.71, 14.57, 15.43, 16.29, 17.14, 18, 18.86 },
        { 10.57, 11.43, 12.29, 13.14, 14, 14.86, 15.71, 16.57, 17.43, 18.29, 19.14 }
    };
        public GameServerPacketEnum Type => GameServerPacketEnum.TranscendenceReceiveExpResult;

        private readonly StatusManager _statusManager;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly List<short> _transcendSlots = new();
        private readonly Random rand = new Random();

        public DigimonTranscendenceExpPacketProcessor(
            ILogger logger,
            ISender sender,
            IMapper mapper,
            StatusManager statusManager
         )
        {
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _statusManager = statusManager;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            packet.Skip(2);
            short targetAcademySlot = packet.ReadShort();
            _ = packet.ReadByte();
            var npcId = packet.ReadInt();
            var inputType = (AcademyInputType)packet.ReadByte();
            byte archiveSlot = packet.ReadByte();
            short digimonCount = packet.ReadShort();

            for (int i = 0; i < digimonCount; i++)
            {
                var academySlot = packet.ReadShort();
                _logger.Debug($"academySlot: {academySlot}");

                _transcendSlots.Add(academySlot);
            }

            _ = packet.ReadShort(); //Digimon Trans Slot
            _ = packet.ReadInt(); //ItemId
            short itemSlot = packet.ReadShort();
            short amount = packet.ReadShort();

            var successRate = 0;
            long chargeExp = 0;

            var digivicePartner = client.Tamer.Digimons.FirstOrDefault(x => x.Slot == archiveSlot);

            var targetItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);

            if (targetItem == null)
                return;

            if (digivicePartner == null)
                return;

            foreach (var targetSlot in _transcendSlots)
            {
                var Chance = 5;
                var Success = rand.Next(0, 100);

                var targetPartner = client.Tamer.DigimonArchive.DigimonArchives.First(x => x.Slot == targetSlot);

                if (targetPartner == null)
                    return;

                if (targetPartner.Digimon == null)
                {
                    targetPartner.SetDigimonInfo(_mapper.Map<DigimonModel>(await _sender.Send(new GetDigimonByIdQuery(targetPartner.DigimonId))));
                    targetPartner.Digimon?.SetBaseInfo(_statusManager.GetDigimonBaseInfo(targetPartner.Digimon.BaseType));
                }
                else//if (targetPartner.Digimon != null)
                {
                    if (!targetPartner.Digimon.IsRaremonType)
                    {
                        if (inputType == AcademyInputType.Low)
                        {
                            long Exp = 0;
                            long BonusExp = 0;

                            float[] values = ExperienceLowValues(targetPartner.Digimon.HatchGrade, targetPartner.Digimon.Digiclone.CloneLevel, targetPartner.Digimon.Level);

                            if (digivicePartner.SameType(targetPartner.Digimon.BaseType))
                            {

                                Exp = RoundAndMultiply(values[0] * 3, 1000);
                                BonusExp = RoundAndMultiply(values[1] * 3, 100);
                            }
                            else
                            {
                                Exp = RoundAndMultiply(values[0], 1000);
                                BonusExp = RoundAndMultiply(values[1], 1000);
                            }

                            float initialValue = Exp;
                            float targetPercentage = initialValue / 1000;


                            Exp = (long)ConvertPercentageToValue(targetPercentage);

                            float initialValueBonusExp = BonusExp;
                            float BonusExptargetPercentage = initialValueBonusExp / 1000;

                            BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                            if (Chance >= Success)
                            {
                                successRate = 1;
                                chargeExp += BonusExp;
                                digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                            }
                            else
                            {
                                Exp += digivicePartner.TranscendenceExperience;
                                digivicePartner.UpdateTranscendenceExp((long)Exp);
                            }
                        }
                        else if (inputType == AcademyInputType.High)
                        {

                            long Exp = 0;
                            long BonusExp = 0;

                            float[] values = ExperienceMidValues(targetPartner.Digimon.HatchGrade, targetPartner.Digimon.Digiclone.CloneLevel, targetPartner.Digimon.Level);

                            if (digivicePartner.SameType(targetPartner.Digimon.BaseType))
                            {

                                Exp = RoundAndMultiply(values[2], 1000);
                                BonusExp = RoundAndMultiply(values[1] * 3, 100);
                            }
                            else
                            {
                                Exp = RoundAndMultiply(values[0], 1000);
                                BonusExp = RoundAndMultiply(values[1], 1000);
                            }
                            float initialValue = Exp;
                            float targetPercentage = initialValue / 1000;

                            Exp = (long)ConvertPercentageToValue(targetPercentage);

                            float initialValueBonusExp = BonusExp;
                            float BonusExptargetPercentage = initialValueBonusExp / 1000;

                            BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                            if (Chance >= Success)
                            {
                                successRate = 1;
                                chargeExp += BonusExp;
                                digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                            }
                            else
                            {
                                Exp += digivicePartner.TranscendenceExperience;
                                digivicePartner.UpdateTranscendenceExp((long)Exp);
                            }
                        }

                    }
                    else if (targetPartner.Digimon.IsRaremonType)
                    {
                        if (inputType == AcademyInputType.Low)
                        {
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.Default)
                            {
                                float NormalValue = 2.24f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;


                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);

                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }
                            }
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.High)
                            {
                                float NormalValue = 16.82f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;


                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);


                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }

                            }
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.Perfect)
                            {
                                float NormalValue = 44.85f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;

                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);

                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }
                            }
                        }
                        else if (inputType == AcademyInputType.High)
                        {
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.Default)
                            {
                                float NormalValue = 3.58f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;




                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);


                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }
                            }
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.High)
                            {
                                float NormalValue = 26.91f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;


                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);


                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }
                            }
                            if (targetPartner.Digimon.HatchGrade == DigimonHatchGradeEnum.Perfect)
                            {
                                float NormalValue = 71.76f;
                                float DoubleValue = NormalValue * 2;

                                long Exp = 0;
                                long BonusExp = 0;

                                Exp = RoundAndMultiply(NormalValue, 1000);
                                BonusExp = RoundAndMultiply(DoubleValue, 1000);


                                float initialValue = Exp;
                                float targetPercentage = initialValue / 1000;

                                Exp = (long)ConvertPercentageToValue(targetPercentage);

                                float initialValueBonusExp = BonusExp;
                                float BonusExptargetPercentage = initialValueBonusExp / 1000;

                                BonusExp = (long)ConvertPercentageToValue(BonusExptargetPercentage);

                                if (Chance >= Success)
                                {
                                    successRate = 1;
                                    chargeExp += BonusExp;
                                    digivicePartner.UpdateTranscendenceExp((long)(BonusExp + digivicePartner.TranscendenceExperience));
                                }
                                else
                                {
                                    Exp += digivicePartner.TranscendenceExperience;
                                    digivicePartner.UpdateTranscendenceExp((long)Exp);
                                }
                            }
                        }
                    }

                    targetPartner.RemoveDigimon();
                    await _sender.Send(new UpdateCharacterDigimonArchiveItemCommand(targetPartner));
                    await _sender.Send(new DeleteDigimonCommand(targetPartner.Digimon.Id));

                    client.Tamer.RemoveDigimon((byte)targetPartner.Slot);
                }

            }

            client.Tamer.Inventory.RemoveOrReduceItem(targetItem, amount, itemSlot);
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

            long expEarned = digivicePartner.TranscendenceExperience;
            if (expEarned > 140000) expEarned = 140000;

            client.Send(new DigimonTranscendenceReceiveExpPacket(inputType, (byte)targetAcademySlot, digimonCount, _transcendSlots, itemSlot, targetItem, (short)successRate, chargeExp, (long)expEarned));
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());

            _transcendSlots.Clear();

            await _sender.Send(new UpdateDigimonExperienceCommand(digivicePartner));
        }

        static int RoundAndMultiply(float value, int multiplier)
        {
            double roundedValue = Math.Round(value, 2); // Round to two decimal places
            int multipliedValue = (int)(roundedValue * multiplier);
            return multipliedValue;
        }
        static long ConvertPercentageToValue(float percentage)
        {
            // O valor máximo correspondente a 100% (140000)
            long maxValue = 140000;

            // Calcula o valor correspondente à porcentagem fornecida
            int calculatedValue = (int)(maxValue * (percentage / 100.0f));

            // Garante que o valor calculado não exceda o valor máximo
            long finalValue = Math.Min(calculatedValue, maxValue);

            return finalValue;
        }
        float[] ExperienceLowValues(DigimonHatchGradeEnum Scale, short ClonLevel, int Level)
        {
            ClonLevel -= 1;

            var ReturnValue = new float[3];
            double Value = 0;

            if (Scale == DigimonHatchGradeEnum.Default)
            {
                Value = (ClonLevel + Level + 250) * 10 * 1.0 / 1400 / 10;
            }
            else if (Scale == DigimonHatchGradeEnum.High)
            {
                Value = (ClonLevel + Level + 250) * 20 * 1.0 / 1400 / 2;
            }
            else if (Scale == DigimonHatchGradeEnum.Perfect)
            {
                Value = (ClonLevel + Level + 250) * 40 * 1.0 / 1400;
            }

            var BonusValue = Value * 2;

            ReturnValue[0] = (float)Value;
            ReturnValue[1] = (float)BonusValue;

            return ReturnValue;
        }
        float[] ExperienceMidValues(DigimonHatchGradeEnum Scale, short ClonLevel, int Level)
        {
            ClonLevel -= 1;

            var ReturnValue = new float[3];
            double Value = 0;

            if (Scale == DigimonHatchGradeEnum.Default)
            {
                Value = (ClonLevel + Level + 250) * 10 * 1.0 / 1400 / 10;
            }
            else if (Scale == DigimonHatchGradeEnum.High)
            {
                Value = (ClonLevel + Level + 250) * 20 * 1.0 / 1400 / 2;
            }
            else if (Scale == DigimonHatchGradeEnum.Perfect)
            {
                Value = GetCloneValue(Level, ClonLevel);

            }

            var multiplier = Value * 0.6;
            Value = Value + multiplier;


            var BonusValue = Value * 2;

            ReturnValue[0] = (float)Value;
            ReturnValue[1] = (float)BonusValue;
            ReturnValue[2] = (float)Value * 3;

            return ReturnValue;
        }
        double GetCloneValue(int level, short clone)
        {
            if (level < 1 || level > 120 || clone < 0 || clone > 60 || clone % 6 != 0)
            {
                throw new ArgumentException("Invalid level or clone value");
            }

            int levelIndex = 0;

            if (level > 1 && level <= 10)
            {
                levelIndex = 0;
            }
            else if (level > 10 && level <= 20)
            {
                levelIndex = 1;
            }
            else if (level > 20 && level <= 30)
            {
                levelIndex = 2;
            }
            else if (level > 30 && level <= 40)
            {
                levelIndex = 3;
            }
            else if (level > 40 && level <= 50)
            {
                levelIndex = 4;
            }
            else if (level > 50 && level <= 60)
            {
                levelIndex = 5;
            }
            else if (level > 60 && level <= 70)
            {
                levelIndex = 6;
            }
            else if (level > 70 && level <= 80)
            {
                levelIndex = 7;
            }
            else if (level > 80 && level <= 90)
            {
                levelIndex = 8;
            }
            else if (level > 90 && level <= 99)
            {
                levelIndex = 9;
            }
            else if (level == 99)
            {
                levelIndex = 10;
            }
            else if (level >= 100 && level <= 110)
            {
                levelIndex = 11;
            }
            else if (level > 110 && level < 120)
            {
                levelIndex = 12;
            }
            else
            {
                levelIndex = 13;
            }

            int cloneIndex = clone / 6;


            return cloneValues[levelIndex, cloneIndex];
        }
    }
}
