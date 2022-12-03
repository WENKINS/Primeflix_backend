using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    // Class that handles the requests and returns responses

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepository _productRepository;
        private IGenreRepository _genreRepository;
        private ICelebrityRepository _celebrityRepository;

        public ProductsController(IProductRepository productRepository, IGenreRepository genreRepository, ICelebrityRepository celebrityRepository)
        {
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _celebrityRepository = celebrityRepository;
        } 

/*        //api/products
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDetailsDto>();
            foreach (var product in products)
            {
                var directors = _celebrityRepository.GetDirectorsOfAProduct(product.Id);
                var directorsDto = new List<CelebrityDto>();

                foreach(var director in directors)
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
                    genresDto.Add(new GenreDto
                    {
                        Id = genre.Id,
                        Name = genre.Name
                    });
                }

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
                    Price = product.Price,
                    Directors = directorsDto,
                    Actors = actorsDto,
                    Genres = genresDto
                }) ;
            }
            return Ok(productsDto);
        }*/

        //api/products/params
        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProducts([FromQuery]bool recentlyAdded = false, [FromQuery]string? format = "All", [FromQuery]List<string>? genres = null)
        {
            List<int> genresId = new List<int>();

            if (genres != null && genres.Count > 0)
            {
                foreach (var genre in genres)
                {
                    genresId.Add(_genreRepository.GetGenre(genre).Id);
                }
            }

            else
            {
                genresId = null;
            }

            var products = _productRepository.FilterResults(recentlyAdded, format, genresId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var oGenres = _genreRepository.GetGenresOfAProduct(product.Id);
                var genresDto = new List<GenreDto>();

                foreach (var genre in oGenres)
                {
                    genresDto.Add(new GenreDto
                    {
                        Id = genre.Id,
                        Name = genre.Name
                    });
                }

                productsDto.Add(new ProductDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    ReleaseDate = product.ReleaseDate,
                    Stock = product.Stock,
                    Rating = product.Rating,
                    Format = product.Format,
                    PictureUrl = product.PictureUrl,
                    Price = product.Price,
                    Genres = genresDto
                });
            }
            return Ok(productsDto);
        }

        //api/products/productId
        [HttpGet("{productId}", Name = "GetProduct")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public IActionResult GetProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var product = _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                genresDto.Add(new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                });
            }

            var productDto = new ProductDetailsDto()
            {
                Id = product.Id,
                Title = product.Title,
                ReleaseDate = product.ReleaseDate,
                Duration = product.Duration,
                Stock = product.Stock,
                Rating = product.Rating,
                Format = product.Format,
                PictureUrl = product.PictureUrl,
                Price = product.Price,
                Directors = directorsDto,
                Actors = actorsDto,
                Genres = genresDto
            };

            return Ok(productDto);
        }

        //api/products/title/Title
        /*[HttpGet("title/{title}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProduct(string title)
        {
            if (!_productRepository.ProductExists(title))
                return NotFound();

            var product = _productRepository.GetProduct(title);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                genresDto.Add(new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                });
            }

            var productDto = new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                ReleaseDate = product.ReleaseDate,
                Duration = product.Duration,
                Stock = product.Stock,
                Rating = product.Rating,
                Format = product.Format,
                PictureUrl = product.PictureUrl,
                Price = product.Price,
                Directors = directorsDto,
                Actors = actorsDto,
                Genres = genresDto
            };

            return Ok(productDto);
        }*/

        //api/products/genres/productId
        [HttpGet("genres/{productId}")]
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

        //api/products?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToCreate)
        {
            var statusCode = ValidateProduct(dirId, actId, genresId, productToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!_productRepository.CreateProduct(productToCreate, dirId, actId, genresId))
            {
                ModelState.AddModelError("", $"Something went wrong saving the product {productToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { productId = productToCreate.Id }, productToCreate);
        }

        //api/products/productId?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateProduct(int productId, [FromQuery] List<int> dirId, [FromQuery] List<int> actId, [FromQuery] List<int> genresId, [FromBody] Product productToUpdate)
        {

            var statusCode = ValidateProduct(dirId, actId, genresId, productToUpdate);

            if (productId != productToUpdate.Id)
                return BadRequest();

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!_productRepository.UpdateProduct(productToUpdate, dirId, actId, genresId))
            {
                ModelState.AddModelError("", $"Something went wrong updating the product {productToUpdate.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/products/productId
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete = _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {productToDelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private StatusCodeResult ValidateProduct(List<int> directorsId, List<int> actorsId, List<int> genresId, Product product)
        {
            if (product == null || directorsId.Count() <= 0 || actorsId.Count() <= 0 || genresId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing product, director(s), actor(s), or genre(s)");
                return BadRequest();
            }

            if(_productRepository.IsDuplicate(product.Id, product.Title))
            {
                ModelState.AddModelError("", "Duplicate title");
                return StatusCode(422);
            }

            foreach(var id in directorsId)
            {
                if(!_celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in actorsId)
            {
                if (!_celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in genresId)
            {
                if (!_genreRepository.GenreExists(id))
                {
                    ModelState.AddModelError("", "Genre Not Found");
                    return StatusCode(404);
                }
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }
    }
}
