using DigitalWorldOnline.Commons.DTOs.Chat;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllChatMessagesQueryHandler : IRequestHandler<AllChatMessagesQuery, IList<ChatMessageDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public AllChatMessagesQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<ChatMessageDTO>> Handle(AllChatMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllChatMessagesAsync();
        }
    }
}
