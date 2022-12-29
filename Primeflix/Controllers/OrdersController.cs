using Microsoft.AspNetCore.Mvc;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private ICartRepository _cartRepository;
        private IAuthentication _authentication;

        public OrdersController(IOrderRepository orderRepository, ICartRepository cartRepository, IAuthentication authentication)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _authentication = authentication;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = await GetUserIdFromToken();
            var cart = await _cartRepository.GetCartOfAUser(userId);
            var result = await _orderRepository.PlaceOrder(cart.Id);

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
