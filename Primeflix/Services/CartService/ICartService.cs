using Primeflix.Models;

namespace Primeflix.Services.CartService
{
    public interface ICartService
    {
        Task<ICollection<Cart>> GetCarts();
        Task<Cart> GetCart(int cartId);
        Task<Cart> GetCartOfAUser(int userId);
        Task<bool> CartExists(int cartId);
        Task<bool> CartOfAUserExists(int userId);
        Task<bool> IsDuplicate(int cartId, int userId);
        Task<bool> AddProductToCart(int userId, int productId, int quantity);
        Task<bool> UpdateCart(int userId, int productId, int quantity);
        Task<bool> DeleteCart(Cart cart);
        Task<bool> Save();
    }
}
