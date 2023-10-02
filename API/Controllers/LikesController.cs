using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikedRepository _likedRepository;
        public LikesController(IUserRepository userRepository, ILikedRepository likedRepository)
        {
            _likedRepository = likedRepository;
            _userRepository = userRepository;

        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _likedRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You Can NoT lIKE YourSelf");
            var userlike = await _likedRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userlike != null) return BadRequest("You Already Lie This User Before");

            userlike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userlike);
            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed To Like User");

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDtos>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {

            likesParams.UserId = User.GetUserId();

            var users = await _likedRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrenPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

    }
}