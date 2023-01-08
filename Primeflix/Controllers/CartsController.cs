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

        public CartsController(ICartRepository cartRepository, IUserRepository userRepository, IProductRepository productRepository)
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
                return BadRequest();

            if (userRole.Equals("admin"))
            {
                var carts = await _cartRepository.GetCarts();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cartsDto = new List<CartDto>();

                foreach (var cart in carts)
                {
                    var user = await _userRepository.GetUser(cart.UserId);

                    var userDto = new UserWithoutPasswordDto()
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

            else
            {
                var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

                if (userId == null || userId == 0)
                    return BadRequest();

                if (!await _cartRepository.CartOfAUserExists(userId))
                    return NotFound();

                var cart = await _cartRepository.GetCartOfAUser(userId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userRepository.GetUser(userId);

                var userDto = new UserWithoutPasswordDto()
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

                var cartDto = new CartDto()
                {
                    Id = cart.Id,
                    User = userDto,
                    Products = productsDto
                };

                return Ok(cartDto);
            }
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
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (products == null)
                return BadRequest();

            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null || userId == 0)
                return BadRequest();

            if (!(await _cartRepository.CartExists(userId)))
                await _cartRepository.CreateCart(userId);

            if(!(await _cartRepository.EmptyCart(userId)))
            {
                ModelState.AddModelError("", $"Something went wrong adding product to cart");
                return StatusCode(500, ModelState);
            }

            foreach (var product in products)
            {
                if(!await _productRepository.ProductExists(product.ProductId))
                {
                    ModelState.AddModelError("", $"Product {product.ProductId} does not exist");
                    return StatusCode(404, ModelState);
                }
                if (!await _cartRepository.AddProductToCart(userId, product.ProductId, product.Quantity))
                {
                    ModelState.AddModelError("", $"Something went wrong adding product to cart");
                    return StatusCode(500, ModelState);
                }
            }

            return await GetCart();
        }

        //api/carts
        [HttpDelete()]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCart()
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            var userId = await _userRepository.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userId == null || userId == 0)
                return BadRequest();

            if (!await _cartRepository.CartOfAUserExists(userId))
                return NotFound();

            var cartToDelete = await _cartRepository.GetCartOfAUser(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _cartRepository.DeleteCart(cartToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the cart");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
