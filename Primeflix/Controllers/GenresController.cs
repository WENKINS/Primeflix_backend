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
        private IFormatRepository _formatRepository;
        private IGenreTranslationRepository _genreTranslationRepository;
        private IProductTranslationRepository _productTranslationRepository;

        public GenresController(
            IGenreRepository genreRepository, 
            IProductRepository productRepository, 
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository
            )
        {
            _genreRepository = genreRepository;
            _productRepository = productRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
        }

        //api/genres/languageCode
        [HttpGet("{languageCode}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenres(string languageCode)
        {
            var genres = _genreRepository.GetGenres(languageCode).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                var genreTranslation = _genreTranslationRepository.GetGenreTranslation(genre.Id, languageCode);
                genresDto.Add(new GenreDto
                {
                    Id = genre.Id,
                    Name = genreTranslation.Translation
                });
            }
            return Ok(genresDto);
        }

        //api/genres/languageCode/genreId
        [HttpGet("{languageCode}/{genreId}", Name = "GetGenre")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenre(string languageCode, int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var genre = _genreRepository.GetGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genreTranslation = _genreTranslationRepository.GetGenreTranslation(genre.Id, languageCode);

            var genreDto = new GenreDto()
            {
                Id = genre.Id,
                Name = genreTranslation.Translation
            };

            return Ok(genreDto);
        }

        //api/genres/products/productId
        [HttpGet("products/{languageCode}/{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public IActionResult GetGenresOfAProduct(int productId, string languageCode)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var genres = _genreRepository.GetGenresOfAProduct(productId);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                var genreTranslation = _genreTranslationRepository.GetGenreTranslation(genre.Id, languageCode);
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

        //api/genres/genresId/products
        [HttpGet("{languageCode}/{genreId}/products")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProductsOfAGenre(string languageCode, int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var products = _genreRepository.GetProductsOfAGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();

            foreach (var product in products)
            {
                var oFormat = _formatRepository.GetFormatOfAProduct(product.Id);
                var formatDto = new FormatDto()
                {
                    Id = oFormat.Id,
                    Name = oFormat.Name
                };

                var productTranslation = _productTranslationRepository.GetProductTranslation(product.Id, languageCode);

                productsDto.Add(new ProductDetailsDto
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

            var genre = _genreRepository.GetGenres("fr")
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
