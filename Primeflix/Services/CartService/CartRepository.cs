using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.CartService
{
    public class CartRepository : ICartRepository
    {
        private DatabaseContext _databaseContext;
        public CartRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AddProductToCart(int userId, int productId, int quantity)
        {
            var product = _databaseContext.Products.Where(p => p.Id == productId).FirstOrDefault();
            var user = _databaseContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            var cart = await GetCartOfAUser(userId);

            var cartProduct = new CartProduct()
            {
                Cart = cart,
                Product = product
            };
            _databaseContext.Add(cartProduct);

            return await Save();
        }

        public async Task<bool> CartExists(int cartId)
        {
            return _databaseContext.Carts.Any(c => c.Id == cartId);
        }

        public async Task<bool> CartOfAUserExists(int userId)
        {
            return _databaseContext.Carts.Any(c => c.UserId == userId);
        }

        public async Task<bool> CreateCart(int userId)
        {
            var user = _databaseContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            var newCart = new Cart()
            {
                User = user
            };
            _databaseContext.Add(newCart);
            return await Save();
        }

        public async Task<bool> DeleteCart(Cart cart)
        {
            _databaseContext.Remove(cart);
            return await Save();
        }

        public async Task<Cart> GetCart(int cartId)
        {
            return _databaseContext.Carts.Where(c => c.Id == cartId).FirstOrDefault();
        }

        public async Task<Cart> GetCartOfAUser(int userId)
        {
            return _databaseContext.Carts.Where(c => c.UserId == userId).FirstOrDefault();
        }

        public async Task<ICollection<Cart>> GetCarts()
        {
            return _databaseContext.Carts.ToList();
        }

        public async Task<ICollection<Product>> GetProductsOfACart(int cartId)
        {
            return _databaseContext.CartProducts.Where(cp => cp.CartId == cartId).Select(cp => cp.Product).ToList();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        public async Task<bool> UpdateCart(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
