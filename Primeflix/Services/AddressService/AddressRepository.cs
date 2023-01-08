using Primeflix.Data;

namespace Primeflix.Services.Address
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AddressRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AddressOfAUserExists(int userId)
        {
            return _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Address)
                .Any();
        }

        public async Task<Models.Address> GetAddressOfAUser(int userId)
        {
            return _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Address)
                .FirstOrDefault();
        }

        public async Task<bool> CreateAddress(Models.Address address)
        {
            _databaseContext.Add(address);
            return await Save();
        }

        public async Task<bool> UpdateAddress(Models.Address address)
        {
            _databaseContext.Update(address);

            return await Save();
        }

        public async Task<bool> DeleteAddress(Models.Address address)
        {
            _databaseContext.Remove(address);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
