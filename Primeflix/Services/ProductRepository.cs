using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class ProductRepository : IProductRepository
    {
        private DatabaseContext _databaseContext;

        public ProductRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool CreateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId)
        {
            var directors = _databaseContext.Celebrities.Where(c => directorsId.Contains(c.Id)).ToList();
            var actors = _databaseContext.Celebrities.Where(c => actorsId.Contains(c.Id)).ToList();
            var genres = _databaseContext.Genres.Where(g => genresId.Contains(g.Id)).ToList();

            foreach(var director in directors)
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

            _databaseContext.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _databaseContext.Remove(product);
            return Save();
        }

        public ICollection<Product> FilterResults(bool recentlyAdded, string format, List<int> genresId)
        {
            var products = new List<Product>();

            if(genresId != null)
            {
                foreach (var genreId in genresId)
                {
                    if (format == "All")
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Take(3)
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

                    if (format == "film")
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.Format == format)
                                .Take(3)
                                .ToList());
                        }

                        else
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.Format == format)
                                .ToList());
                        }
                    }

                    if (format == "serie")
                    {
                        if (recentlyAdded)
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.Format == format)
                                .Take(3)
                                .ToList());
                        }

                        else
                        {
                            products.AddRange(_databaseContext.ProductsGenres
                                .Where(g => g.GenreId == genreId)
                                .Select(p => p.Product)
                                .OrderBy(p => p.Title)
                                .Where(p => p.Format == format)
                                .ToList());
                        }
                    }
                }
            }

            else
            {
                if (format == "All")
                {
                    if (recentlyAdded)
                    {
                        products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.DirectorsMovies)
                            .Include(p => p.ActorsMovies)
                            .Include(p => p.ProductGenre)
                            .Take(3)
                            .ToList();
                        return products;
                    }

                    products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.DirectorsMovies)
                        .Include(p => p.ActorsMovies)
                        .Include(p => p.ProductGenre)
                        .ToList();
                    return products;
                }

                if (format == "film")
                {
                    if (recentlyAdded)
                    {
                        products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.DirectorsMovies)
                            .Include(p => p.ActorsMovies)
                            .Include(p => p.ProductGenre)
                            .Where(p => p.Format == format)
                            .Take(3)
                            .ToList();

                        return products;
                    }

                    products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.DirectorsMovies)
                        .Include(p => p.ActorsMovies)
                        .Include(p => p.ProductGenre)
                        .Where(p => p.Format == format)
                        .ToList();
                    return products;
                }

                if (format == "serie")
                {
                    if (recentlyAdded)
                    {
                        products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                            .Include(p => p.DirectorsMovies)
                            .Include(p => p.ActorsMovies)
                            .Include(p => p.ProductGenre)
                            .Where(p => p.Format == format)
                            .Take(3)
                            .ToList();
                        return products;
                    }

                    products = (List<Product>)_databaseContext.Products.OrderBy(p => p.Title)
                        .Include(p => p.DirectorsMovies)
                        .Include(p => p.ActorsMovies)
                        .Include(p => p.ProductGenre)
                        .Where(p => p.Format == format)
                        .ToList();
                    return products;
                }
            }

            return products;
        }

        public Product GetProduct(int productId)
        {
            return _databaseContext.Products.Where(p => p.Id == productId).FirstOrDefault();
        }

        public Product GetProduct(string title)
        {
            return _databaseContext.Products.Where(p => p.Title == title).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _databaseContext.Products.OrderBy(p => p.Title)
                .Include(p => p.DirectorsMovies)
                .Include(p => p.ActorsMovies)
                .Include(p => p.ProductGenre)
                .ToList();
        }

        public bool IsDuplicate(int productId, string productTitle)
        {
            var product = _databaseContext.Products.Where(p => p.Title.Trim().ToUpper() == productTitle.Trim().ToUpper() && p.Id != productId).FirstOrDefault();
            if (product != null)
            {
                // IMPLEMENT DIRECTOR NAME COMPARISON (ADD METHOD MAYBE?)
                return true;
            }
            return false;
        }

        public bool ProductExists(int productId)
        {
            return _databaseContext.Products.Any(p => p.Id == productId);
        }

        public bool ProductExists(string title)
        {
            return _databaseContext.Products.Any(p => p.Title == title);
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        public bool UpdateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId)
        {
            var directors = _databaseContext.Celebrities.Where(c => directorsId.Contains(c.Id)).ToList();
            var actors = _databaseContext.Celebrities.Where(c => actorsId.Contains(c.Id)).ToList();
            var genres = _databaseContext.Genres.Where(g => genresId.Contains(g.Id)).ToList();

            var productDirectorsToDelete = _databaseContext.Directors.Where(d => d.ProductId == product.Id);
            var productActorsToDelete = _databaseContext.Actors.Where(a => a.ProductId == product.Id);
            var productGenresToDelete = _databaseContext.ProductsGenres.Where(g => g.ProductId == product.Id);

            _databaseContext.RemoveRange(productDirectorsToDelete);
            _databaseContext.RemoveRange(productActorsToDelete);
            _databaseContext.RemoveRange(productGenresToDelete);

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

            _databaseContext.Update(product);
            return Save();
        }
    }
}
