using Primeflix.Models;

namespace Primeflix.Services.OrderStatusService
{
    public interface IOrderStatusRepository
    {
        Task<bool> StatusExists(int statusId);
        Task<bool> StatusExists(string statusName);
        Task<bool> IsDuplicate(string statusName);
        Task<ICollection<StatusTranslation>> GetOrderStatuses(string languageCode);
        Task<StatusTranslation> GetStatus(int statusId, string languageCode);
        Task<StatusTranslation> GetStatus(string statusName, string languageCode);
        Task<Status> GetStatus(int statusId);
        Task<Status> GetStatus(string statusName);
        Task<ICollection<Order>> GetOrdersOfAStatus(int statusId);
        Task<ICollection<Order>> GetOrdersOfAStatusAndUser(int statusId, int userId);
        Task<bool> CreateStatus(Status status, List<StatusTranslation> translations);
        Task<bool> UpdateStatus(Status status);
        Task<bool> DeleteStatus(Status status);
        Task<bool> Save();
    }
}
