using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.CelebrityService;
using Primeflix.Services.UserService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelebritiesController : ControllerBase
    {
        private readonly ICelebrityRepository _celebrityRepository;
        private readonly IUserRepository _userRepository;

        public CelebritiesController(
            ICelebrityRepository celebrityRepository,
            IUserRepository userRepository)
        {
            _celebrityRepository = celebrityRepository;
            _userRepository = userRepository;
        }

        //api/celebrities
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetCelebrities()
        {
            var celebrities = await _celebrityRepository.GetCelebrities();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebritiesDto = new List<CelebrityDto>();
            foreach (var celebrity in celebrities)
            {
                celebritiesDto.Add(new CelebrityDto
                {
                    Id = celebrity.Id,
                    FirstName = celebrity.FirstName,
                    LastName = celebrity.LastName
                });
            }
            return Ok(celebritiesDto);
        }

        //api/celebrities/celebrityId
        [HttpGet("{celebrityId}", Name = "GetCelebrity")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetCelebrity(int celebrityId)
        {
            if (!await _celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            var celebrity = await _celebrityRepository.GetCelebrity(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebrityDto = new CelebrityDto()
            {
                Id = celebrity.Id,
                FirstName = celebrity.FirstName,
                LastName = celebrity.LastName
            };

            return Ok(celebrityDto);
        }

        //api/celebrities
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCelebrity([FromBody] Celebrity celebrityToCreate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (celebrityToCreate == null)
                return BadRequest("No data was provided");

            if (await _celebrityRepository.CelebrityExists(celebrityToCreate.FirstName, celebrityToCreate.LastName))
                return StatusCode(409, $"Celebrity {celebrityToCreate.FirstName} {celebrityToCreate.LastName} already exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _celebrityRepository.CreateCelebrity(celebrityToCreate))
                return StatusCode(500, $"Something went wrong saving {celebrityToCreate.FirstName} {celebrityToCreate.LastName}");

            return StatusCode(201);
        }

        //api/celebrities/celebrityId
        [HttpPut("{celebrityId}")]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCelebrity(int celebrityId, [FromBody] Celebrity updatedCelebrity)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (updatedCelebrity == null)
                return BadRequest("No data was provided");

            if (celebrityId != updatedCelebrity.Id)
                return BadRequest("User IDs are not the same");

            if (!await _celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            if (await _celebrityRepository.IsDuplicate(updatedCelebrity.FirstName, updatedCelebrity.LastName))
                return StatusCode(422, $"Celebrity {updatedCelebrity.FirstName} {updatedCelebrity.LastName} already exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _celebrityRepository.UpdateCelebrity(updatedCelebrity))
                return StatusCode(500, $"Something went wrong update {updatedCelebrity.FirstName} {updatedCelebrity.LastName}");

            return NoContent();
        }

        //api/celebrities/celebrityId
        [HttpDelete("{celebrityId}")]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCelebrity(int celebrityId)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (!await _celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            var celebrityToDelete = await _celebrityRepository.GetCelebrity(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _celebrityRepository.DeleteCelebrity(celebrityToDelete))
                return StatusCode(500, $"Something went wrong deleting {celebrityToDelete.FirstName} {celebrityToDelete.LastName}");

            return NoContent();
        }
    }
}
