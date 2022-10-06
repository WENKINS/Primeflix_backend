using Microsoft.AspNetCore.Mvc;
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
        public List<Celebrity> GetCelebrities()
        {
            var data = _context.Celebrity.ToList();
            return data;
        }
    }
}
