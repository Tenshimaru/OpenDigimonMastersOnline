using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class XaiInformationQuery : IRequest<XaiAssetDTO>
    {
        public int ItemId { get; private set; }

        public XaiInformationQuery(int itemId)
        {
            ItemId = itemId;
        }
    }
}