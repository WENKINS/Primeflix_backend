using Microsoft.AspNetCore.Mvc;
using Primeflix.Services.UserService;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;

        public PaymentController(
            IPaymentRepository paymentRepository, 
            IUserRepository userRepository, 
            ICartRepository cartRepository
            )
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _cartRepository = cartRepository;
        }

        [HttpPost("checkout")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> CreateCheckoutSession()
        {
            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null)
                return BadRequest("User ID could not be retrieved");

            var cartId = (await _cartRepository.GetCartOfAUser(userId)).Id;

            var session = await _paymentRepository.CreateCheckoutSession(cartId);

            return Ok(session.Url);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<bool>> FulfillOrder()
        {
            var response = await _paymentRepository.FulfillOrder(Request);
            if (!response)
                return BadRequest();

            return Ok();
        }
    }
}
