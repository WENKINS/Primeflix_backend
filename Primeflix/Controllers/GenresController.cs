using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : Controller
    {

        private IGenreRepository _genreRepository;
        private IProductRepository _productRepository;

        public GenresController(IGenreRepository genreRepository, IProductRepository productRepository)
        {
            _genreRepository = genreRepository;
            _productRepository = productRepository;
        }

        //api/genres
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenres()
        {
            var genres = _genreRepository.GetGenres().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                genresDto.Add(new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                });
            }
            return Ok(genresDto);
        }

        //api/genres/genreId
        [HttpGet("{genreId}", Name = "GetGenre")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenre(int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var genre = _genreRepository.GetGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genreDto = new GenreDto()
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return Ok(genreDto);
        }

        //api/genres/products/productId
        [HttpGet("products/{productId}")]
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

        //api/genres/genresId/products
        [HttpGet("{genreId}/products")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProductsOfAGenre(int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var products = _genreRepository.GetProductsOfAGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();

            foreach (var product in products)
            {
                productsDto.Add(new ProductDetailsDto
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

        //api/genres
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Genre))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateGenre([FromBody]Genre genreToCreate)
        {
            if (genreToCreate == null)
                return BadRequest(ModelState);

            var genre = _genreRepository.GetGenres()
                .Where(g => g.Name.Trim().ToUpper() == genreToCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (genre != null)
            {
                ModelState.AddModelError("", $"Genre {genreToCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_genreRepository.CreateGenre(genreToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {genreToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetGenre", new { genreId = genreToCreate.Id }, genreToCreate);
        }

        //api/genres/genreId
        [HttpPut("{genreId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateGenre(int genreId, [FromBody]Genre updatedGenre)
        {
            if (updatedGenre == null)
                return BadRequest(ModelState);

            if (genreId != updatedGenre.Id)
                return BadRequest(ModelState);

            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            if(_genreRepository.IsDuplicate(genreId, updatedGenre.Name))
            {
                ModelState.AddModelError("", $"Genre {updatedGenre.Name} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_genreRepository.UpdateGenre(updatedGenre))
            {
                ModelState.AddModelError("", $"Something went wrong update {updatedGenre.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/genres/genreId
        [HttpDelete("{genreId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteGenre(int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var genreToDelete = _genreRepository.GetGenre(genreId);

            if(_genreRepository.GetProductsOfAGenre(genreId).Count() >0)
            {
                ModelState.AddModelError("", $"Genre {genreToDelete.Name} cannot be deleted because it is used by at least one product");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_genreRepository.DeleteGenre(genreToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {genreToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
