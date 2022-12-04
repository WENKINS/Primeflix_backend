using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class FormatRepository : IFormatRepository
    {
        private DatabaseContext _databaseContext;

        public FormatRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool CreateFormat(Format format)
        {
            _databaseContext.Add(format);
            return Save();
        }

        public bool DeleteFormat(Format format)
        {
            _databaseContext.Remove(format);
            return Save();
        }

        public bool FormatExists(int formatId)
        {
            return _databaseContext.Formats.Any(f => f.Id == formatId);
        }

        public Format GetFormat(int formatId)
        {
            return _databaseContext.Formats.Where(f => f.Id == formatId).FirstOrDefault();
        }

        public Format GetFormat(string formatName)
        {
            return _databaseContext.Formats.Where(f => f.Name == formatName).FirstOrDefault();
        }

        public Format GetFormatOfAProduct(int productId)
        {
            return _databaseContext.Products.Where(p => p.Id == productId).Select(p => p.Format).FirstOrDefault();
        }

        public ICollection<Format> GetFormats()
        {
            return _databaseContext.Formats.OrderBy(f => f.Name).ToList();
        }

        public ICollection<Product> GetProductsOfAFormat(int formatId)
        {
            return _databaseContext.Products.Where(p => p.FormatId == formatId).ToList();
        }

        public bool IsDuplicate(int formatId, string formatName)
        {
            var format = _databaseContext.Formats.Where(f => f.Name.Trim().ToUpper() == formatName.Trim().ToUpper() && f.Id != formatId).FirstOrDefault();
            return format == null ? false : true;
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        public bool UpdateFormat(Format format)
        {
            _databaseContext.Update(format);
            return Save();
        }
    }
}
