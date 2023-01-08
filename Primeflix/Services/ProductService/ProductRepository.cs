using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.ProductService
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> ProductExists(int productId)
        {
            return _databaseContext.Products.Any(p => p.Id == productId);
        }

        public async Task<bool> ProductExists(string title)
        {
            return _databaseContext.Products.Any(p => p.Title == title);
        }

        public async Task<bool> IsDuplicate(int productId, string productTitle)
        {
            var product = _databaseContext.Products.Where(p => p.Title.Trim().ToUpper() == productTitle.Trim().ToUpper() && p.Id != productId).FirstOrDefault();
            if (product != null)
            {
                // IMPLEMENT DIRECTOR NAME COMPARISON (ADD METHOD MAYBE?)
                return true;
            }
            return false;
        }

        public async Task<ICollection<Product>> GetProducts()
        {
            return _databaseContext.Products.OrderBy(p => p.Title)
                .Include(p => p.DirectorsMovies)
                .Include(p => p.ActorsMovies)
                .Include(p => p.ProductGenre)
                .ToList();
        }

        public async Task<Product> GetProduct(int productId)
        {
            return _databaseContext.Products.Where(p => p.Id == productId).FirstOrDefault();
        }

        public async Task<Product> GetProduct(string title)
        {
            return _databaseContext.Products.Where(p => p.Title == title).FirstOrDefault();
        }

        public async Task<ICollection<Product>> FilterResults(bool recentlyAdded, int formatId, List<int> genresId)
        {
            var products = new List<Product>();

            if(genresId != null)
            {
                foreach (var genreId in genresId)
                {
                    if (formatId == 0)
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .OrderByDescending(p => p.CreatedAt)
                                .Take(20)
                                .ToList());
                        }

                        else
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .ToList());
                        }
                    }

                    if (formatId == 1)
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.FormatId == formatId)
                                .OrderByDescending(p => p.CreatedAt)
                                .Take(20)
                                .ToList());
                        }

                        else
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.FormatId == formatId)
                                .ToList());
                        }
                    }
                    
                    if (formatId == 2)
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.FormatId == formatId)
                                .OrderByDescending(p => p.CreatedAt)
                                .Take(20)
                                .ToList());
                        }

                        else
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.FormatId == formatId)
                                .ToList());
                        }
                    }
                }
            }

            else
            {
                if (formatId == 0)
                {
                    if (recentlyAdded)
                    {
                        products = _databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.ProductGenre)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(20)
                            .ToList();
                        return products;
                    }

                    products = _databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.ProductGenre)
                        .Include(p => p.Format)
                        .Distinct()
                        .ToList();
                }

                if (formatId == 1)
                {
                    if (recentlyAdded)
                    {
                        products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.ProductGenre)
                            .Where(p => p.FormatId == formatId)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(20)
                            .ToList();

                        return products;
                    }

                    products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.ProductGenre)
                        .Where(p => p.FormatId == formatId)
                        .ToList();
                    return products;
                }

                if (formatId == 2)
                {
                    if (recentlyAdded)
                    {
                        products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.ProductGenre)
                            .Where(p => p.FormatId == formatId)
                            .Take(3)
                            .ToList();
                        return products;
                    }

                    products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.ProductGenre)
                        .Where(p => p.FormatId == formatId)
                        .ToList();
                    return products;
                }
            }

            return products;
        }

        public async Task<ICollection<Product>> SearchProducts(string searchText)
        {
            return _databaseContext.ProductsTranslations
                .Where(pt => pt.Title.ToLower().Contains(searchText.ToLower())
                ||
                pt.Summary.ToLower().Contains(searchText.ToLower())).Select(pt => pt.Product)
                .Distinct()
                .ToList();
        }

        public async Task<bool> CreateProduct(Product product, List<ProductTranslation> translations, List<int> directorsId, List<int> actorsId, List<int> genresId)
        {
            var directors = _databaseContext.Celebrities.Where(c => directorsId.Contains(c.Id)).ToList();
            var actors = _databaseContext.Celebrities.Where(c => actorsId.Contains(c.Id)).ToList();
            var genres = _databaseContext.Genres.Where(g => genresId.Contains(g.Id)).ToList();

            foreach (var director in directors)
            {
                var productDirector = new Director()
                {
                    Celebrity = director,
                    Product = product
                };
                _databaseContext.Add(productDirector);
            }

            foreach (var actor in actors)
            {
                var productActor = new Actor()
                {
                    Celebrity = actor,
                    Product = product
                };
                _databaseContext.Add(productActor);
            }

            foreach (var genre in genres)
            {
                var productGenre = new ProductGenre()
                {
                    Genre = genre,
                    Product = product
                };
                _databaseContext.Add(productGenre);
            }

            foreach (var translation in translations)
            {
                translation.Product = product;
                _databaseContext.Add(translation);
            }

            _databaseContext.Add(product);

            return await Save();
        }

        public async Task<bool> UpdateProduct(Product product, List<ProductTranslation> translations, List<int> directorsId, List<int> actorsId, List<int> genresId)
        {
            var directors = _databaseContext.Celebrities.Where(c => directorsId.Contains(c.Id)).ToList();
            var actors = _databaseContext.Celebrities.Where(c => actorsId.Contains(c.Id)).ToList();
            var genres = _databaseContext.Genres.Where(g => genresId.Contains(g.Id)).ToList();

            var productDirectorsToDelete = _databaseContext.Directors.Where(d => d.ProductId == product.Id);
            var productActorsToDelete = _databaseContext.Actors.Where(a => a.ProductId == product.Id);
            var productGenresToDelete = _databaseContext.ProductsGenres.Where(g => g.ProductId == product.Id);
            var translationsToDelete = _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == product.Id);

            _databaseContext.RemoveRange(productDirectorsToDelete);
            _databaseContext.RemoveRange(productActorsToDelete);
            _databaseContext.RemoveRange(productGenresToDelete);
            _databaseContext.RemoveRange(translationsToDelete);

            await Save();

            foreach (var director in directors)
            {
                var productDirector = new Director()
                {
                    Celebrity = director,
                    Product = product
                };
                _databaseContext.Add(productDirector);
            }

            foreach (var actor in actors)
            {
                var productActor = new Actor()
                {
                    Celebrity = actor,
                    Product = product
                };
                _databaseContext.Add(productActor);
            }

            foreach (var genre in genres)
            {
                var productGenre = new ProductGenre()
                {
                    Genre = genre,
                    Product = product
                };
                _databaseContext.Add(productGenre);
            }

            foreach (var translation in translations)
            {
                translation.Product = product;
                _databaseContext.Add(translation);
            }

            _databaseContext.Update(product);

            return await Save();
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            _databaseContext.Remove(product);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
