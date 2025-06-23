using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class InitialInfoPacket : PacketWriter
    {
        private const int PacketNumber = 1003;

        /// <summary>
        /// Initial information for character spawn.
        /// </summary>
        /// <param name="character">The tamer that is trying to log-in</param>
        public InitialInfoPacket(CharacterModel character, GameParty? party)
        {
            Type(PacketNumber);
            WriteInt(1);
            WriteInt(character.Location.X);
            WriteInt(character.Location.Y);
            WriteInt(character.GeneralHandler);
            WriteInt(character.Model.GetHashCode());
            WriteString(character.Name);
            WriteInt64(character.CurrentExperience * 100);
            WriteShort(character.Level);
            WriteInt(character.HP);
            WriteInt(character.DS);
            WriteInt(character.CurrentHp);
            WriteInt(character.CurrentDs);
            WriteInt(CharacterModel.Fatigue);
            WriteInt(character.AT);
            WriteInt(character.DE);
            WriteInt(character.MS);
            WriteBytes(character.Equipment.ToArray());
            WriteBytes(character.ChipSets.ToArray());
            WriteBytes(character.Digivice.ToArray());
            WriteBytes(character.TamerSkill.ToArray());
            WriteBytes(character.Progress.ToArray());
            WriteInt(character.Incubator.EggId);
            WriteInt(character.Incubator.HatchLevel);
            WriteInt(-1); // Egg TradeLimitTime
            WriteInt(character.Incubator.BackupDiskId);
            WriteInt(-1); // BackupDisk TradeLimitTime

            WriteShort((short)character.BuffList.ActiveBuffs.Count);
            foreach (var buff in character.BuffList.ActiveBuffs.ToList())
            {
                WriteShort((short)buff.BuffId);
                WriteShort((short)buff.TypeN);
                WriteInt(UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            WriteByte(character.DigimonSlots);
            WriteInt(character.Partner.GeneralHandler);
            WriteInt(character.Partner.CurrentType);
            WriteString(character.Partner.Name);
            WriteByte((byte)character.Partner.HatchGrade);
            WriteShort(character.Partner.Size);
            WriteInt64(character.Partner.CurrentExperience * 100);
            WriteInt64(character.Partner.TranscendenceExperience); //TODO: Transcend EXP
            WriteShort(character.Partner.Level);
            WriteInt(character.Partner.HP);
            WriteInt(character.Partner.DS);
            WriteInt(character.Partner.DE);
            WriteInt(character.Partner.AT);
            WriteInt(character.Partner.CurrentHp);
            WriteInt(character.Partner.CurrentDs);
            WriteInt(character.Partner.FS);
            WriteInt(0); //?
            WriteInt(character.Partner.EV);
            WriteInt(character.Partner.CC);
            WriteInt(character.Partner.MS);
            WriteInt(character.Partner.AS);
            WriteInt(0); //? 
            WriteInt(character.Partner.HT);
            WriteInt(0); //?
            WriteInt(0); //?
            WriteInt(character.Partner.AR);
            WriteInt(character.Partner.BL);
            WriteInt(character.Partner.BaseType);

            WriteByte((byte)character.Partner.Evolutions.Count);

            //TODO: teste com foreach
            for (int i = 0; i < character.Partner.Evolutions.Count; i++)
            {
                var form = character.Partner.Evolutions[i];
                WriteBytes(form.ToArray());
            }

            WriteShort(character.Partner.Digiclone.CloneLevel);
            WriteShort(character.Partner.Digiclone.ATValue);
            WriteShort(character.Partner.Digiclone.BLValue);
            WriteShort(character.Partner.Digiclone.CTValue);
            WriteShort(0); //DE Value (not implemented on client-side)
            WriteShort(character.Partner.Digiclone.EVValue);
            WriteShort(0); //HT Value (not implemented on client-side)
            WriteShort(character.Partner.Digiclone.HPValue);
            WriteShort(character.Partner.Digiclone.ATLevel);
            WriteShort(character.Partner.Digiclone.BLLevel);
            WriteShort(character.Partner.Digiclone.CTLevel);
            WriteShort(0); //DE Level (not implemented on client-side)
            WriteShort(character.Partner.Digiclone.EVLevel);
            WriteShort(0); //HT Level (not implemented on client-side)
            WriteShort(character.Partner.Digiclone.HPLevel);

            WriteShort((short)character.Partner.BuffList.ActiveBuffs.Count);
            foreach (var buff in character.Partner.BuffList.ActiveBuffs)
            {
                WriteShort((short)buff.BuffId);
                WriteShort((short)buff.TypeN);
                WriteInt(UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            WriteShort(character.Partner.AttributeExperience.Data);
            WriteShort(character.Partner.AttributeExperience.Vaccine);
            WriteShort(character.Partner.AttributeExperience.Virus);

            WriteShort(character.Partner.AttributeExperience.Ice);
            WriteShort(character.Partner.AttributeExperience.Water);
            WriteShort(character.Partner.AttributeExperience.Fire);
            WriteShort(character.Partner.AttributeExperience.Land);
            WriteShort(character.Partner.AttributeExperience.Wind);
            WriteShort(character.Partner.AttributeExperience.Wood);
            WriteShort(character.Partner.AttributeExperience.Light);
            WriteShort(character.Partner.AttributeExperience.Dark);
            WriteShort(character.Partner.AttributeExperience.Thunder);
            WriteShort(character.Partner.AttributeExperience.Steel);

            WriteInt(0); //nUID (não é mais utilizado?)
            WriteByte(0); //TODO: CashSkillCount (se passar acima de 0, informar o objeto)

            byte slot = 1;
            foreach (var digimon in character.ActiveDigimons)
            {
                WriteByte(slot);
                WriteUInt(digimon.GeneralHandler);
                WriteInt(digimon.BaseType);
                WriteString(digimon.Name);
                WriteByte((byte)digimon.HatchGrade);
                WriteShort(digimon.Size);
                WriteInt64(digimon.CurrentExperience * 100);
                WriteInt64(digimon.TranscendenceExperience); //Transcend EXP
                WriteShort(digimon.Level);
                WriteInt(digimon.HP);
                WriteInt(digimon.DS);
                WriteInt(digimon.DE);
                WriteInt(digimon.AT);
                WriteInt(digimon.CurrentHp);
                WriteInt(digimon.CurrentDs);
                WriteInt(digimon.FS);
                WriteInt(0); //?
                WriteInt(digimon.EV);
                WriteInt(digimon.CC);
                WriteInt(digimon.MS);
                WriteInt(digimon.AS);
                WriteInt(0); //? 
                WriteInt(digimon.HT);
                WriteInt(0); //?
                WriteInt(0); //?
                WriteInt(0); //?
                WriteInt(digimon.BL);
                WriteInt(digimon.BaseType);

                WriteByte((byte)digimon.Evolutions.Count);
                //TODO: teste com foreach
                for (int i = 0; i < digimon.Evolutions.Count; i++)
                {
                    var form = digimon.Evolutions[i];
                    WriteBytes(form.ToArray());
                }

                WriteShort(digimon.Digiclone.CloneLevel);
                WriteShort(digimon.Digiclone.ATValue);
                WriteShort(digimon.Digiclone.BLValue);
                WriteShort(digimon.Digiclone.CTValue);
                WriteShort(0); //DE Value (not implemented on client-side)
                WriteShort(digimon.Digiclone.EVValue);
                WriteShort(0); //HT Value (not implemented on client-side)
                WriteShort(digimon.Digiclone.HPValue);
                WriteShort(digimon.Digiclone.ATLevel);
                WriteShort(digimon.Digiclone.BLLevel);
                WriteShort(digimon.Digiclone.CTLevel);
                WriteShort(0); //DE Level (not implemented on client-side)
                WriteShort(digimon.Digiclone.EVLevel);
                WriteShort(0); //HT Level (not implemented on client-side)
                WriteShort(digimon.Digiclone.HPLevel);

                WriteShort(digimon.AttributeExperience.Data);
                WriteShort(digimon.AttributeExperience.Vaccine);
                WriteShort(digimon.AttributeExperience.Virus);

                WriteShort(digimon.AttributeExperience.Ice);
                WriteShort(digimon.AttributeExperience.Water);
                WriteShort(digimon.AttributeExperience.Fire);
                WriteShort(digimon.AttributeExperience.Land);
                WriteShort(digimon.AttributeExperience.Wind);
                WriteShort(digimon.AttributeExperience.Wood);
                WriteShort(digimon.AttributeExperience.Light);
                WriteShort(digimon.AttributeExperience.Dark);
                WriteShort(digimon.AttributeExperience.Thunder);
                WriteShort(digimon.AttributeExperience.Steel);

                WriteInt(16404); //16404
                WriteByte(0);

                slot++;
            }

            WriteByte(99); //Define fim do loop de load dos digimons
            WriteInt(0);
            WriteInt(character.Channel);
            WriteBytes(character.SerializeMapRegion());
            WriteInt(character.DigimonArchive.Slots);

            if (party != null)
            {
                WriteUInt((uint)party.Id);

                WriteInt((int)party.LootType); //loot type
                WriteByte((byte)party.LootFilter); //rare rate
                WriteByte(0); //rare grade
                WriteByte((byte)(party.LeaderSlot)); //Party leader slot

                foreach (var member in party.Members.Where(x => x.Value.Id != character.Id))
                {
                    WriteByte(member.Key);
                    if (character.Channel == member.Value.Channel &&
                        character.Location.MapId == member.Value.Location.MapId)
                    {
                        WriteInt(member.Value.GeneralHandler);
                        WriteInt(member.Value.Partner.GeneralHandler);
                    }
                    else
                    {
                        WriteInt(0);
                        WriteInt(0);
                    }

                    WriteInt(member.Value.Model.GetHashCode());
                    WriteShort(member.Value.Level);
                    WriteString(member.Value.Name);

                    WriteInt(member.Value.Partner.CurrentType);
                    WriteShort(member.Value.Partner.Level);
                    WriteString(member.Value.Partner.Name);

                    WriteInt(member.Value.Location.MapId);
                    WriteInt(member.Value.Channel);
                }

                WriteByte(99);
            }
            else
            {
                WriteInt(0); //Party Id
                WriteInt(0); //Party loot type
                WriteByte(0); //Rare Rate
                WriteByte(0); //Rare Grade
                WriteByte(0); //Party leader slot

                WriteByte(99); //Fim do loop de party member
            }

            WriteShort(character.CurrentTitle);

            //ItemCooldown(max 32)
            for (int i = 0; i < 32; i++)
                WriteInt(0);

            WriteInt(0); //versão do game
            WriteInt(2); //nWorkDayHistory (total de dias evento login diario)
            WriteInt(0); //nTodayAttendanceTimeTS (tempo restante do dia atual)
            WriteInt(0); //Id Boss vivo no mapa atual (já é passado no ComplementarInfo)
            WriteByte(0); //PC Bang (???)

            //ConsignedShop
            if (character.ConsignedShop != null)
            {
                WriteInt(character.ConsignedShop.Location.MapId);
                WriteInt(character.ConsignedShop.Channel); //Channel
                WriteInt(character.ConsignedShop.Location.X); //X
                WriteInt(character.ConsignedShop.Location.Y); //Y
                WriteInt(character.ConsignedShop.ItemId); //itemid
            }
            else
                WriteInt(0);

            WriteInt(0); //clientOption (aparenta ter relação com o tutorial)
            WriteInt(0); //Achievement rank

            WriteByte(0); //hatch atual já rodou minigame
            WriteShort(0); //total de sucesso do minigame (TODO: externalizar % por tentativa bem sucedida)

            var Buffs = character.ActiveSkill.Where(x =>
                x.Type == Enums.ClientEnums.TamerSkillTypeEnum.Normal && x.SkillId > 0 &&
                x.RemainingCooldownSeconds > 0).ToList();

            if (Buffs.Any())
            {
                WriteByte((byte)Buffs.Count);
                foreach (var buff in Buffs)
                {
                    WriteInt(buff.SkillId);

                    if (buff.RemainingCooldownSeconds > 0)
                    {
                        WriteInt(UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingCooldownSeconds));
                    }
                    else
                    {
                        WriteInt(0);
                    }
                }
            }
            else
            {
                WriteByte(0);
            }

            var cashBuffs = character.ActiveSkill.Where(x =>
                    x.Type == Enums.ClientEnums.TamerSkillTypeEnum.Cash && x.SkillId > 0 && x.RemainingMinutes > 0)
                .ToList();

            if (cashBuffs.Any())
            {
                WriteByte((byte)cashBuffs.Count);
                foreach (var buff in cashBuffs)
                {
                    if (buff.RemainingMinutes > 0)
                    {
                        WriteInt(buff.SkillId);

                        WriteInt(UtilitiesFunctions.RemainingTimeMinutes(buff.RemainingMinutes));
                        if (buff.RemainingCooldownSeconds > 0)
                        {
                            WriteInt(UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingCooldownSeconds));
                        }
                        else
                        {
                            WriteInt(0);
                        }
                    }
                    else
                    {
                        WriteInt(0);
                        WriteInt(0);
                        WriteInt(0);
                    }
                }
            }
            else
            {
                WriteByte(0);
            }

            WriteByte(0); //bloqueio de chat (se passar 1, informar a duração)
            //WriteInt(60);//Duração block chat em segundos 
            //Obs.: Passar 0 posteriormente não remove o valor da duração passado anteriormente.
            WriteByte(0); //master match (1 = equipe A, 2 = equipe B)

            // TODO: Encyclopedia deckbuff the most OP is 1021, this is the DeckGroupID in xml
            if (character.DeckBuffId == null)
            {
                WriteByte(0);
            }
            else
            {
                WriteInt((int)character.DeckBuffId);
            }

            WriteByte(0); //Megaphone ban (1 = block)

            WriteInt(0);

            WriteBytes(new byte[29]);
        }
    }
}