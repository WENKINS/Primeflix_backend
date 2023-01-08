using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.FormatService;
using Primeflix.Services.GenreService;
using Primeflix.Services.GenreTranslationService;
using Primeflix.Services.LanguageService;
using Primeflix.Services.ProductService;
using Primeflix.Services.ProductTranslationService;
using Primeflix.Services.UserService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFormatRepository _formatRepository;
        private readonly IGenreTranslationRepository _genreTranslationRepository;
        private readonly IProductTranslationRepository _productTranslationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILanguageRepository _languageRepository;

        public GenresController(
            IGenreRepository genreRepository,
            IProductRepository productRepository,
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository,
            IUserRepository userRepository,
            ILanguageRepository languageRepository
            )
        {
            _genreRepository = genreRepository;
            _productRepository = productRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
            _userRepository = userRepository;
            _languageRepository = languageRepository;
        }

        //api/genres
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenres([FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            var genres = await _genreRepository.GetGenres(lang);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
            return Ok(genresDto);
        }

        //api/genres/genreId?lang=fr
        [HttpGet("{genreId}", Name = "GetGenre")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenre(int genreId, [FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            var genre = await _genreRepository.GetGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, lang);

            var genreDto = new GenreDto()
            {
                Id = genre.Id,
                Name = genreTranslation.Translation
            };

            return Ok(genreDto);
        }

        //api/genres/products/productId?lang=fr
        [HttpGet("products/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenresOfAProduct(int productId, [FromQuery] string lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var genres = await _genreRepository.GetGenresOfAProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genresDto = new List<GenreDto>();
            foreach (var genre in genres)
            {
                var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, lang);
                genresDto.Add(new GenreDto()
                {
                    Id = genre.Id,
                    Name = genreTranslation.Translation
                });
            }

            return Ok(genresDto);
        }

        //api/genres/genresId/products
        [HttpGet("{genreId}/products")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProductsOfAGenre(int genreId, [FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
                return BadRequest("Language doesn't exist");

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            var products = await _genreRepository.GetProductsOfAGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var oFormat = await _formatRepository.GetFormatOfAProduct(product.Id);
                var formatDto = new FormatDto()
                {
                    Id = oFormat.Id,
                    Name = oFormat.Name
                };

                var productTranslation = await _productTranslationRepository.GetProductTranslation(product.Id, lang);

                productsDto.Add(new ProductDto
                {
                    Id = product.Id,
                    Title = productTranslation.Title,
                    ReleaseDate = product.ReleaseDate,
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
        [Authorize]
        [ProducesResponseType(201, Type = typeof(Genre))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateGenre([FromBody]GenreWithTranslationsDto genreToCreate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (genreToCreate == null)
                return BadRequest("No data was provided");

            if (await _genreRepository.GenreExists(genreToCreate.EnglishName) || await _genreRepository.GenreExists(genreToCreate.FrenchName))
                return StatusCode(409, "Genre already exists");

            if (!await _genreRepository.CreateGenre(genreToCreate))
                return StatusCode(500, "Something went wrong adding the genre");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return StatusCode(201);
        }

        //api/genres/genreId
        [HttpPut("{genreId}")]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateGenre(int genreId, [FromBody]GenreWithTranslationsDto updatedGenre)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (updatedGenre == null)
                return BadRequest("No data was provided");

            if (genreId != updatedGenre.Id)
                return BadRequest("Genre IDs are not the same");

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            if (await _genreRepository.IsDuplicate(updatedGenre.EnglishName) || await _genreRepository.IsDuplicate(updatedGenre.FrenchName))
                return StatusCode(409, "Genre name already exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _genreRepository.UpdateGenre(updatedGenre))
                return StatusCode(500, "Something went wrong updating the genre");

            return NoContent();
        }

        //api/genres/genreId
        [HttpDelete("{genreId}")]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteGenre(int genreId)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest("User role could not be retrieved");

            if (!userRole.Equals("admin"))
                return StatusCode(401, "User is not an admin");

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            var genreToDelete = await _genreRepository.GetGenre(genreId);

            if (!await _genreRepository.DeleteGenre(genreToDelete))
                return StatusCode(500, $"Something went wrong deleting {genreToDelete.Name}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
    }
}
