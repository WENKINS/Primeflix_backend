using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.Authentication
{
    public interface IAuthentication
    {
        Task<bool> Register(User user, string password);
        Task<string> Login(string email, string password);
        Task<bool> UserExists(string email);
        Task<bool> UserExists(int userId);
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(string email);
        Task<User> GetUser(int userId);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> Save();
        Task<string> DecodeTokenForId(string token);
        Task<string?> DecodeTokenForRole(string token);
        Task<string> LoginWithFacebook(string accessToken);
    }
}
