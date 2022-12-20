using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
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
        [HttpGet(Name = "GetCarts")]
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
                var productsDto = new List<ProductDto>();

                foreach(var product in products)
                {
                    productsDto.Add(new ProductDto
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

        //api/products/search/params
        /*[HttpGet("search/{searchText}", Name = "SearchProducts")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public async Task<IActionResult> SearchProducts(string searchText, [FromQuery] int page = 1)
        {
            var products = await _productRepository.SearchProducts(searchText);

            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var oGenres = await _genreRepository.GetGenresOfAProduct(product.Id);
                var genresDto = new List<GenreDto>();

                foreach (var genre in oGenres)
                {
                    var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, "en");
                    genresDto.Add(new GenreDto
                    {
                        Id = genre.Id,
                        Name = genreTranslation.Translation
                    });
                }

                var oFormat = await _formatRepository.GetFormatOfAProduct(product.Id);
                var formatDto = new FormatDto()
                {
                    Id = oFormat.Id,
                    Name = oFormat.Name
                };

                var productTranslation = await _productTranslationRepository.GetProductTranslation(product.Id, "en");

                if (productTranslation != null)
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = productTranslation.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

                else
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

            }

            var pageResults = 10;
            var pageCount = Math.Ceiling(((double)products.Count() / (double)pageResults));

            var productsResults = productsDto.Skip((page - 1) * pageResults).Take((int)pageResults).ToList();

            ProductsPageResultsDto productsPageResultsDto = new ProductsPageResultsDto()
            {
                Products = productsResults,
                CurrentPage = page,
                TotalPages = (int)pageCount
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(productsPageResultsDto);
        }

        //api/productId
        [AllowAnonymous]
        [HttpGet("{productId}", Name = "GetProduct")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProduct(int productId, [FromQuery] string? lang = "en")
        {
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var product = await _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var directors = await _celebrityRepository.GetDirectorsOfAProduct(product.Id);
            var directorsDto = new List<CelebrityDto>();

            foreach (var director in directors)
            {
                directorsDto.Add(new CelebrityDto
                {
                    Id = director.Id,
                    FirstName = director.FirstName,
                    LastName = director.LastName
                });
            }

            var actors = await _celebrityRepository.GetActorsOfAProduct(product.Id);
            var actorsDto = new List<CelebrityDto>();

            foreach (var actor in actors)
            {
                actorsDto.Add(new CelebrityDto
                {
                    Id = actor.Id,
                    FirstName = actor.FirstName,
                    LastName = actor.LastName
                });
            }

            var genres = await _genreRepository.GetGenresOfAProduct(product.Id);
            var genresDto = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, lang);
                genresDto.Add(new GenreDto
                {
                    Id = genre.Id,
                    Name = genreTranslation.Translation
                });
            }

            var oFormat = await _formatRepository.GetFormatOfAProduct(product.Id);
            var formatDto = new FormatDto()
            {
                Id = oFormat.Id,
                Name = oFormat.Name
            };

            var productTranslation = await _productTranslationRepository.GetProductTranslation(product.Id, lang);

            var productDto = new ProductDetailsDto()
            {
                Id = product.Id,
                Title = productTranslation.Title,
                Summary = productTranslation.Summary,
                ReleaseDate = product.ReleaseDate,
                Duration = product.Duration,
                Stock = product.Stock,
                Rating = product.Rating,
                Format = formatDto,
                PictureUrl = product.PictureUrl,
                Price = product.Price,
                Directors = directorsDto,
                Actors = actorsDto,
                Genres = genresDto
            };

            return Ok(productDto);
        }

        //api/products/genres/productId
        [AllowAnonymous]
        [HttpGet("genres/{languageCode}/{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenresOfAProduct(int productId, string languageCode)
        {
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var genres = await _genreRepository.GetGenresOfAProduct(productId);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, languageCode);
                genresDto.Add(new GenreDto()
                {
                    Id = genre.Id,
                    Name = genreTranslation.Translation
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genresDto);
        }

        //api/products?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [Authorize]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProduct([FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToCreate)
        {
            var statusCode = await ValidateProduct(dirId, actId, genresId, productToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

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
        [Authorize]
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduct(int productId, [FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToUpdate)
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
        }

        //api/products/productId
        [Authorize]
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete = await _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {productToDelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private async Task<StatusCodeResult> ValidateProduct(List<int> directorsId, List<int> actorsId, List<int> genresId, Product product)
        {
            if (product == null || directorsId.Count() <= 0 || actorsId.Count() <= 0 || genresId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing product, director(s), actor(s), or genre(s)");
                return BadRequest();
            }

            if (await _productRepository.IsDuplicate(product.Id, product.Title))
            {
                ModelState.AddModelError("", "Duplicate title");
                return StatusCode(422);
            }

            if (!await _formatRepository.FormatExists(product.Format.Id))
            {
                ModelState.AddModelError("", "Format doesn't exist");
                return StatusCode(404);
            }

            foreach (var id in directorsId)
            {
                if (!await _celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in actorsId)
            {
                if (!await _celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in genresId)
            {
                if (!await _genreRepository.GenreExists(id))
                {
                    ModelState.AddModelError("", "Genre Not Found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }*/
    }
}
