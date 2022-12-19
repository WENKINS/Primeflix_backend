using Primeflix.Models;

namespace Primeflix.Services.OrderService
{
    public class OrderService : IOrderService
    {
        public Task<bool> DeleteCart(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Order>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Order>> GetOrdersOfAUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDuplicate(int orderId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OrderExists(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OrderProducts(int cartId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }
    }
}
