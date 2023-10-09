using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Messages messages);

        void DeleteMessage(Messages messages);

        Task<Messages> GetMessages(int id);

        Task<PagedList<MessageDtos>> GetMessagesForUsers(MessagesParams messagesParams);

        Task<IEnumerable<MessageDtos>> GetMessageThread(string currentUserName, string recipientUserName);

        Task<bool> SaveAllAsync();
    }
}