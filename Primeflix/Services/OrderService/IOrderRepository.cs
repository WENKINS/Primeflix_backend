using Primeflix.Models;

namespace Primeflix.Services.OrderService
{
    public interface IOrderRepository
    {
        Task<bool> PlaceOrder(int cartId);
        Task<ICollection<Order>> GetOrders();
        Task<Order> GetOrder(int orderId);
        Task<ICollection<Order>> GetOrdersOfAUser(int userId);
        Task<bool> OrderExists(int orderId);
        Task<bool> IsDuplicate(int orderId, int userId);
        Task<bool> OrderProducts(int cartId);
        Task<bool> DeleteCart(Order order);
        Task<bool> Save();
    }
}
