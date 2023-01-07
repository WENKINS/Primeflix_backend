using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services.Authentication;
using Primeflix.Services.LanguageService;
using Primeflix.Services.OrderStatusService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private ILanguageRepository _languageRepository;
        private IAuthentication _authentication;
        private IOrderStatusRepository _orderStatusRepository;
        
        public OrderStatusController(
            ILanguageRepository languageRepository,
            IAuthentication authentication,
            IOrderStatusRepository orderStatusRepository
            )
        {
            _languageRepository = languageRepository;
            _authentication = authentication;
            _orderStatusRepository = orderStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatuses([FromQuery]string lang)
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

            var language = await _languageRepository.GetLanguage(lang);

            var statuses = await _orderStatusRepository.GetOrderStatuses(language.Id);

            if (!ModelState.IsValid)
                return BadRequest();

            var statusesDto = new List<StatusDto>();

            foreach(var status in statuses)
            {
                statusesDto.Add(new StatusDto{
                    Id = status.StatusId,
                    Name = status.Name
                });
            }

            return Ok(statusesDto);
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
