using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.OrderStatusService;
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
        private IOrderStatusRepository _orderStatusRepository;

        public OrdersController(IOrderRepository orderRepository, ICartRepository cartRepository, IAuthentication authentication, IProductRepository productRepository, IOrderStatusRepository orderStatusRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _authentication = authentication;
            _productRepository = productRepository;
            _orderStatusRepository = orderStatusRepository;
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

        [HttpPut]
        [Authorize]
        [HttpPut("{orderId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderStatusUpdateDto orderUpdate)
        {
            var userRole = await GetUserRoleFromToken();

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (orderId == null)
                return BadRequest();

            if (orderId != orderUpdate.OrderId)
                return BadRequest();

            var order = await _orderRepository.GetOrder(orderId);

            var status = await _orderStatusRepository.GetStatus(orderUpdate.Status);

            if(status == null)
            {
                ModelState.AddModelError("", "Could not find status");
                return StatusCode(500, ModelState);
            }

            order.Status = status;

            if(!(await _orderRepository.UpdateOrder(order)))
            {
                ModelState.AddModelError("", "Somethign went wrong updating the order's status");
                return StatusCode(500, ModelState);
            }

            return NoContent();
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
