using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.LanguageService;
using Primeflix.Services.OrderService;
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
        private readonly IOrderRepository _orderRepository;
        
        public OrderStatusController(
            ILanguageRepository languageRepository,
            IUserRepository userRepository,
            IOrderStatusRepository orderStatusRepository,
            IOrderRepository orderRepository
            )
        {
            _languageRepository = languageRepository;
            _userRepository = userRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StatusDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStatuses([FromQuery]string? lang = "en")
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (!(await _languageRepository.LanguageExists(lang)))
                return StatusCode(500, "Language doesn't exist");

            var statuses = await _orderStatusRepository.GetOrderStatuses(lang);

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
