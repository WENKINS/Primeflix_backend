using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.UserService;
using Primeflix.Services.CartService;
using Primeflix.Services.LanguageService;
using Primeflix.Services.RoleService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IRoleRepository _roleRepository;

        public UsersController(
            IUserRepository userRepository, 
            ILanguageRepository languageRepository, 
            ICartRepository cartRepository, 
            IRoleRepository roleRepository
            )
        {
            _userRepository = userRepository;
            _languageRepository = languageRepository;
            _cartRepository = cartRepository;
            _roleRepository = roleRepository;
        }

        //api/users/register
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register(UserRegisterDto newUser)
        {
            if (newUser == null)
                return BadRequest("No data was provided");

            if (await _userRepository.UserExists(newUser.Email))
                return StatusCode(409, $"A user with email address {newUser.Email} already exists");

            var user = new User()
            {
                Email = newUser.Email,
                Phone = newUser.Phone,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName
            };

            if (!await _userRepository.Register(user, newUser.Password))
                return StatusCode(500, $"Something went wrong saving user with address {newUser.Email}");

            var userId = (await _userRepository.GetUser(user.Email)).Id;

            if (!await _cartRepository.CreateCart(userId))
                return StatusCode(500, $"Something went wrong creating user's cart");

            return StatusCode(201);
        }


        //api/users/login
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            if (userLogin == null)
                return BadRequest("No data was provided");

            if (!await _userRepository.UserExists(userLogin.Email))
                return NotFound();

            var response = await _userRepository.Login(userLogin.Email, userLogin.Password);

            if (response == null)
                return StatusCode(401, "Wrong password");

            return Ok(response);
        }

        //api/users/login/facebook
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [HttpPost("login/facebook")]
        public async Task<IActionResult> Login([FromBody]string token)
        {
            if (String.IsNullOrEmpty(token))
                return BadRequest("No token provided");

            var response = await _userRepository.LoginWithFacebook(token);

            if (response.Equals("Invalid token") || String.IsNullOrEmpty(response))
                return StatusCode(401, "Wrong access token provided");

            return Ok(response);
        }

        //api/users
        [HttpGet]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetUsers()
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User Role could not be retrieved");

            var users = new List<User>();

            if (userRole.Equals("admin"))
            {
                users = (List<User>)await _userRepository.GetUsers();
            }

            else
            {
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null)
                    return BadRequest("User ID could not be retrieved");

                users.Add(await _userRepository.GetUser(userId));
            }

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

                var role = await _roleRepository.GetRoleOfAUser(user.Id);
                var roleDto = new RoleDto()
                {
                    Id = role.Id,
                    Name = role.Name
                };

                usersDto.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Language = languageDto,
                    Role = roleDto
                });
            }

            return Ok(usersDto);
        }

        //api/users/userId
        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUser(int userId)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (!await _userRepository.UserExists(userId))
                return NotFound();

            var user = await _userRepository.GetUser(userId);

            var language = await _languageRepository.GetLanguageOfAUser(user.Id);
            var languageDto = new LanguageDto()
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code
            };

            var role = await _roleRepository.GetRoleOfAUser(user.Id);
            var roleDto = new RoleDto()
            {
                Id = role.Id,
                Name = role.Name
            };

            var userDto = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Language = languageDto,
                Role = roleDto
            };

            return Ok(userDto);
        }

        //api/users/userId
        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (!await _userRepository.UserExists(userId))
                return NotFound();

            var userToDelete = await _userRepository.GetUser(userId);

            if (!await _userRepository.DeleteUser(userToDelete))
                return StatusCode(500, $"Something went wrong deleting {userToDelete.FirstName} {userToDelete.LastName}");

            return NoContent();
        }

        //api/users
        [Authorize]
        [HttpPut]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] User userToUpdate)
        {
            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null)
                return BadRequest("User ID could not be retrieved");

            if (userId != userToUpdate.Id)
                return BadRequest("User IDs are not the same");

            if (!await _userRepository.UserExists(userId))
                return NotFound();

            var user = await _userRepository.GetUser(userToUpdate.Id);

            if (!(await _languageRepository.LanguageExists(userToUpdate.LanguageId)))
                return BadRequest("Language doesn't exist");

            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Phone = userToUpdate.Phone;
            user.Language = await _languageRepository.GetLanguage(userToUpdate.LanguageId);
            user.Email = userToUpdate.Email;

            if (!await _userRepository.UpdateUser(user))
                return StatusCode(500, $"Something went wrong updating the user {userToUpdate.FirstName} {userToUpdate.LastName}");

            return NoContent();
        }

        //api/users/userId
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] User userToUpdate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (userId != userToUpdate.Id)
                return BadRequest("Uder IDs are not the same");

            if (!await _userRepository.UserExists(userId))
                return NotFound();

            var user = await _userRepository.GetUser(userToUpdate.Id);

            if (!(await _roleRepository.RoleExists(userToUpdate.RoleId)))
                return BadRequest("Role doesn't exist");

            if (!(await _languageRepository.LanguageExists(userToUpdate.LanguageId)))
                return BadRequest("Language doesn't exist");

            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Phone = userToUpdate.Phone;
            user.Role = await _roleRepository.GetRole(userToUpdate.RoleId);
            user.Language = await _languageRepository.GetLanguage(userToUpdate.LanguageId);
            user.Email = userToUpdate.Email;

            if (!await _userRepository.UpdateUser(user))
                return StatusCode(500, $"Something went wrong updating the user {userToUpdate.FirstName} {userToUpdate.LastName}");

            return NoContent();
        }
    }
}
