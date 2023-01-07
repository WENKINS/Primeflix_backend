using Primeflix.Models;

namespace Primeflix.Services.OrderStatusService
{
    public interface IOrderStatusRepository
    {
        Task<ICollection<StatusTranslation>> GetOrderStatuses(int languageId);
        Task<StatusTranslation> GetStatus(int statusId, int languageId);
        Task<StatusTranslation> GetStatus(string statusName, int languageId);
        Task<Status> GetStatus(int statusId);
        Task<Status> GetStatus(string statusName);
        Task<ICollection<Order>> GetOrdersOfAStatus(int statusId);
        Task<bool> StatusExists(int statusId);
        Task<bool> IsDuplicate(string statusName);
        Task<bool> DeleteStatus(Status status);
        Task<bool> UpdateStatus(Status status);
        Task<bool> CreateStatus(Status status, List<StatusTranslation> translations);
        Task<bool> Save();
    }
}
