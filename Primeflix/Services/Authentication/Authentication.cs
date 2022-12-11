using Microsoft.IdentityModel.Tokens;
using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Primeflix.Services.Authentication
{
    public class Authentication : IAuthentication
    {
        private DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public Authentication(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = _databaseContext.Users.Where(u => u.Email.Trim().ToUpper().Equals(email.Trim().ToUpper())).FirstOrDefault();

            if(user == null)
            {
                return "User not found"; // SECURITY RISK!
            }

            if(user.Password == password)
            {
                return CreateToken(user);
            }

            return "Wrong password";
        }

        public async Task<bool> Register(User user)
        {
            _databaseContext.Users.Add(user);
            return await Save();
        }

        public async Task<bool> UserExists(string email)
        {
            return _databaseContext.Users
            .Where(u => u.Email.Trim().ToUpper().Equals(email.Trim().ToUpper()))
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
                .Where(u => u.Email.Trim().ToUpper().Equals(email.Trim().ToUpper()))
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

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
