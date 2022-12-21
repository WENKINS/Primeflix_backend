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
        [HttpGet("all", Name = "GetCarts")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CartDto>))]
        public async Task<IActionResult> GetCarts()
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
                var productsDto = new List<ProductLessDetailsDto>();

                foreach(var product in products)
                {
                    productsDto.Add(new ProductLessDetailsDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Price = product.Price,
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
        [HttpGet(Name = "GetCart")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CartDto>))]
        public async Task<IActionResult> GetCart()
        {
            string bearerToken = HttpContext.Request.Headers["Authorization"];

            if(String.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                return BadRequest();
            }

            string token = bearerToken.Substring("Bearer ".Length);
            int userId = Int32.Parse(await _authentication.DecodeToken(token));

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
            var productsDto = new List<ProductLessDetailsDto>();

            foreach (var product in products)
            {
                productsDto.Add(new ProductLessDetailsDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    Price = product.Price,
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

        //api/products?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        /*[HttpPost]
        [ProducesResponseType(201, Type = typeof(Cart))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddToCart([FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToCreate)
        {
            productToCreate.Format = await _formatRepository.GetFormat(productToCreate.Format.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _productRepository.CreateProduct(productToCreate, dirId, actId, genresId))
            {
                ModelState.AddModelError("", $"Something went wrong saving the product {productToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { productId = productToCreate.Id }, productToCreate);
        }

        //api/products/productId?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCart(int productId, [FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToUpdate)
        {

            var statusCode = await ValidateProduct(dirId, actId, genresId, productToUpdate);

            if (productId != productToUpdate.Id)
                return BadRequest();

            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            productToUpdate.Format = await _formatRepository.GetFormat(productToUpdate.Format.Id);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!await _productRepository.UpdateProduct(productToUpdate, dirId, actId, genresId))
            {
                ModelState.AddModelError("", $"Something went wrong updating the product {productToUpdate.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }*/

        //api/carts/cartId
        [HttpDelete("{cartId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            if (!await _cartRepository.CartExists(cartId))
                return NotFound();

            var cartToDelete = await _cartRepository.GetCart(cartId);

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
