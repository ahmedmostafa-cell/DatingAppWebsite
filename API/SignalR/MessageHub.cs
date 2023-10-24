using System.Security.AccessControl;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _presenceHub = presenceHub;

        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Caller.SendAsync("RecieveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUserName();
            if (username == createMessageDto.RecipientUserName)
                throw new HubException("You Can Not Send Message To Your Self");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);


            if (recipient == null) throw new HubException("Not Found User");
            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUsers(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved", new { username = sender.UserName, known = sender.KnownAs });
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {

                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDtos>(message));
            }



        }


        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }


        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed To Add To Group");

        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if (await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("Failed To Remove From Group");
        }
    }
}