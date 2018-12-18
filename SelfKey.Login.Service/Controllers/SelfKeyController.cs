using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfKey.Login.Api;
using SelfKey.Login.Data.Models;
using SelfKey.Login.Service.DbContexts;

namespace SelfKey.Login.Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SelfKeyController : Controller
    {
        private readonly UserDbContext _context;

        public SelfKeyController(UserDbContext context) => _context = context;

        [HttpGet]
        public ActionResult<List<User>> GetAll() => _context.Users.ToList();

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> GetById(long id) => _context.Users.Find(id) ?? (ActionResult<User>)NotFound();

        [HttpPost]
        public IActionResult Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtRoute("GetUser", new { user.Id }, user);
        }

        [HttpPost("verify"), AllowAnonymous]
        public IActionResult Verify([FromBody]User user)
        {
            return Authenticator.Verify(user) ? (IActionResult)Ok() : BadRequest();
        }

        public IActionResult Protected()
        {
            return Ok();
        }
    }
}
