using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SelfKey.Login.Api;
using SelfKey.Login.Data.Models;

namespace SelfKey.Login.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelfKeyController : Controller
    {
        private readonly SelfKeyContext _context;

        public SelfKeyController(SelfKeyContext context) => _context = context;

        [HttpGet]
        public ActionResult<List<Payload>> GetAll() => _context.Payloads.ToList();

        [HttpGet("{id}", Name = "GetPayload")]
        public ActionResult<Payload> GetById(long id) => _context.Payloads.Find(id) ?? (ActionResult<Payload>)NotFound();

        [HttpPost]
        public IActionResult Create(Payload payload)
        {
            _context.Payloads.Add(payload);
            _context.SaveChanges();

            return CreatedAtRoute("GetPayload", new { payload.Id }, payload);
        }

        [HttpPost("{payload, privateKey}")]
        public ActionResult<Payload> Sign(Payload payload, string privateKey)
        {
            payload.Proof.Signature = Signer.Sign(payload.Proof.Nonce, privateKey);
            return payload;
        }

        [HttpPost("verify")]
        public IActionResult Verify(Payload payload)
        {
            return Signer.Verify(payload.Proof.Nonce, payload.Proof.Signature, payload.Proof.Address) ? (IActionResult)Ok() : BadRequest();
        }
    }
}
