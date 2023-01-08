using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.GenreService
{
    public class GenreRepository : IGenreRepository
    {
        private readonly DatabaseContext _databaseContext;

        public GenreRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> GenreExists(int genreId)
        {
            return _databaseContext.Genres.Any(g => g.Id == genreId);
        }

        public async Task<bool> GenreExists(string genreName)
        {
            return _databaseContext.GenresTranslations
            .Where(gt => gt.Translation.Trim().ToUpper() == genreName)
            .Any();
        }

        public async Task<bool> IsDuplicate(string genreName)
        {
            var genre = _databaseContext.GenresTranslations.Where(gt => gt.Translation.Trim().ToUpper() == genreName.Trim().ToUpper()).FirstOrDefault();
            return genre == null ? false : true;
        }

        public async Task<ICollection<Genre>> GetGenres(string languageCode)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.Language.Code == languageCode)
                .Select(g => g.Genre)
                .ToList();
        }

        public async Task<Genre> GetGenre(int genreId)
        {
            return _databaseContext.Genres.Where(g => g.Id == genreId).FirstOrDefault();
        }

        public async Task<Genre> GetGenre(string genreName)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.Translation.Equals(genreName))
                .Select(gt => gt.Genre)
                .FirstOrDefault();
        }

        public async Task<ICollection<Genre>> GetGenresOfAProduct(int productId)
        {
            return _databaseContext.ProductsGenres.Where(p => p.ProductId == productId).Select(g => g.Genre).ToList();
        }

        public async Task<ICollection<Product>> GetProductsOfAGenre(int genreId)
        {
            return _databaseContext.ProductsGenres.Where(g => g.GenreId == genreId).Select(p => p.Product).ToList();
        }

        public async Task<bool> CreateGenre(NewGenreDto genre)
        {
            var genreToCreate = new Genre()
            {
                Name = genre.EnglishName
            };

            _databaseContext.Add(genreToCreate);

            var frenchTranslation = new GenreTranslation()
            {
                Genre = genreToCreate,
                Language = (Language)_databaseContext.Languages.Where(l => l.Name == "French"),
                Translation = genre.FrenchName
            };

            _databaseContext.Add(frenchTranslation);

            var englishTranslation = new GenreTranslation()
            {
                Genre = genreToCreate,
                Language = (Language)_databaseContext.Languages.Where(l => l.Name == "English"),
                Translation = genre.EnglishName
            };

            _databaseContext.Add(englishTranslation);

            return await Save();
        }

        public async Task<bool> UpdateGenre(NewGenreDto genre)
        {
            var frenchTranslationToDelete = _databaseContext.GenresTranslations.Where(gt => gt.Translation.Equals(genre.FrenchName)).FirstOrDefault();
            var englishTranslationToDelete = _databaseContext.GenresTranslations.Where(gt => gt.Translation.Equals(genre.EnglishName)).FirstOrDefault();

            _databaseContext.Remove(frenchTranslationToDelete);
            _databaseContext.Remove(englishTranslationToDelete);

            await Save();

            var genreToUpdate = _databaseContext.Genres.Where(g => g.Id == genre.Id).FirstOrDefault();
            genreToUpdate.Name = genre.EnglishName;

            var frenchTranslation = new GenreTranslation()
            {
                Genre = genreToUpdate,
                Language = (Language)_databaseContext.Languages.Where(l => l.Name == "French"),
                Translation = genre.FrenchName
            };

            _databaseContext.Add(frenchTranslation);

            var englishTranslation = new GenreTranslation()
            {
                Genre = genreToUpdate,
                Language = (Language)_databaseContext.Languages.Where(l => l.Name == "English"),
                Translation = genre.EnglishName
            };

            _databaseContext.Add(englishTranslation);

            _databaseContext.Update(genre);
            return await Save();
        }

        public async Task<bool> DeleteGenre(Genre genre)
        {
            _databaseContext.Remove(genre);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
