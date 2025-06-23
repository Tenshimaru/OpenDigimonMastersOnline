using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ChannelsByMapIdQuery : IRequest<IDictionary<byte, byte>>
    {
        public short MapId { get; set; }

        public ChannelsByMapIdQuery(short mapId)
        {
            MapId = mapId;
        }
    }
}

