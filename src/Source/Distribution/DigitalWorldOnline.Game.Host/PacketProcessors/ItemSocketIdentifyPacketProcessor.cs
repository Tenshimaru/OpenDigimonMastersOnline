using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using MediatR;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Sockets;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemSocketIdentifyPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ItemSocketIdentify;
        private readonly MapServer _mapServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly AssetsLoader _assets;
        public ItemSocketIdentifyPacketProcessor(
            MapServer mapServer,
            ILogger logger,
            ISender sender,
            AssetsLoader assets)
        {
            _mapServer = mapServer;
            _logger = logger;
            _sender = sender;
            _assets = assets;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _ = packet.ReadInt();
            var vipEnabled = packet.ReadByte();
            int npcId = packet.ReadInt();
            short slot = packet.ReadShort();

            await ItemIdentify(client, slot);

        }

        private async Task ItemIdentify(GameClient client, short slot)
        {
            var itemInfo = client.Tamer.Inventory.FindItemBySlot(slot);

            if (itemInfo != null)
            {
                client.Tamer.Inventory.RemoveBits(itemInfo.ItemInfo.ScanPrice / 2);
                var i = 0;

                foreach (var apply in itemInfo.ItemInfo.SkillInfo.Apply)
                {
                           
     
                    int nLv = (itemInfo.ItemInfo.Type == 63) ? (int)itemInfo.ItemInfo.TypeN : (int)itemInfo.ItemInfo.TypeN;
                    int nValue = 0;

                    nValue = GetSkillAtt(itemInfo, nLv, i);

                    Random random = new Random();

                    int ApplyRate = random.Next(itemInfo.ItemInfo.ApplyValueMin, itemInfo.ItemInfo.ApplyValueMax);

                    int valorAleatorio = (int)((double)ApplyRate * nValue / 100);

                    itemInfo.AccessoryStatus[i].SetType((AccessoryStatusTypeEnum)apply.Attribute);
                    itemInfo.AccessoryStatus[i].SetValue((short)valorAleatorio);

                    itemInfo.SetPower((byte)ApplyRate); //TODO: externalizar
                    itemInfo.SetReroll(100);

                    break;
                }

                client.Send(new ItemSocketIdentifyPacket(itemInfo,(int)client.Tamer.Inventory.Bits));

                await _sender.Send(new UpdateItemAccessoryStatusCommand(itemInfo));
                await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            }
        }

        public int GetSkillAtt(ItemModel item, int nSkillLevel, int nApplyIndex)
        {


            var Skill = item.ItemInfo.SkillInfo;

            bool bDigimonSkill = IsDigimonSkill((int)item.ItemInfo.SkillCode);

            if (!bDigimonSkill ||   // 디지몬 스킬이 아닌경우(아이템/테이머) 와
                nApplyIndex == 0)   // 디지몬 공격스킬 효과는 기존 공식 사용
                return Skill.Apply[nApplyIndex].Value + nSkillLevel * Skill.Apply[nApplyIndex].IncreaseValue;

            //디지몬 특수스킬 효과는 레벨을 1 빼주고 계산
            switch ((ApplyStatusEnum)Skill.Apply[nApplyIndex].Attribute)
            {
                // 크리티컬 / 회피 증가 스킬 추가 14.05.28 chu8820
                case ApplyStatusEnum.APPLY_CA:
                case ApplyStatusEnum.APPLY_EV:
                    {
                        if (Skill.Apply[nApplyIndex].Type == SkillCodeApplyTypeEnum.Unknown207)
                        {
                            int nValue = Skill.Apply[nApplyIndex].Value + (nSkillLevel - 1) * Skill.Apply[nApplyIndex].IncreaseValue;
                            return (nValue * 100);
                        }
                        else //if( pFTSkill->s_Apply[ nApplyIndex ].s_nID == 206/*nSkill::Me_206*/ )
                            return Skill.Apply[nApplyIndex].Value + (nSkillLevel - 1) * Skill.Apply[nApplyIndex].IncreaseValue;
                    }
                default:
                    return Skill.Apply[nApplyIndex].Value + (nSkillLevel - 1) * Skill.Apply[nApplyIndex].IncreaseValue;
            }

            return Skill.Apply[nApplyIndex].Value + nSkillLevel * Skill.Apply[nApplyIndex].IncreaseValue;
        }

        public bool IsDigimonSkill(int SkillId)
        {
            if (SkillId / 100 == 21134 || SkillId / 100 == 21137)// 페티메라몬, 시드몬 예외처리
            {
                return true;
            }
            else
            {
                return (SkillId / 1000000 >= 3 && SkillId / 1000000 <= 7);
            }


        }
    }
}