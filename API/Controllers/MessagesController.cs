using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {

        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<ActionResult<MessageDtos>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUserName) return BadRequest("You Can Not Send Message To Your Self");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);


            if (recipient == null) return NotFound();
            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDtos>(message));

            return BadRequest("Faailed To Send Message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDtos>>> GetMessagesForUser([FromQuery] MessagesParams messagesParams)
        {
            messagesParams.UserName = User.GetUserName();
            var messages = await _messageRepository.GetMessagesForUsers(messagesParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrenPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDtos>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName = User.GetUserName();
            var message = await _messageRepository.GetMessages(id);
            if (message.SenderUserName != userName && message.RecipientUserName != userName) return Unauthorized();
            if (message.SenderUserName == userName) message.SenderDeleted = true;
            if (message.RecipientUserName == userName) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }
            if (await _messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("problem in deleting message");
        }
    }
}