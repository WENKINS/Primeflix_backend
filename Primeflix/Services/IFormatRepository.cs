using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IFormatRepository
    {
        ICollection<Format> GetFormats();
        Format GetFormat(int formatId);
        Format GetFormat(string formatName);
        Format GetFormatOfAProduct(int productId);
        ICollection<Product> GetProductsOfAFormat(int formatId);
        bool FormatExists(int formatId);
        bool IsDuplicate(int formatId, string formatName);
        bool CreateFormat(Format format);
        bool UpdateFormat(Format format);
        bool DeleteFormat(Format format);
        bool Save();
    }
}
