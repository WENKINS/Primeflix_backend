using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.ProductService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private ICartRepository _cartRepository;
        private IAuthentication _authentication;
        private IProductRepository _productRepository;

        public CartsController(ICartRepository cartRepository, IAuthentication authentication, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _authentication = authentication;
            _productRepository = productRepository;
        }

        //api/carts
        [HttpGet(Name = "GetCart")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CartDto>))]
        public async Task<IActionResult> GetCart()
        {
            var userRole = await GetUserRoleFromToken();

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
                    var user = await _authentication.GetUser(cart.UserId);

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
                var userId = await GetUserIdFromToken();

                if (userId == null || userId == 0)
                    return BadRequest();

                if (!await _cartRepository.CartOfAUserExists(userId))
                    return NotFound();

                var cart = await _cartRepository.GetCartOfAUser(userId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _authentication.GetUser(userId);

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
        [ProducesResponseType(201, Type = typeof(Cart))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCart([FromBody] IEnumerable<CartProductDto> products)
        {
            if (products == null)
                return BadRequest();

            var userId = await GetUserIdFromToken();

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
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCart()
        {
            var userId = await GetUserIdFromToken();

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
