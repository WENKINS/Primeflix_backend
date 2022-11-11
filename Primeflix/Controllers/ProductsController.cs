using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    // Class that handles the requests and returns responses

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepository _productRepository;
        private IGenreRepository _genreRepository;

        public ProductsController(IProductRepository productRepository, IGenreRepository genreRepository)
        {
            _productRepository = productRepository;
            _genreRepository = genreRepository;
        } 

        //api/products
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDto>();
            foreach (var product in products)
            {
                productsDto.Add(new ProductDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    ReleaseDate = product.ReleaseDate,
                    Duration = product.Duration,
                    Stock = product.Stock,
                    Rating = product.Rating,
                    Format = product.Format,
                    PictureUrl = product.PictureUrl,
                    Price = product.Price
                });
            }
            return Ok(productsDto);
        }

        //api/products/id/productId
        [HttpGet("id/{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var product = _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productDto = new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                ReleaseDate = product.ReleaseDate,
                Duration = product.Duration,
                Stock = product.Stock,
                Rating = product.Rating,
                Format = product.Format,
                PictureUrl = product.PictureUrl,
                Price = product.Price
            };

            return Ok(productDto);
        }

        //api/products/title/Title
        [HttpGet("title/{title}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProduct(string title)
        {
            if (!_productRepository.ProductExists(title))
                return NotFound();

            var product = _productRepository.GetProduct(title);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productDto = new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                ReleaseDate = product.ReleaseDate,
                Duration = product.Duration,
                Stock = product.Stock,
                Rating = product.Rating,
                Format = product.Format,
                PictureUrl = product.PictureUrl,
                Price = product.Price
            };

            return Ok(productDto);
        }

        //api/products/genres/productId
        [HttpGet("genres/{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenresOfAProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var genres = _genreRepository.GetGenresOfAProduct(productId);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                genresDto.Add(new GenreDto()
                {
                    Id = genre.Id,
                    Name = genre.Name
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genresDto);
        }
    }
}
