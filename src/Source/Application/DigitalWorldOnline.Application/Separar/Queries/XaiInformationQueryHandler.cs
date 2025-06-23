using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class XaiInformationQueryHandler : IRequestHandler<XaiInformationQuery, XaiAssetDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public XaiInformationQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<XaiAssetDTO> Handle(XaiInformationQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetXaiInformationAsync(request.ItemId);
        }
    }
}