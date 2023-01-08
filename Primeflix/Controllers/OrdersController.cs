using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.LanguageService;
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
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly ILanguageRepository _languageRepository;

        public OrdersController(
            IOrderRepository orderRepository, 
            IUserRepository userRepository, 
            IProductRepository productRepository, 
            IOrderStatusRepository orderStatusRepository,
            ILanguageRepository languageRepository
            )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderStatusRepository = orderStatusRepository;
            _languageRepository = languageRepository;
        }

        [HttpGet()]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OrderDto>))]
        public async Task<IActionResult> GetOrders([FromQuery] string? lang)
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            var orders = new List<Order>();

            if(userRole == "admin")
            {
                orders = (List<Order>)await _orderRepository.GetOrders();
            }

            else
            {
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null)
                    return BadRequest("User ID could not be retrieved");

                orders = (List<Order>)await _orderRepository.GetOrdersOfAUser(userId);
            }

            var ordersDto = new List<OrderDto>();

            foreach(var order in orders)
            {
                var user = await _userRepository.GetUser(order.UserId);

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

                var status = await _orderStatusRepository.GetStatus(order.StatusId, lang);
                var statusDto = new StatusDto()
                {
                    Id = status.StatusId,
                    Name = status.Name
                };

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

        [HttpGet("orderId")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(OrderDto))]
        public async Task<IActionResult> GetOrder(int orderId, [FromQuery] string? lang)
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            var order = await _orderRepository.GetOrder(orderId);

            if (userRole != "admin")
            {
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null)
                    return BadRequest("User ID could not be retrieved");

                if (userId != order.UserId)
                    return StatusCode(401, "Order does not belong to user");
            }

            var user = await _userRepository.GetUser(order.UserId);

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

            foreach (var orderDetail in orderDetails)
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

            var status = await _orderStatusRepository.GetStatus(order.StatusId, lang);
            var statusDto = new StatusDto()
            {
                Id = status.StatusId,
                Name = status.Name
            };

            var orderDto = new OrderDto()
            {
                Id = order.Id,
                User = userDto,
                Date = order.Date,
                Total = order.Total,
                Details = orderDetailsDto
            };

            return Ok(orderDto);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OrderDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrdersOfAStatus([FromQuery]string status = "Paid", [FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            if (!(await _orderStatusRepository.StatusExists(status)))
                return BadRequest("Status doesn't exist");

            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            var orderStatus = _orderStatusRepository.GetStatus(status);

            var orders = new List<Order>();

            if (userRole == "admin")
            {
                orders = (List<Order>)await _orderStatusRepository.GetOrdersOfAStatus(orderStatus.Id);
            }

            else
            {
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null)
                    return BadRequest("User ID could not be retrieved");

                orders = (List<Order>)await _orderStatusRepository.GetOrdersOfAStatusAndUser(orderStatus.Id, userId);
            }

            var ordersDto = new List<OrderDto>();

            foreach (var order in orders)
            {
                var user = await _userRepository.GetUser(order.UserId);

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

                foreach (var orderDetail in orderDetails)
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
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (orderId != orderUpdate.OrderId)
                return BadRequest("Order IDs are not the same");

            if (!(await _orderRepository.OrderExists(orderId)))
                return NotFound();

            var order = await _orderRepository.GetOrder(orderId);

            if (!(await _orderStatusRepository.StatusExists(orderUpdate.Status)))
                return NotFound();

            var status = await _orderStatusRepository.GetStatus(orderUpdate.Status);

            if(status == null)
            {
                ModelState.AddModelError("", "Could not find status");
                return StatusCode(500, ModelState);
            }

            order.Status = status;

            if(!(await _orderRepository.UpdateOrder(order)))
            {
                ModelState.AddModelError("", "Something went wrong updating the order's status");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
