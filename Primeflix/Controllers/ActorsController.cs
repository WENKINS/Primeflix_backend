using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : Controller
    {
        private readonly DatabaseContext _context;

        public ActorsController(DatabaseContext context)
        {
            _context = context;
        }


        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            return await _context.Actor.Include(a => a.Celebrity).ToListAsync();
        }

    }
}
