using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class MembersController(AppDbContext context) : BaseApiController
    {
        [HttpGet]
        // assigning the request type is get

        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var Members = await context.Users.ToListAsync();
            return Members;
        }
        [HttpGet("{id}")]
        // need to pass a url parameter 
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);

            if(member ==null) return NotFound();
            return member;
        }
  


    }
}
