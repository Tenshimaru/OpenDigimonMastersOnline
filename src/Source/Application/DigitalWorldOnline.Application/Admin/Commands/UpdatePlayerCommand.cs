using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdatePlayerCommand : IRequest<bool>
    {
        public long Id { get; }
        public string Name { get; }
        public byte Level { get; }
        public long CurrentExperience { get; }
        public int MapId { get; }
        public CharacterStateEnum State { get; }
        public CharacterEventStateEnum EventState { get; }
        public byte Channel { get; }
        public CharacterModelEnum Model { get; }
        public short Size { get; }
        public int CurrentHp { get; }
        public int CurrentDs { get; }
        public int XGauge { get; }
        public short XCrystals { get; }
        public short CurrentTitle { get; }
        public byte DigimonSlots { get; }

        public UpdatePlayerCommand(
            long id,
            string name,
            byte level,
            long currentExperience,
            int mapId,
            CharacterStateEnum state,
            CharacterEventStateEnum eventState,
            byte channel,
            CharacterModelEnum model,
            short size,
            int currentHp,
            int currentDs,
            int xGauge,
            short xCrystals,
            short currentTitle,
            byte digimonSlots)
        {
            Id = id;
            Name = name;
            Level = level;
            CurrentExperience = currentExperience;
            MapId = mapId;
            State = state;
            EventState = eventState;
            Channel = channel;
            Model = model;
            Size = size;
            CurrentHp = currentHp;
            CurrentDs = currentDs;
            XGauge = xGauge;
            XCrystals = xCrystals;
            CurrentTitle = currentTitle;
            DigimonSlots = digimonSlots;
        }
    }
}
