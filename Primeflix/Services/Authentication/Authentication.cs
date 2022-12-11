using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.Authentication
{
    public class Authentication : IAuthentication
    {
        private DatabaseContext _databaseContext;

        public Authentication(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<string> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Register(User user)
        {
            _databaseContext.Users.Add(user);
            return await Save();
        }

        public async Task<bool> UserExists(string email)
        {
            return _databaseContext.Users
            .Where(u => u.Email.Trim().ToUpper() == email.Trim().ToUpper())
            .Any();
        }

        public async Task<bool> UserExists(int userId)
        {
            return _databaseContext.Users
            .Where(u => u.Id == userId)
            .Any();
        }

        public async Task<User> GetUser(string email)
        {
            return _databaseContext.Users
                .Where(u => u.Email.Trim().ToUpper() == email.Trim().ToUpper())
                .FirstOrDefault();
        }

        public async Task<User> GetUser(int userId)
        {
            return _databaseContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
