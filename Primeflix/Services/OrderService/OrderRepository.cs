using Primeflix.Data;
using Primeflix.Models;
using Primeflix.Services.CartService;

namespace Primeflix.Services.OrderService
{
    public class OrderRepository : IOrderRepository
    {
        private DatabaseContext _databaseContext;
        private ICartRepository _cartRepository;

        public OrderRepository(DatabaseContext databaseContext, ICartRepository cartRepository)
        {
            _databaseContext = databaseContext;
            _cartRepository = cartRepository;
        }

        public async Task<bool> DeleteCart(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Order>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Order>> GetOrdersOfAUser(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsDuplicate(int orderId, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> OrderExists(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> OrderProducts(int cartId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PlaceOrder(int cartId)
        {
            var user = _databaseContext.Carts.Where(c => c.Id == cartId).Select(c => c.User).FirstOrDefault();
            var cartItems = await _cartRepository.GetProductsOfACart(cartId);
            float totalPrice = 0;
            var orderDetails = new List<OrderDetails>();

            foreach (var cartItem in cartItems)
            {
                var product = _databaseContext.Products.Where(p => p.Id == cartItem.ProductId).FirstOrDefault();
                totalPrice = totalPrice + (float)(cartItem.Quantity * product.Price);
            }

            var order = new Order()
            {
                User = user,
                Date = DateTime.Now,
                Total = totalPrice
            };

            _databaseContext.Add(order);
            await Save();

            order = _databaseContext.Orders.Where(o => o.UserId == user.Id).FirstOrDefault();

            foreach (var cartItem in cartItems)
            {
                var ordertItem = new OrderDetails()
                {
                    Order = order,
                    Product = cartItem.Product,
                    Quantity = cartItem.Quantity
                };

                _databaseContext.Add(ordertItem);
            }

            _databaseContext.CartProducts.RemoveRange(cartItems);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
