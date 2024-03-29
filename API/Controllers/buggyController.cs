
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class buggyController : BaseApiController
    {
        private readonly DataContext _context;

        public buggyController(DataContext context)
        {

            _context = context;



        }

        [Authorize]
        [HttpGet("auth")]

        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]

        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null)
            {
                return NotFound();
            }
            else
            {
                return thing;
            }
        }


        [HttpGet("server-error")]

        public ActionResult<string> GetServerError()
        {

            var thing = _context.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;


        }


        [HttpGet("bad-request")]

        public ActionResult<string> GetBadRequest()
        {

            return BadRequest("this is not a good request");

        }

    }
}