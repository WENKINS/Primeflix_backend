using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services.CelebrityService;
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
    public class DirectorsController : ControllerBase
    {
        private readonly ICelebrityRepository _celebrityRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IFormatRepository _formatRepository;
        private readonly IGenreTranslationRepository _genreTranslationRepository;
        private readonly IProductTranslationRepository _productTranslationRepository;
        private readonly IUserRepository _userRepository;

        public DirectorsController(
            ICelebrityRepository celebrityRepository, 
            IProductRepository productRepository, 
            IGenreRepository genreRepository, 
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository,
            IUserRepository userRepository
            )
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
            _userRepository = userRepository;
        }

        //api/directors
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetDirectors()
        {
            var celebrities = await _celebrityRepository.GetDirectors();

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

        //api/directors/celebrityId
        [HttpGet("{celebrityId}", Name = "GetDirector")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetDirector(int celebrityId)
        {
            if (!await _celebrityRepository.DirectorExists(celebrityId))
                return NotFound();

            var director = await _celebrityRepository.GetDirector(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebrityDto = new CelebrityDto()
            {
                Id = director.Id,
                FirstName = director.FirstName,
                LastName = director.LastName
            };

            return Ok(celebrityDto);
        }

        //api/directors/products/productId
        [HttpGet("products/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public async Task<IActionResult> GetDirectorsOfAProduct(int productId)
        {
            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var directors = await _celebrityRepository.GetDirectorsOfAProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebritiesDto = new List<CelebrityDto>();
            foreach (var director in directors)
            {
                celebritiesDto.Add(new CelebrityDto()
                {
                    Id = director.Id,
                    FirstName = director.FirstName,
                    LastName = director.LastName
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(celebritiesDto);
        }

        //api/directors/celebrityId/products/languageCode
        [HttpGet("{languageCode}/{celebrityId}/products")]
        [AllowAnonymous]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProductsOfADirector(int celebrityId, string languageCode)
        {
            if (!await _celebrityRepository.DirectorExists(celebrityId))
                return NotFound();

            var products = await _celebrityRepository.GetProductsOfADirector(celebrityId);

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
                    var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(genre.Id, languageCode);
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

                var productTranslation = await _productTranslationRepository.GetProductTranslation(product.Id, languageCode);

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
