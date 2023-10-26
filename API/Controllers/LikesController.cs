using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LikesController(IUnitOfWork uow, IMapper mapper)
        {

            _uow = uow;
            _mapper = mapper;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _uow.UserRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _uow.LikedRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You Can NoT lIKE YourSelf");
            var userlike = await _uow.LikedRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userlike != null) return BadRequest("You Already Lie This User Before");

            userlike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userlike);
            if (await _uow.Complete()) return Ok();

            return BadRequest("Failed To Like User");

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDtos>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {

            likesParams.UserId = User.GetUserId();

            var users = await _uow.LikedRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrenPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

    }
}