using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext ctx;
        public ITokenService tokenservice { get; }

        public AccountController(DataContext Ctx, ITokenService Tokenservice)
        {
            tokenservice = Tokenservice;

            ctx = Ctx;

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
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    UserName = registerDTOs.UserName.ToLower(),
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password)),
                    passwordSalt = hmac.Key
                };
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
                return new UserDtos
                {
                    UserName = user.UserName,
                    token = tokenservice.createToken(user)

                };

            }


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDtos>> login(LoginDtoS loginDtos)
        {
            var user = await ctx.Users.SingleOrDefaultAsync(a => a.UserName == loginDtos.UserName);
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
                    token = tokenservice.createToken(user)

                };
            }
        }

        private async Task<bool> UserExists(string username)
        {

            return await ctx.Users.AnyAsync(a => a.UserName == username);

        }




    }
}