using DigitalWorldOnline.Commons.DTOs.Chat;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllChatMessagesQuery : IRequest<IList<ChatMessageDTO>>
    {
    }
}