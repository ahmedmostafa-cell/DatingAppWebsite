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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        public IUserRepository _UserRepository;
        private readonly DataContext ctx;
        public ITokenService tokenservice { get; }
        public IMapper _mapper { get; }

        public AccountController(IUserRepository userRepository, DataContext Ctx, ITokenService Tokenservice, IMapper mapper)
        {
            _mapper = mapper;
            tokenservice = Tokenservice;

            ctx = Ctx;
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
                using var hmac = new HMACSHA512();

                user.UserName = registerDTOs.UserName.ToLower();
                user.passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password));
                user.passwordSalt = hmac.Key;

                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
                return new UserDtos
                {
                    UserName = user.UserName,
                    token = tokenservice.createToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs

                };

            }


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDtos>> login(LoginDtoS loginDtos)
        {
            // var user = await _UserRepository.GetUserByUserNameAsync(loginDtos.UserName);
            var user = await ctx.Users.Include(p => p.Photos).SingleOrDefaultAsync(a => a.UserName == loginDtos.UserName);
            if (user == null)
            {
                return Unauthorized("invalid username");
            }
            else
            {
                using var hmac = new HMACSHA512(user.passwordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.passwordHash[i]) return Unauthorized("invalid password");
                }
                return new UserDtos
                {
                    UserName = user.UserName,
                    token = tokenservice.createToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs

                };
            }
        }

        private async Task<bool> UserExists(string username)
        {


            return await ctx.Users.AnyAsync(a => a.UserName == username.ToLower());

        }




    }
}