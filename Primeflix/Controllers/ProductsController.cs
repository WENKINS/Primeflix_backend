using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
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
        private ICelebrityRepository _celebrityRepository;

        public ProductsController(IProductRepository productRepository, IGenreRepository genreRepository, ICelebrityRepository celebrityRepository)
        {
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _celebrityRepository = celebrityRepository;
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
        [HttpGet("id/{productId}", Name = "GetProduct")]
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

        //api/products
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromBody] Product productToCreate)
        {
            if (productToCreate == null)
                return BadRequest(ModelState);

            var product = _productRepository.GetProducts()
                .Where(p => p.Title.Trim().ToUpper() == productToCreate.Title.Trim().ToUpper())
                .FirstOrDefault();

            if (product != null)
            {
                var existingDirector = _celebrityRepository.GetDirectorsOfAProduct(product.Id);
                var newDirector = _celebrityRepository.GetDirectorsOfAProduct(productToCreate.Id);
                if (existingDirector.SequenceEqual(newDirector))
                {
                    ModelState.AddModelError("", $"Product {productToCreate.Title} already exists");
                    return StatusCode(422, ModelState);
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.CreateProduct(productToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {productToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { productId = productToCreate.Id }, productToCreate);
        }

        //api/products/productId
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateProduct(int productId, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
                return BadRequest(ModelState);

            if (productId != updatedProduct.Id)
                return BadRequest(ModelState);

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            if (_productRepository.IsDuplicate(productId, updatedProduct.Title))
            {
                ModelState.AddModelError("", $"Product {updatedProduct.Title} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.UpdateProduct(updatedProduct))
            {
                ModelState.AddModelError("", $"Something went wrong update {updatedProduct.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/products/productId
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete = _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {productToDelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
