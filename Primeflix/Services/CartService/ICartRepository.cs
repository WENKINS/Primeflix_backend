using Primeflix.Models;

namespace Primeflix.Services.CartService
{
    public interface ICartRepository
    {
        Task<bool> CartExists(int cartId);
        Task<bool> CartOfAUserExists(int userId);
        Task<ICollection<Cart>> GetCarts();
        Task<Cart> GetCart(int cartId);
        Task<Cart> GetCartOfAUser(int userId);
        Task<ICollection<CartProduct>> GetProductsOfACart(int cartId);
        Task<bool> AddProductToCart(int userId, int productId, int quantity);
        Task<bool> EmptyCart(int userId);
        Task<bool> CreateCart(int userId);
        Task<bool> DeleteCart(Cart cart);
        Task<bool> Save();
    }
}
