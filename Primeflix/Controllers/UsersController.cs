using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.LanguageService;
using Primeflix.Services.RoleService;

namespace Primeflix.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private IAuthentication _authentication;
        private ILanguageRepository _languageRepository;
        private ICartRepository _cartRepository;
        private IRoleRepository _roleRepository;

        public UsersController(IAuthentication authentication, ILanguageRepository languageRepository, ICartRepository cartRepository, IRoleRepository roleRepository)
        {
            _authentication = authentication;
            _languageRepository = languageRepository;
            _cartRepository = cartRepository;
            _roleRepository = roleRepository;
        }

        //api/users/register
        [AllowAnonymous]
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
                Phone = newUser.Phone,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName
            };

            if (!await _authentication.Register(user, newUser.Password))
            {
                ModelState.AddModelError("", $"Something went wrong saving user with address {newUser.Email}");
                return StatusCode(500, ModelState);
            }

            var userId = (await _authentication.GetUser(user.Email)).Id;

            if(!await _cartRepository.CreateCart(userId))
            {
                ModelState.AddModelError("", $"Something went wrong creating user's cart");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetUser", new { userId = user.Id }, user);
        }


        //api/users/login
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            if (userLogin == null)
                return BadRequest(ModelState);

            if (!await _authentication.UserExists(userLogin.Email))
            {
                ModelState.AddModelError("", $"User {userLogin.Email} doesn't exist"); // SECURITY RISK!
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authentication.Login(userLogin.Email, userLogin.Password);

            if (response == null)
            {
                ModelState.AddModelError("", "Wrong password");
                return StatusCode(401, ModelState);
            }

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
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (userRole.Equals("admin"))
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

            else
            {
                var userId = await GetUserIdFromToken();

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


        }

        //api/users/userId
        [HttpGet("{userId}", Name = "GetUser")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUser(int userId)
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (userRole.Equals("admin"))
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

            ModelState.AddModelError("", "User is not an admin");
            return StatusCode(401, ModelState);
        }

        //api/users/userId
        [Authorize]
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (userRole.Equals("admin"))
            {
                if (!await _authentication.UserExists(userId))
                    return NotFound();

                var userToDelete = await _authentication.GetUser(userId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await _authentication.DeleteUser(userToDelete))
                {
                    ModelState.AddModelError("", $"Something went wrong deleting {userToDelete.FirstName} {userToDelete.LastName}");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }

            ModelState.AddModelError("", "User is not an admin");
            return StatusCode(401, ModelState);
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
            var userId = await GetUserIdFromToken();

            if (userId == null)
                return BadRequest();

            if (userId != userToUpdate.Id)
                return BadRequest();

            if (!await _authentication.UserExists(userId))
                return NotFound();

            var user = await _authentication.GetUser(userToUpdate.Id);

            user.FirstName = userToUpdate.FirstName;
            user.LastName = userToUpdate.LastName;
            user.Phone = userToUpdate.Phone;
            if(!(await _languageRepository.LanguageExists(userToUpdate.LanguageId)))
            {
                ModelState.AddModelError("", $"The language doesn't exist");
                return StatusCode(500, ModelState);
            }
            user.Language = await _languageRepository.GetLanguage(userToUpdate.LanguageId);
            user.Email = userToUpdate.Email;

            if (!await _authentication.UpdateUser(user))
            {
                ModelState.AddModelError("", $"Something went wrong updating the user {userToUpdate.FirstName} {userToUpdate.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/users/userId
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] User userToUpdate)
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (userRole.Equals("admin"))
            {
                if (userId != userToUpdate.Id)
                    return BadRequest();

                if (!await _authentication.UserExists(userId))
                    return NotFound();

                var user = await _authentication.GetUser(userToUpdate.Id);

                user.FirstName = userToUpdate.FirstName;
                user.LastName = userToUpdate.LastName;
                user.Phone = userToUpdate.Phone;
                if (!(await _roleRepository.RoleExists(userToUpdate.RoleId)))
                {
                    ModelState.AddModelError("", $"The role doesn't exist");
                    return StatusCode(500, ModelState);
                }
                user.Role = await _roleRepository.GetRole(userToUpdate.RoleId);
                if (!(await _languageRepository.LanguageExists(userToUpdate.LanguageId)))
                {
                    ModelState.AddModelError("", $"The language doesn't exist");
                    return StatusCode(500, ModelState);
                }
                user.Language = await _languageRepository.GetLanguage(userToUpdate.LanguageId);
                user.Email = userToUpdate.Email;

                if (!await _authentication.UpdateUser(user))
                {
                    ModelState.AddModelError("", $"Something went wrong updating the user {userToUpdate.FirstName} {userToUpdate.LastName}");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }


            ModelState.AddModelError("", "User is not an admin");
            return StatusCode(401, ModelState);
        }

        public async Task<int> GetUserIdFromToken()
        {
            string bearerToken = HttpContext.Request.Headers["Authorization"];

            if (String.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                return 0;
            }

            string token = bearerToken.Substring("Bearer ".Length);
            int userId = Int32.Parse(await _authentication.DecodeTokenForId(token));

            return userId;
        }

        public async Task<string> GetUserRoleFromToken()
        {
            string bearerToken = HttpContext.Request.Headers["Authorization"];

            if (String.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                return null;
            }

            string token = bearerToken.Substring("Bearer ".Length);
            string role = await _authentication.DecodeTokenForRole(token);

            return role;
        }


    }
}
