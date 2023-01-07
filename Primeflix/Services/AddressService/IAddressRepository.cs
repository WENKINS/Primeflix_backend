namespace Primeflix.Services.Address
{
    public interface IAddressRepository
    {
        Task<Models.Address> GetAddressOfAUser(int userId);
        Task<bool> UpdateAddress(Models.Address address);
        Task<bool> DeleteAddress(Models.Address address);

        Task<bool> CreateAddress(Models.Address address);
        Task<bool> Save();
        Task<bool> AddressOfAUserExists(int userId);
    }
}
