using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class SetTitlePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.SetTitle;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ISender _sender;

        public SetTitlePacketProcessor(AssetsLoader assets, MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer, ISender sender)
        {
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var titleId = packet.ReadShort();

            var OldTitleBuff = _assets.AchievementAssets.FirstOrDefault(x => x.QuestId == client.Tamer.CurrentTitle && x.BuffId > 0);

            if (OldTitleBuff != null)
            {
                foreach (var partner in client.Tamer.Digimons.Where( x=> x.Id != client.Tamer.Partner.Id))
                {
                    if(partner.BuffList.ForceExpired(OldTitleBuff.BuffId))
                    {
                        partner.BuffList.Remove(OldTitleBuff.BuffId);

                        await _sender.Send(new UpdateDigimonBuffListCommand(partner.BuffList));
                    }
                }

                if (client.Partner.BuffList.ForceExpired(OldTitleBuff.BuffId))
                {
                    client.Partner.BuffList.Remove(OldTitleBuff.BuffId);
                    client?.Send(new RemoveBuffPacket(client.Partner.GeneralHandler, OldTitleBuff.BuffId));
                }
            }

            var newTitle = _assets.AchievementAssets.FirstOrDefault(x => x.QuestId == titleId && x.BuffId > 0);

            if (newTitle != null)
            {
                var buff = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == newTitle.BuffId);

                var duration = UtilitiesFunctions.RemainingTimeSeconds(0);

                var newDigimonBuff = DigimonBuffModel.Create(buff.BuffId, buff.SkillId);

                newDigimonBuff.SetBuffInfo(buff);

                foreach (var partner in client.Tamer.Digimons.Where(x => x.Id != client.Tamer.Partner.Id))
                {
                    var partnernewDigimonBuff = DigimonBuffModel.Create(buff.BuffId, buff.SkillId);

                    partnernewDigimonBuff.SetBuffInfo(buff);

                    partner.BuffList.Add(partnernewDigimonBuff);

                    await _sender.Send(new UpdateDigimonBuffListCommand(partner.BuffList));                  
                }

                client.Partner.BuffList.Add(newDigimonBuff);

                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());
                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());
                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());
                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());
            }

            client.Tamer.UpdateCurrentTitle(titleId);

            //client.Partner?.SetTitleStatus(_assets.TitleStatus.FirstOrDefault(x => x.ItemId == client.Tamer.CurrentTitle));

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapConfig?.Type)
            {
                case MapTypeEnum.Dungeon:
                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());
                    break;
                case MapTypeEnum.Event:
                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());
                    break;
                case MapTypeEnum.Pvp:
                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());
                    break;
                default:
                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());
                    break;
            }

            client.Send(new UpdateStatusPacket(client.Tamer));

            await _sender.Send(new UpdateCharacterTitleCommand(client.TamerId, titleId));
            //await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
        }
    }
}