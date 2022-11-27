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

        public DirectorsController(ICelebrityRepository celebrityRepository, IProductRepository productRepository)
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
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

        //api/directors/celebrityId/products
        [HttpGet("{celebrityId}/products")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProductsOfADirector(int celebrityId)
        {
            if (!_celebrityRepository.DirectorExists(celebrityId))
                return NotFound();

            var products = _celebrityRepository.GetProductsOfADirector(celebrityId);

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
    }
}
