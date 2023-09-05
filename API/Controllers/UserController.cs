using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Controllers
{


    public class UserController : BaseApiController
    {

        public IUserRepository _UserRepository;
        public IMapper _mapper { get; }
        private readonly IPhotoService _photoService;

        public UserController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _UserRepository = userRepository;



        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDtos>>> getUsers()
        {

            return Ok(await _UserRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDtos>> getUserByUserName(string username)
        {
            return await _UserRepository.GetMemberByUserNameAsync(username);

        }
        [HttpPut]
        public async Task<ActionResult<MemberDtos>> updateUser(memberUpdateDto memberupdatedto)
        {

            var user = await _UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            _mapper.Map(memberupdatedto, user);
            if (await _UserRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed To Update User");

        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDtos>> addPhoto(IFormFile file)
        {
            var user = await _UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if (await _UserRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(getUserByUserName), new { username = user.UserName }, _mapper.Map<PhotoDtos>(photo));
            }
            return BadRequest("Problem Uploading Image");


        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {

            var user = await _UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("this is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _UserRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed To Set MainPhot");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Yo Cannot Delete Your Main Photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _UserRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed To Delete Photo");



        }
    }
}