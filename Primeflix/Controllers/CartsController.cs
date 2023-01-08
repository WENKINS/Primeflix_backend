using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.UserService;
using Primeflix.Services.CartService;
using Primeflix.Services.ProductService;
using Microsoft.AspNetCore.Authorization;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public CartsController(
            ICartRepository cartRepository, 
            IUserRepository userRepository, 
            IProductRepository productRepository
            )
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        //api/carts
        [HttpGet(Name = "GetCart")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CartDto>))]
        public async Task<IActionResult> GetCart()
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            var carts = new List<Cart>();

            if (userRole.Equals("admin"))
            {
                carts = (List<Cart>)await _cartRepository.GetCarts();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
            }

            else
            {
                // If user not an admin, they get to see their own cart only
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null || userId == 0)
                    return BadRequest();

                if (!await _cartRepository.CartOfAUserExists(userId))
                    return NotFound();

                carts.Add(await _cartRepository.GetCartOfAUser(userId));
            }

            var cartsDto = new List<CartDto>();

            foreach (var cart in carts)
            {
                var user = await _userRepository.GetUser(cart.UserId);

                var userDto = new UserLessDetailsDto()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                };

                var products = await _cartRepository.GetProductsOfACart(cart.Id);
                var productsDto = new List<CartProductWithTitleDto>();

                foreach (var product in products)
                {
                    var productInfo = await _productRepository.GetProduct(product.ProductId);

                    productsDto.Add(new CartProductWithTitleDto
                    {
                        Id = productInfo.Id,
                        Title = productInfo.Title,
                        Price = productInfo.Price,
                        Quantity = product.Quantity
                    });
                }

                cartsDto.Add(new CartDto
                {
                    Id = cart.Id,
                    User = userDto,
                    Products = productsDto
                });
            }

            return Ok(cartsDto);
        }

        //api/carts
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201, Type = typeof(Cart))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCart([FromBody] IEnumerable<CartProductDto> products)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (products == null)
                return BadRequest("No product was provided to add to the cart");

            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null || userId == 0)
                return BadRequest("User ID could not be retrieved");

            if (!(await _cartRepository.CartExists(userId)))
                await _cartRepository.CreateCart(userId);

            if (!(await _cartRepository.EmptyCart(userId))) // We empty the cart first, before adding all the products
                return StatusCode(500, "Something went wrong adding the product(s) to the cart");

            foreach (var product in products)
            {
                if (!await _productRepository.ProductExists(product.ProductId))
                    return StatusCode(404, "Product doesn't exist");

                if (!await _cartRepository.AddProductToCart(userId, product.ProductId, product.Quantity))
                    return StatusCode(500, "Something went wrong adding the product(s) to the cart");
            }

            return await GetCart();
        }
    }
}
