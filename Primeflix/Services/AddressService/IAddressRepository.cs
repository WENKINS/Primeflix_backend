namespace Primeflix.Services.Address
{
    public interface IAddressRepository
    {
        Task<bool> AddressOfAUserExists(int userId);
        Task<Models.Address> GetAddressOfAUser(int userId);
        Task<bool> CreateAddress(Models.Address address);
        Task<bool> UpdateAddress(Models.Address address);
        Task<bool> DeleteAddress(Models.Address address);
        Task<bool> Save();
    }
}
