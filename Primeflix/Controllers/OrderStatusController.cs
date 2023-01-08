using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services.LanguageService;
using Primeflix.Services.OrderStatusService;
using Primeflix.Services.UserService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        
        public OrderStatusController(
            ILanguageRepository languageRepository,
            IUserRepository userRepository,
            IOrderStatusRepository orderStatusRepository
            )
        {
            _languageRepository = languageRepository;
            _userRepository = userRepository;
            _orderStatusRepository = orderStatusRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StatusDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStatuses([FromQuery]string lang)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

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
    }
}
