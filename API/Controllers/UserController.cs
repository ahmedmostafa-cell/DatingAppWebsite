using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext ctx;

        public UserController(DataContext Ctx)
        {

            ctx = Ctx;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> getUsers()
        {
            return await ctx.Users.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> getUser(int id)
        {
            return await ctx.Users.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

    }
}