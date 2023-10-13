using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        public IUserRepository _UserRepository;
        private readonly UserManager<AppUser> _userManager;
        public ITokenService tokenservice { get; }
        public IMapper _mapper { get; }

        public AccountController(IUserRepository userRepository, UserManager<AppUser> userManager, ITokenService Tokenservice, IMapper mapper)
        {
            _mapper = mapper;
            tokenservice = Tokenservice;

            _userManager = userManager;
            _UserRepository = userRepository;

        }


        [HttpPost("register")] //api/Account/register
        public async Task<ActionResult<UserDtos>> getUser(RegisterDTOs registerDTOs)
        {
            if (await UserExists(registerDTOs.UserName))
            {
                return BadRequest("UserName is taken");
            }
            else
            {
                var user = _mapper.Map<AppUser>(registerDTOs);


                user.UserName = registerDTOs.UserName.ToLower();


                var result = await _userManager.CreateAsync(user, registerDTOs.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);
                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleResult.Succeeded) return BadRequest(result.Errors);
                return new UserDtos
                {
                    UserName = user.UserName,
                    token = await tokenservice.createToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs,
                    Gender = user.Gender

                };

            }


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDtos>> login(LoginDtoS loginDtos)
        {
            // var user = await _UserRepository.GetUserByUserNameAsync(loginDtos.UserName);
            var user = await _userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(a => a.UserName == loginDtos.UserName);
            if (user == null)
            {
                return Unauthorized("invalid username");
            }
            else
            {
                var result = await _userManager.CheckPasswordAsync(user, loginDtos.Password);
                if (!result) return Unauthorized();
                return new UserDtos
                {
                    UserName = user.UserName,
                    token = await tokenservice.createToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs,
                    Gender = user.Gender

                };
            }
        }

        private async Task<bool> UserExists(string username)
        {


            return await _userManager.Users.AnyAsync(a => a.UserName == username.ToLower());

        }




    }
}