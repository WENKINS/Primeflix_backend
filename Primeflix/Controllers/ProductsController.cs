using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Primeflix.DTO;
using Primeflix.Models;
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
    // Class that handles the requests and returns responses

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICelebrityRepository _celebrityRepository;
        private readonly IFormatRepository _formatRepository;
        private readonly IGenreTranslationRepository _genreTranslationRepository;
        private readonly IProductTranslationRepository _productTranslationRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserRepository _userRepository;

        public ProductsController(
            IProductRepository productRepository,
            IGenreRepository genreRepository,
            ICelebrityRepository celebrityRepository,
            IFormatRepository formatRepository,
            IGenreTranslationRepository genreTranslationRepository,
            IProductTranslationRepository productTranslationRepository,
            ILanguageRepository languageRepository,
            IUserRepository userRepository
            )
        {
            _productRepository = productRepository;
            _genreRepository = genreRepository;
            _celebrityRepository = celebrityRepository;
            _formatRepository = formatRepository;
            _genreTranslationRepository = genreTranslationRepository;
            _productTranslationRepository = productTranslationRepository;
            _languageRepository = languageRepository;
            _userRepository = userRepository;
        }

        //api/products/params
        [AllowAnonymous]
        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? lang = "en", [FromQuery] bool recentlyAdded = false, [FromQuery] string? format = "All", [FromQuery] List<string>? genre = null)
        {
            if (pageSize <= 0)
                return BadRequest();

            if (page < 1)
                return BadRequest();

            if(!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

            if(!(await _formatRepository.FormatExists(format)) && !format.Equals("All"))
            {
                ModelState.AddModelError("", $"Format doesn't exist");
                return StatusCode(500, ModelState);
            }

            List<int> genresId = new List<int>();

            if (genre != null && genre.Count > 0)
            {
                foreach (var singleGenre in genre)
                {
                    if(!(await _genreRepository.GenreExists(singleGenre)))
                    {
                        ModelState.AddModelError("", $"Genre doesn't exist");
                        return StatusCode(500, ModelState);
                    }
                    genresId.Add((await _genreRepository.GetGenre(singleGenre)).Id);
                }
            }

            else
            {
                genresId = null;
            }

            int formatId = 0;

            if (!Equals("All", format))
            {
                var productFormat = await _formatRepository.GetFormat(format);
                formatId = productFormat.Id;
            }

            var products = await _productRepository.FilterResults(recentlyAdded, formatId, genresId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var oGenres = await _genreRepository.GetGenresOfAProduct(product.Id);
                var genresDto = new List<GenreDto>();

                foreach (var oGenre in oGenres)
                {
                    var genreTranslation = await _genreTranslationRepository.GetGenreTranslation(oGenre.Id, lang);
                    genresDto.Add(new GenreDto
                    {
                        Id = oGenre.Id,
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

                if (productTranslation != null)
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = productTranslation.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

                else
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

            }

            var pageCount = Math.Ceiling(((double)products.Count() / (double)pageSize));

            if(page > pageCount)
            {
                page = (int)pageCount;
            }

            var productsResults = productsDto.Skip((page - 1) * pageSize).Take((int)pageSize).ToList();

            ProductsPageResultsDto productsPageResultsDto = new ProductsPageResultsDto()
            {
                Products = productsResults,
                CurrentPage = page,
                TotalPages = (int)pageCount
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(productsPageResultsDto);
        }

        //api/products/search/params
        [AllowAnonymous]
        [HttpGet("search/{searchText}", Name = "SearchProducts")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public async Task<IActionResult> SearchProducts(string searchText, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? lang = "en")
        {
            if (page < 1)
                return BadRequest();

            if (pageSize <= 0)
                return BadRequest();

            if (!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

            var products = await _productRepository.SearchProducts(searchText);

            var productsDto = new List<ProductDto>();

            foreach (var product in products)
            {
                var oGenres = await _genreRepository.GetGenresOfAProduct(product.Id);
                var genresDto = new List<GenreDto>();

                foreach (var genre in oGenres)
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

                if (productTranslation != null)
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = productTranslation.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

                else
                {
                    productsDto.Add(new ProductDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        ReleaseDate = product.ReleaseDate,
                        Stock = product.Stock,
                        Rating = product.Rating,
                        Format = formatDto,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Genres = genresDto
                    });
                }

            }

            var pageCount = Math.Ceiling(((double)products.Count() / (double)pageSize));

            if (page > pageCount)
            {
                page = (int)pageCount;
            }

            var productsResults = productsDto.Skip((page - 1) * pageSize).Take((int)pageSize).ToList();

            ProductsPageResultsDto productsPageResultsDto = new ProductsPageResultsDto()
            {
                Products = productsResults,
                CurrentPage = page,
                TotalPages = (int)pageCount
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(productsPageResultsDto);
        }

        //api/productId
        [AllowAnonymous]
        [HttpGet("{productId}", Name = "GetProduct")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDetailsDto>))]
        public async Task<IActionResult> GetProduct(int productId, [FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            var product = await _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            var productDto = new ProductDetailsDto()
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
            };

            return Ok(productDto);
        }

        //api/products/genres/productId?lang
        [AllowAnonymous]
        [HttpGet("genres/{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GenreDto>))]
        public async Task<IActionResult> GetGenresOfAProduct(int productId, [FromQuery] string? lang = "en")
        {
            if (!(await _languageRepository.LanguageExists(lang)))
            {
                ModelState.AddModelError("", $"Language doesn't exist");
                return StatusCode(500, ModelState);
            }

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

        //api/products?dirId=45&dirId=46&actId=1&genreId=1&genreId=2
        [Authorize]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductToCreateDto productToCreate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            var statusCode = await ValidateProduct(productToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Product product = new Product()
            {
                Title = productToCreate.OriginalTitle,
                ReleaseDate = productToCreate.ReleaseDate,
                Duration = productToCreate.Duration,
                Stock = productToCreate.Stock,
                Rating = productToCreate.Rating,
                Format = await _formatRepository.GetFormat(productToCreate.FormatId),
                PictureUrl = productToCreate.PictureUrl,
                Price = productToCreate.Price
            };

            var productTranslations = new List<ProductTranslation>();

            productTranslations.Add(new ProductTranslation
            {
                Language = await _languageRepository.GetLanguage(2),
                Title = productToCreate.EnglishTitle,
                Summary = productToCreate.EnglishSummary
            });

            productTranslations.Add(new ProductTranslation
            {
                Language = await _languageRepository.GetLanguage(1),
                Title = productToCreate.FrenchTitle,
                Summary = productToCreate.FrenchSummary
            });

            if (!await _productRepository.CreateProduct(product, productTranslations, productToCreate.DirectorsId, productToCreate.ActorsId, productToCreate.GenresId))
            {
                ModelState.AddModelError("", $"Something went wrong saving the product {productToCreate.OriginalTitle}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { productId = product.Id }, product);
        }

        //api/products/productId
        [Authorize]
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductToCreateDto productToUpdate)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }

            var statusCode = await ValidateProduct(productToUpdate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (productId != productToUpdate.Id)
                return BadRequest();

            if (!await _productRepository.ProductExists(productId))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            Product product = new Product()
            {
                Id = (int)productToUpdate.Id,
                Title = productToUpdate.OriginalTitle,
                ReleaseDate = productToUpdate.ReleaseDate,
                Duration = productToUpdate.Duration,
                Stock = productToUpdate.Stock,
                Rating = productToUpdate.Rating,
                Format = await _formatRepository.GetFormat(productToUpdate.FormatId),
                PictureUrl = productToUpdate.PictureUrl,
                Price = productToUpdate.Price
            };

            var productTranslations = new List<ProductTranslation>();

            productTranslations.Add(new ProductTranslation
            {
                Language = await _languageRepository.GetLanguage(2),
                Title = productToUpdate.EnglishTitle,
                Summary = productToUpdate.EnglishSummary
            });

            productTranslations.Add(new ProductTranslation
            {
                Language = await _languageRepository.GetLanguage(1),
                Title = productToUpdate.FrenchTitle,
                Summary = productToUpdate.FrenchSummary
            });

            if (!await _productRepository.UpdateProduct(product, productTranslations, productToUpdate.DirectorsId, productToUpdate.ActorsId, productToUpdate.GenresId))
            {
                ModelState.AddModelError("", $"Something went wrong updating the product {productToUpdate.OriginalTitle}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/products/productId
        [Authorize]
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var userRole = await _userRepository.GetUserRoleFromToken(HttpContext.Request.Headers["Authorization"]);

            if (userRole == null)
                return BadRequest();

            if (!userRole.Equals("admin"))
            {
                ModelState.AddModelError("", "User is not an admin");
                return StatusCode(401, ModelState);
            }
            if (!await _productRepository.ProductExists(productId))
            return NotFound();

            var productToDelete = await _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {productToDelete.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private async Task<StatusCodeResult> ValidateProduct(ProductToCreateDto product)
        {
            if (product == null || product.DirectorsId.Count() <= 0 || product.ActorsId.Count() <= 0 || product.GenresId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing product, director(s), actor(s), or genre(s)");
                return BadRequest();
            }

            if (!await _formatRepository.FormatExists(product.FormatId))
            {
                ModelState.AddModelError("", "Format doesn't exist");
                return StatusCode(404);
            }

            foreach (var id in product.DirectorsId)
            {
                if(!await _celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in product.ActorsId)
            {
                if (!await _celebrityRepository.CelebrityExists(id))
                {
                    ModelState.AddModelError("", "Celebrity Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in product.GenresId)
            {
                if (!await _genreRepository.GenreExists(id))
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
