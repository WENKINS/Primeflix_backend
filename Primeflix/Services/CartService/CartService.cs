using Primeflix.Models;

namespace Primeflix.Services.CartService
{
    public class CartService : ICartService
    {
        public Task<bool> AddProductToCart(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CartExists(int cartId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CartOfAUserExists(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCart(Cart cart)
        {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCart(int cartId)
        {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCartOfAUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Cart>> GetCarts()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDuplicate(int cartId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCart(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
