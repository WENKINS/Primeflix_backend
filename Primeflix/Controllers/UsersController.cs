using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;
using Primeflix.Services.LanguageService;

namespace Primeflix.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private IAuthentication _authentication;
        private ILanguageRepository _languageRepository;
        public UsersController(IAuthentication authentication, ILanguageRepository languageRepository)
        {
            _authentication = authentication;
            _languageRepository = languageRepository;
        }

        //api/users/register
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


        //api/users/login
        [ProducesResponseType(201, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            if (userLogin == null)
                return BadRequest(ModelState);

            if (!await _authentication.UserExists(userLogin.Email))
            {
                ModelState.AddModelError("", $"User {userLogin.Email} doesn't exists"); // SECURITY RISK!
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authentication.Login(userLogin.Email, userLogin.Password);

            return Ok(response);
        }

        //api/users
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authentication.GetUsers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                var language = await _languageRepository.GetLanguageOfAUser(user.Id);
                var languageDto = new LanguageDto()
                {
                    Id = language.Id,
                    Name = language.Name,
                    Code = language.Code
                };

                usersDto.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Password = user.Password,
                    Language = languageDto
                });
            }
            return Ok(usersDto);
        }

        //api/users/userId
        [HttpGet("{userId}", Name = "GetUser")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (!await _authentication.UserExists(userId))
                return NotFound();

            var user = await _authentication.GetUser(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var language = await _languageRepository.GetLanguageOfAUser(user.Id);
            var languageDto = new LanguageDto()
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code
            };

            var userDto = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Language = languageDto
            };

            return Ok(userDto);
        }


    }
}
