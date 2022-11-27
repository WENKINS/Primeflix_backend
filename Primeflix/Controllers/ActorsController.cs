using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : Controller
    {
        private ICelebrityRepository _celebrityRepository;
        private IProductRepository _productRepository;

        public ActorsController(ICelebrityRepository celebrityRepository, IProductRepository productRepository)
        {
            _celebrityRepository = celebrityRepository;
            _productRepository = productRepository;
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

        //api/actors/celebrityId/products
        [HttpGet("{celebrityId}/products")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProductsOfAnActor(int celebrityId)
        {
            if (!_celebrityRepository.ActorExists(celebrityId))
                return NotFound();

            var products = _celebrityRepository.GetProductsOfAnActor(celebrityId);

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
