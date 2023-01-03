using Microsoft.AspNetCore.Mvc;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.PaymentService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAuthentication _authentication;
        private readonly ICartRepository _cartRepository;
        public PaymentController(IPaymentRepository paymentRepository, IAuthentication authentication, ICartRepository cartRepository)
        {
            _paymentRepository = paymentRepository;
            _authentication = authentication;
            _cartRepository = cartRepository;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<string>> CreateCheckoutSession()
        {
            var userId = await GetUserIdFromToken();
            var cartId = (await _cartRepository.GetCartOfAUser(userId)).Id;
            var session = await _paymentRepository.CreateCheckoutSession(cartId);
            return Ok(session.Url);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> FulfillOrder()
        {
            var response = await _paymentRepository.FulfillOrder(Request);
            if (!response)
                return BadRequest();
            return Ok();
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
