using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Address;
using Primeflix.Services.Authentication;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IAuthentication _authentication;

        public AddressController(IAddressRepository addressRepository, IAuthentication authentication)
        {
            _addressRepository = addressRepository;
            _authentication = authentication;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressOfAUser()
        {
            var userId = await GetUserIdFromToken();

            if (userId == null || userId == 0)
                return BadRequest();

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
        public async Task<IActionResult> AddOrUpdateAddress(AddressDto addressDto)
        {
            var userId = await GetUserIdFromToken();

            if (userId == null || userId == 0)
                return BadRequest();

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
                {
                    ModelState.AddModelError("", "Something went wrong adding the user's address");
                    return StatusCode(500, ModelState);
                }

                var user = await _authentication.GetUser(userId);
                user.Address = newAddress;

                if (!(await _authentication.UpdateUser(user)))
                {
                    ModelState.AddModelError("", "Something went wrong adding the user's address");
                    return StatusCode(500, ModelState);
                }
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
                {
                    ModelState.AddModelError("", "Something went wrong updating the user's address");
                    return StatusCode(500, ModelState);
                }

                return StatusCode(204);
            }
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
