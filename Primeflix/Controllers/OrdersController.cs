using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.ProductService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private ICartRepository _cartRepository;
        private IAuthentication _authentication;
        private IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, ICartRepository cartRepository, IAuthentication authentication, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _authentication = authentication;
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            var userId = await GetUserIdFromToken();
            var cart = await _cartRepository.GetCartOfAUser(userId);
            await _orderRepository.PlaceOrder(cart.Id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            var orders = new List<Order>();

            if(userRole == "admin")
            {
                orders = (List<Order>)await _orderRepository.GetOrders();
            }

            else
            {
                var userId = await GetUserIdFromToken();
                orders = (List<Order>)await _orderRepository.GetOrdersOfAUser(userId);
            }

            var ordersDto = new List<OrderDto>();

            foreach(var order in orders)
            {
                var user = await _authentication.GetUser(order.UserId);

                var userDto = new UserWithoutPasswordDto()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email
                };

                var orderDetails = await _orderRepository.GetOrderDetails(order.Id);
                var orderDetailsDto = new List<OrderDetailsDto>();

                foreach(var orderDetail in orderDetails)
                {
                    var product = await _productRepository.GetProduct(orderDetail.ProductId);
                    var productDto = new ProductLessDetailsDto()
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Price = product.Price
                    };

                    orderDetailsDto.Add(new OrderDetailsDto
                    {
                        Product = productDto,
                        Quantity = orderDetail.Quantity
                    });
                }

                ordersDto.Add(new OrderDto
                {
                    Id = order.Id,
                    User = userDto,
                    Date = order.Date,
                    Total = order.Total,
                    Details = orderDetailsDto
                });
            }

            return Ok(ordersDto);
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
