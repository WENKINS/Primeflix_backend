using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.FormatService
{
    public class FormatRepository : IFormatRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FormatRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> FormatExists(int formatId)
        {
            return _databaseContext.Formats
                .Where(f => f.Id == formatId)
                .Any();
        }

        public async Task<bool> FormatExists(string formatName)
        {
            return _databaseContext.Formats
                .Where(f => f.Name.Trim().ToUpper().Equals(formatName.Trim().ToUpper()))
                .Any();
        }

        public async Task<bool> IsDuplicate(string formatName)
        {
            var format = _databaseContext.Formats
                .Where(f => f.Name.Trim().ToUpper() == formatName.Trim().ToUpper())
                .FirstOrDefault();
            return format == null ? false : true;
        }

        public async Task<ICollection<Format>> GetFormats()
        {
            return _databaseContext.Formats
                .OrderBy(f => f.Name)
                .ToList();
        }

        public async Task<Format> GetFormat(int formatId)
        {
            return _databaseContext.Formats
                .Where(f => f.Id == formatId)
                .FirstOrDefault();
        }

        public async Task<Format> GetFormat(string formatName)
        {
            return _databaseContext.Formats
                .Where(f => f.Name.Trim().ToUpper().Equals(formatName.Trim().ToUpper()))
                .FirstOrDefault();
        }

        public async Task<Format> GetFormatOfAProduct(int productId)
        {
            return _databaseContext.Products
                .Where(p => p.Id == productId)
                .Select(p => p.Format)
                .FirstOrDefault();
        }

        public async Task<ICollection<Product>> GetProductsOfAFormat(int formatId)
        {
            return _databaseContext.Products
                .Where(p => p.FormatId == formatId)
                .ToList();
        }

        public async Task<bool> CreateFormat(Format format)
        {
            _databaseContext.Add(format);
            return await Save();
        }

        public async Task<bool> UpdateFormat(Format format)
        {
            _databaseContext.Update(format);
            return await Save();
        }

        public async Task<bool> DeleteFormat(Format format)
        {
            _databaseContext.Remove(format);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
