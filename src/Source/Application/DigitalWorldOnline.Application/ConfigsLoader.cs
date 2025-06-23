using AutoMapper;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application
{
    public class ConfigsLoader
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private bool? _loading;

        public bool Loading => _loading == null || _loading.Value;

        public List<CloneConfigModel> Clones { get; private set; }
        public List<HatchConfigModel> Hatchs { get; private set; }
        public List<FruitConfigModel> Fruits { get; private set; }
        public List<EventConfigModel> Events { get; private set; }

        public ConfigsLoader(
            ISender sender,
            IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        public ConfigsLoader Load()
        {
            Task.Run(LoadConfigs);

            return this;
        }

        private async Task LoadConfigs()
        {
            if (_loading != null)
                return;

            _loading = true;

            Clones = _mapper.Map<List<CloneConfigModel>>(await _sender.Send(new CloneConfigsQuery()));
            Hatchs = _mapper.Map<List<HatchConfigModel>>(await _sender.Send(new HatchConfigsQuery()));
            Fruits = _mapper.Map<List<FruitConfigModel>>(await _sender.Send(new FruitConfigsQuery()));
            Events = _mapper.Map<List<EventConfigModel>>(await _sender.Send(new EventsConfigQuery()));

            _loading = false;
        }
    }
}
