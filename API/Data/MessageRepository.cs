using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Messages messages)
        {
            _context.Messages.Add(messages);
        }

        public void DeleteMessage(Messages messages)
        {
            _context.Messages.Remove(messages);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(x => x.Connections).Where(x => x.Connections.Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync();
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Messages> GetMessages(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDtos>> GetMessagesForUsers(MessagesParams messagesParams)
        {
            var query = _context.Messages.OrderByDescending(a => a.MessageSent).AsQueryable();
            query = messagesParams.Container switch
            {
                "Inbox" => query.Where(a => a.RecipientUserName == messagesParams.UserName && a.RecipientDeleted == false),
                "Outbox" => query.Where(a => a.SenderUserName == messagesParams.UserName && a.SenderDeleted == false),
                _ => query.Where(a => a.RecipientUserName == messagesParams.UserName && a.DateRead == null && a.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDtos>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDtos>.CreateAync(messages, messagesParams.PageNumber, messagesParams.PageSize);
        }

        public async Task<IEnumerable<MessageDtos>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(u => u.Photos)
            .Include(u => u.Recipient).ThenInclude(u => u.Photos)
            .Where
            (
                m => m.RecipientUserName == currentUserName && m.RecipientDeleted == false && m.SenderUserName == recipientUserName || m.RecipientUserName == recipientUserName && m.SenderUserName == currentUserName && m.SenderDeleted == false
            )
            .OrderBy(u => u.MessageSent).ToListAsync();

            var unreadMessages = messages.Where(a => a.DateRead == null && a.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDtos>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}