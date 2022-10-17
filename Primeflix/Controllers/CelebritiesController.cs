using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelebritiesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CelebritiesController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Celebrity>>> GetCelebrity()
        {
            return await _context.Celebrity.ToListAsync();
        }
    }
}
