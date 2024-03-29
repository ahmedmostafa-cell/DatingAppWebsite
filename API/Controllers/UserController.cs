using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Controllers
{

    [Authorize]
    public class UserController : BaseApiController
    {


        public IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _uow;

        public UserController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _uow = uow;




        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDtos>>> getUsers([FromQuery] UserParams userParams)
        {
            var gender = await _uow.UserRepository.GetUserGender(User.GetUserName());
            userParams.CurrentUserName = User.GetUserName();
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }
            var users = await _uow.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrenPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }

        [HttpGet("{username}", Name = "getUserByUserName")]
        public async Task<ActionResult<MemberDtos>> getUserByUserName(string username)
        {
            var currentUsername = User.GetUserName();
            return await _uow.UserRepository.GetMemberAsync(username,
            isCurrentUser: currentUsername == username
            );

        }
        [HttpPut]
        public async Task<ActionResult<MemberDtos>> updateUser(memberUpdateDto memberupdatedto)
        {

            var user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            _mapper.Map(memberupdatedto, user);
            if (await _uow.Complete()) return NoContent();
            return BadRequest("Failed To Update User");

        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDtos>> addPhoto(IFormFile file)
        {
            var user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);
            if (await _uow.Complete())
            {

                return CreatedAtRoute("getUserByUserName", new { username = user.UserName }, _mapper.Map<PhotoDtos>(photo));
            }
            return BadRequest("Problem Uploading Image");


        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {

            var user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("this is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _uow.Complete()) return NoContent();
            return BadRequest("Failed To Set MainPhot");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Yo Cannot Delete Your Main Photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _uow.Complete()) return Ok();
            return BadRequest("Failed To Delete Photo");



        }




    }
}