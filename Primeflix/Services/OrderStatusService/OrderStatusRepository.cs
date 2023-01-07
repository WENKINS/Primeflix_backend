using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.OrderStatusService
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrderStatusRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> CreateStatus(Status status, List<StatusTranslation> translations)
        {
            _databaseContext.Add(status);
            foreach(var translation in translations)
            {
                _databaseContext.Add(translation);
            }

            return await Save();
        }

        public async Task<bool> DeleteStatus(Status status)
        {
            _databaseContext.Remove(status);
            return await Save();
        }

        public async Task<ICollection<Order>> GetOrdersOfAStatus(int statusId)
        {
            return _databaseContext.Orders.Where(o => o.StatusId == statusId).ToList();
        }

        public async Task<ICollection<StatusTranslation>> GetOrderStatuses(int languageId)
        {
            return _databaseContext.StatusTranslations.Where(st => st.LanguageId == languageId).ToList();
        }

        public async Task<StatusTranslation> GetStatus(int statusId, int languageId)
        {
            return _databaseContext.StatusTranslations.Where(s => s.Id == statusId).FirstOrDefault();
        }

        public async Task<StatusTranslation> GetStatus(string statusName, int languageId)
        {
            return _databaseContext.StatusTranslations.Where(st => st.Name.Equals(statusName) && st.LanguageId == languageId).FirstOrDefault();
        }

        public async Task<Status> GetStatus(int statusId)
        {
            return _databaseContext.Statuses.Where(s => s.Id == statusId).FirstOrDefault();
        }

        public async Task<Status> GetStatus(string statusName)
        {
            return _databaseContext.StatusTranslations.Where(st => st.Name.Equals(statusName)).Select(st => st.Status).FirstOrDefault();
        }

        public async Task<bool> IsDuplicate(string statusName)
        {
            return _databaseContext.Statuses.Where(s => s.Name.Equals(statusName)).Any();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        public async Task<bool> StatusExists(int statusId)
        {
            return _databaseContext.Statuses.Where(s => s.Id == statusId).Any();
        }

        public async Task<bool> UpdateStatus(Status status)
        {
            _databaseContext.Statuses.Update(status);
            return await Save();
        }
    }
}
