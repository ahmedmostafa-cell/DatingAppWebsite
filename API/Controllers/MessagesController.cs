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


        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public MessagesController(IMapper mapper, IUnitOfWork uow)
        {

            _mapper = mapper;
            _uow = uow;

        }

        [HttpPost]
        public async Task<ActionResult<MessageDtos>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUserName) return BadRequest("You Can Not Send Message To Your Self");

            var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);


            if (recipient == null) return NotFound();
            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _uow.MssageRepository.AddMessage(message);
            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDtos>(message));

            return BadRequest("Faailed To Send Message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDtos>>> GetMessagesForUser([FromQuery] MessagesParams messagesParams)
        {
            messagesParams.UserName = User.GetUserName();
            var messages = await _uow.MssageRepository.GetMessagesForUsers(messagesParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrenPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName = User.GetUserName();
            var message = await _uow.MssageRepository.GetMessages(id);
            if (message.SenderUserName != userName && message.RecipientUserName != userName) return Unauthorized();
            if (message.SenderUserName == userName) message.SenderDeleted = true;
            if (message.RecipientUserName == userName) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MssageRepository.DeleteMessage(message);
            }
            if (await _uow.Complete()) return Ok();
            return BadRequest("problem in deleting message");
        }
    }
}