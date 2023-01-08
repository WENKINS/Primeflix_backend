using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services.CelebrityService;
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
    public class ActorsController : ControllerBase
    {
        private readonly ICelebrityRepository _celebrityRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IFormatRepository _formatRepository;
        private readonly IGenreTranslationRepository _genreTranslationRepository;
        private readonly IProductTranslationRepository _productTranslationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILanguageRepository _languageRepository;

        public ActorsController(
            ICelebrityRepository celebrityRepository,
            IProductRepository productRepository,
            IGenreRepository genreRepository,
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository,
            IUserRepository userRepository,
            ILanguageRepository languageRepository
            )
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
            _userRepository = userRepository;
            _languageRepository = languageRepository;
        }

        //api/actors
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetActors()
        {
            var celebrities = await _celebrityRepository.GetActors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebritiesDto = new List<CelebrityDto>();
            foreach (var celebrity in celebrities)
            {
                celebritiesDto.Add(new CelebrityDto
                {
                    Id = celebrity.Id,
                    FirstName = celebrity.FirstName,
                    LastName = celebrity.LastName
                });
            }
            return Ok(celebritiesDto);
        }

        //api/actors/celebrityId
        [HttpGet("{celebrityId}", Name = "GetActor")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetActor(int celebrityId)
        {
            if (!await _celebrityRepository.ActorExists(celebrityId))
                return NotFound();

            var actor = await _celebrityRepository.GetActor(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebrityDto = new CelebrityDto()
            {
                Id = actor.Id,
                FirstName = actor.FirstName,
                LastName = actor.LastName
            };

            return Ok(celebrityDto);
        }

        //api/actors/products/productId
        [HttpGet("products/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetActorsOfAProduct(int productId)
        {
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var actors = await _celebrityRepository.GetActorsOfAProduct(productId);

            var celebritiesDto = new List<CelebrityDto>();
            foreach (var actor in actors)
            {
                celebritiesDto.Add(new CelebrityDto()
                {
                    Id = actor.Id,
                    FirstName = actor.FirstName,
                    LastName = actor.LastName
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(celebritiesDto);
        }

        //api/actors/celebrityId/products?lang
        [HttpGet("{celebrityId}/products")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProductsOfAnActor(int celebrityId, [FromQuery] string lang)
        {
            if (!await _celebrityRepository.ActorExists(celebrityId))
                return NotFound();

            if (!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

            var products = await _celebrityRepository.GetProductsOfAnActor(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();

            foreach (var product in products)
            {
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
                    Price = product.Price,
                    Directors = directorsDto,
                    Actors = actorsDto,
                    Genres = genresDto
                });
            }

            return Ok(productsDto);
        }
    }
}
