﻿using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Services.CelebrityService;
using Primeflix.Services.FormatService;
using Primeflix.Services.GenreService;
using Primeflix.Services.GenreTranslationService;
using Primeflix.Services.ProductService;
using Primeflix.Services.ProductTranslationService;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : Controller
    {
        private ICelebrityRepository _celebrityRepository;
        private IProductRepository _productRepository;
        private IGenreRepository _genreRepository;
        private IFormatRepository _formatRepository;
        private IGenreTranslationRepository _genreTranslationRepository;
        private IProductTranslationRepository _productTranslationRepository;

        public ActorsController(
            ICelebrityRepository celebrityRepository,
            IProductRepository productRepository,
            IGenreRepository genreRepository,
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository
            )
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
        }

        //api/actors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetActors()
        {
            var celebrities = _celebrityRepository.GetActors();

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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetActor(int celebrityId)
        {
            if (!_celebrityRepository.ActorExists(celebrityId))
                return NotFound();

            var actor = _celebrityRepository.GetActor(celebrityId);

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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetActorsOfAProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var actors = _celebrityRepository.GetActorsOfAProduct(productId);

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

        //api/actors/celebrityId/products/languageCode
        [HttpGet("{languageCode}/{celebrityId}/products")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProductsOfAnActor(int celebrityId, string languageCode)
        {
            if (!_celebrityRepository.ActorExists(celebrityId))
                return NotFound();

            var products = _celebrityRepository.GetProductsOfAnActor(celebrityId);

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
