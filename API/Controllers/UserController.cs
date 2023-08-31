using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
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

        public IUserRepository _UserRepository;
        public IMapper _mapper { get; }

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _UserRepository.GetUserByUserNameAsync(username);
            if (user == null) return NotFound();
            _mapper.Map(memberupdatedto, user);
            if (await _UserRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed To Update User");

        }

    }
}