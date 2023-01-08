using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.OrderStatusService;
using Primeflix.Services.ProductService;
using Primeflix.Services.UserService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _authentication;
        private readonly IProductRepository _productRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;

        public OrdersController(
            IOrderRepository orderRepository, 
            IUserRepository authentication, 
            IProductRepository productRepository, 
            IOrderStatusRepository orderStatusRepository
            )
        {
            _orderRepository = orderRepository;
            _authentication = authentication;
            _productRepository = productRepository;
            _orderStatusRepository = orderStatusRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OrderDto>))]
        public async Task<IActionResult> GetOrders()
        {
            var userRole = await _authentication.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            var orders = new List<Order>();

            if(userRole == "admin")
            {
                orders = (List<Order>)await _orderRepository.GetOrders();
            }

            else
            {
                var userId = await _authentication.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);
                orders = (List<Order>)await _orderRepository.GetOrdersOfAUser(userId);
            }

            var ordersDto = new List<OrderDto>();

            foreach(var order in orders)
            {
                var user = await _authentication.GetUser(order.UserId);

                var userDto = new UserLessDetailsDto()
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
            var userRole = await _authentication.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

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
    }
}
