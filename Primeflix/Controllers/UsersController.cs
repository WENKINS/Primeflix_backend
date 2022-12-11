using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private IAuthentication _authentication;
        public UsersController(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        //api/users/register
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto newUser)
        {
            if (newUser == null)
                return BadRequest(ModelState);

            if (await _authentication.UserExists(newUser.Email))
            {
                ModelState.AddModelError("", $"A user with email address {newUser.Email} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User()
            {
                Email = newUser.Email,
                Password = newUser.Password
            };

            if (!await _authentication.Register(user))
            {
                ModelState.AddModelError("", $"Something went wrong saving user with address {newUser.Email}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetUser", new { userId = user.Id }, user);
        }

        //api/users/userId
        [HttpGet("{userId}", Name = "GetUser")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (!await _authentication.UserExists(userId))
                return NotFound();

            var user = await _authentication.GetUser(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Language = user.Language
            };

            return Ok(userDto);
        }


    }
}
