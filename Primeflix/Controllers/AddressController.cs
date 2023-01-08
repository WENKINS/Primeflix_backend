using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Address;
using Primeflix.Services.UserService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;

        public AddressController(IAddressRepository addressRepository, IUserRepository userRepository)
        {
            _addressRepository = addressRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AddressDto>))]
        public async Task<IActionResult> GetAddressOfAUser()
        {
            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null || userId == 0)
                return BadRequest("User ID could not be retrieved");

            var address = await _addressRepository.GetAddressOfAUser(userId);
            if (address == null)
                return NotFound();

            AddressDto addressDto = new AddressDto()
            {
                Street = address.Street,
                Number = address.Number,
                PostalCode = address.PostalCode,
                City = address.City,
                Country = address.Country
            };

            return Ok(addressDto);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddOrUpdateAddress(AddressDto addressDto)
        {
            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null || userId == 0)
                return BadRequest("User ID could not be found");

            var address = await _addressRepository.GetAddressOfAUser(userId);

            if (address == null)
            {
                var newAddress = new Address()
                {
                    Street = addressDto.Street,
                    Number = addressDto.Number,
                    PostalCode = addressDto.PostalCode,
                    City = addressDto.City,
                    Country = addressDto.Country
                };

                if(!(await _addressRepository.CreateAddress(newAddress)))
                    return StatusCode(500, "Something went wrong adding the user's address");

                var user = await _userRepository.GetUser(userId);
                user.Address = newAddress;

                if (!(await _userRepository.UpdateUser(user)))
                    return StatusCode(500, "Something went wrong adding the user's address");

                return StatusCode(201);
            }

            else
            {
                address.Street = addressDto.Street;
                address.Number = addressDto.Number;
                address.PostalCode = addressDto.PostalCode;
                address.City = addressDto.City;
                address.Country = addressDto.Country;

                if (!(await _addressRepository.UpdateAddress(address)))
                    return StatusCode(500, "Something went wrong updating the user's address");

                return StatusCode(204);
            }
        }
    }
}
