using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.GameHost;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ActiveTamerSkillRemovePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ActiveTamerCashSkillRemove;

        private readonly ISender _sender;


        public ActiveTamerSkillRemovePacketProcessor(
            ISender sender)
        {
            _sender = sender;
        }
        public async Task Process(GameClient client, byte[] packetData)
        {

            var packet = new GamePacketReader(packetData);

            int skillId = packet.ReadInt();

            var activeSkill = client.Tamer.ActiveSkill.FirstOrDefault(x => x.SkillId == skillId);

            activeSkill.SetTamerSkill(0, 0, Commons.Enums.ClientEnums.TamerSkillTypeEnum.Normal);

            client?.Send(new ActiveTamerCashSkillRemove(skillId));
            await _sender.Send(new UpdateTamerSkillCooldownByIdCommand(activeSkill));

        }


    }

}

