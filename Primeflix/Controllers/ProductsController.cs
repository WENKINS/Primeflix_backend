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

        //api/products?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToCreate)
        {
            var statusCode = ValidateProduct(dirId, actId, genresId, productToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!_productRepository.CreateProduct(productToCreate, dirId, actId, genresId))
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
        public IActionResult UpdateProduct(int productId, [FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToUpdate)
        {

            var statusCode = ValidateProduct(dirId, actId, genresId, productToUpdate);

            if (productId != productToUpdate.Id)
                return BadRequest();

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!_productRepository.UpdateProduct(productToUpdate, dirId, actId, genresId))
            {
                ModelState.AddModelError("", $"Something went wrong updating the product {productToUpdate.Title}");
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

        private StatusCodeResult ValidateProduct(List<int> directorsId, List<int> actorsId, List<int> genresId, Product product)
        {
            if (product == null || directorsId.Count() <= 0 || actorsId.Count() <= 0 || genresId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing product, director(s), actor(s), or genre(s)");
                return BadRequest();
            }

            if(_productRepository.IsDuplicate(product.Id, product.Title))
            {
                ModelState.AddModelError("", "Duplicate title");
                return StatusCode(422);
            }

            foreach(var id in directorsId)
            {
                if(!_celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in actorsId)
            {
                if (!_celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in genresId)
            {
                if (!_genreRepository.GenreExists(id))
                {
                    ModelState.AddModelError("", "Genre Not Found");
                    return StatusCode(404);
                }
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }
    }
}
