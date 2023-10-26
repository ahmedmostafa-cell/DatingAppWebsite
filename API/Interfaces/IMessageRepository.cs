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



        void AddGroup(Group group);

        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);

        Task<Group> GetMessageGroup(string groupName);


        Task<Group> GetGroupForConnection(string connectionId);
    }
}