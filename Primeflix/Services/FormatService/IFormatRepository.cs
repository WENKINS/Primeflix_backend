using Primeflix.Models;

namespace Primeflix.Services.FormatService
{
    public interface IFormatRepository
    {
        Task<bool> FormatExists(int formatId);
        Task<bool> FormatExists(string formatName);
        Task<bool> IsDuplicate(string formatName);
        Task<ICollection<Format>> GetFormats();
        Task<Format> GetFormat(int formatId);
        Task<Format> GetFormat(string formatName);
        Task<Format> GetFormatOfAProduct(int productId);
        Task<ICollection<Product>> GetProductsOfAFormat(int formatId);
        Task<bool> CreateFormat(Format format);
        Task<bool> UpdateFormat(Format format);
        Task<bool> DeleteFormat(Format format);
        Task<bool> Save();
    }
}
