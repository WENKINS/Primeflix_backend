using Primeflix.Models;

namespace Primeflix.Services.OrderService
{
    public interface IOrderRepository
    {
        Task<bool> PlaceOrder(int cartId);
        Task<ICollection<Order>> GetOrders();
        Task<Order> GetOrder(int orderId);
        Task<ICollection<OrderDetails>> GetOrderDetails(int orderId);
        Task<ICollection<Order>> GetOrdersOfAUser(int userId);
        Task<bool> OrderExists(int orderId);
        Task<bool> IsDuplicate(int orderId, int userId);
        Task<bool> DeleteOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> Save();
    }
}
