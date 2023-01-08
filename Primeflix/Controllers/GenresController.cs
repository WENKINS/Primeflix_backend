using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.FormatService;
using Primeflix.Services.GenreService;
using Primeflix.Services.GenreTranslationService;
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

        public GenresController(
            IGenreRepository genreRepository, 
            IProductRepository productRepository, 
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository,
            IUserRepository userRepository
            )
        {
            _genreRepository = genreRepository;
            _productRepository = productRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
            _userRepository = userRepository;
        }

        //api/genres
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenres([FromQuery] string? lang = "en")
        {
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
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var genres = await _genreRepository.GetGenresOfAProduct(productId);

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

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genresDto);
        }

        //api/genres/genresId/products
        [HttpGet("{genreId}/products")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProductsOfAGenre(int genreId, [FromQuery] string lang)
        {
            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            var products = await _genreRepository.GetProductsOfAGenre(genreId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();

            foreach (var product in products)
            {
                var oFormat = await _formatRepository.GetFormatOfAProduct(product.Id);
                var formatDto = new FormatDto()
                {
                    Id = oFormat.Id,
                    Name = oFormat.Name
                };

                var productTranslation = await _productTranslationRepository.GetProductTranslation(product.Id, lang);

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
        [Authorize]
        [ProducesResponseType(201, Type = typeof(Genre))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateGenre([FromBody]NewGenreDto genreToCreate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (genreToCreate == null)
                return BadRequest(ModelState);

            if (await _genreRepository.GenreExists(genreToCreate.EnglishName) || await _genreRepository.GenreExists(genreToCreate.FrenchName))
            {
                ModelState.AddModelError("", $"Genre already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!await _genreRepository.CreateGenre(genreToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong creating Genre {genreToCreate.EnglishName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetGenre", new { genreId = genreToCreate.Id }, genreToCreate);
        }

        //api/genres/genreId
        [HttpPut("{genreId}")]
        [Authorize]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateGenre(int genreId, [FromBody]NewGenreDto updatedGenre)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (updatedGenre == null)
                return BadRequest(ModelState);

            if (genreId != updatedGenre.Id)
                return BadRequest(ModelState);

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            if(await _genreRepository.IsDuplicate(updatedGenre.EnglishName) || await _genreRepository.IsDuplicate(updatedGenre.FrenchName))
            {
                ModelState.AddModelError("", $"Genre already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!await _genreRepository.UpdateGenre(updatedGenre))
            {
                ModelState.AddModelError("", $"Something went wrong updating the genre");
                return StatusCode(500, ModelState);
            }

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
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            if (!await _genreRepository.GenreExists(genreId))
                return NotFound();

            var genreToDelete = await _genreRepository.GetGenre(genreId);

            var products = await _genreRepository.GetProductsOfAGenre(genreId);

            if(products.Count() >0)
            {
                ModelState.AddModelError("", $"Genre {genreToDelete.Name} cannot be deleted because it is used by at least one product");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!await _genreRepository.DeleteGenre(genreToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {genreToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
