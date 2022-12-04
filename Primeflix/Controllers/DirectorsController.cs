using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : Controller
    {
        private ICelebrityRepository _celebrityRepository;
        private IProductRepository _productRepository;
        private IGenreRepository _genreRepository;
        private IFormatRepository _formatRepository;
        private IGenreTranslationRepository _genreTranslationRepository;

        public DirectorsController(
            ICelebrityRepository celebrityRepository, 
            IProductRepository productRepository, 
            IGenreRepository genreRepository, 
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository
            )
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
        }

        //api/directors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetDirectors()
        {
            var celebrities = _celebrityRepository.GetDirectors();

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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetDirector(int celebrityId)
        {
            if (!_celebrityRepository.DirectorExists(celebrityId))
                return NotFound();

            var director = _celebrityRepository.GetDirector(celebrityId);

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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetDirectorsOfAProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var directors = _celebrityRepository.GetDirectorsOfAProduct(productId);

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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProductsOfADirector(int celebrityId, string languageCode)
        {
            if (!_celebrityRepository.DirectorExists(celebrityId))
                return NotFound();

            var products = _celebrityRepository.GetProductsOfADirector(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();

            foreach (var product in products)
            {
                var directors = _celebrityRepository.GetDirectorsOfAProduct(product.Id);
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

                var actors = _celebrityRepository.GetActorsOfAProduct(product.Id);
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

                var genres = _genreRepository.GetGenresOfAProduct(product.Id);
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
                
                var oFormat = _formatRepository.GetFormatOfAProduct(product.Id);
                var formatDto = new FormatDto()
                {
                    Id = oFormat.Id,
                    Name = oFormat.Name
                };

                productsDto.Add(new ProductDetailsDto
                {
                    Id = product.Id,
                    Title = product.Title,
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
