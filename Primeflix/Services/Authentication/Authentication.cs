using Microsoft.IdentityModel.Tokens;
using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services.RoleService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Primeflix.Services.Authentication
{
    public class Authentication : IAuthentication
    {
        private DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;
        private IRoleRepository _roleRepository;

        public Authentication(DatabaseContext databaseContext, IConfiguration configuration, IRoleRepository roleRepository)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = _databaseContext.Users.Where(u => u.Email.Trim().ToUpper().Equals(email.Trim().ToUpper())).FirstOrDefault();

            if(user == null)
            {
                return "User not found"; // SECURITY RISK!
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return "Wrong password";
            }

            return await CreateToken(user);
        }

        public async Task<bool> Register(User user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if(user.Language == null)
            {
                user.Language = _databaseContext.Languages.Where(l => l.Name == "English").FirstOrDefault();
            }

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

        public async Task<IEnumerable<User>> GetUsers()
        {
            return _databaseContext.Users.OrderBy(u => u.Email).ToList();
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

        private async Task<string> CreateToken(User user)
        {
            var role = await _roleRepository.GetRoleOfAUser(user.Id);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name)
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

        public async Task<string> DecodeTokenForId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokens = jsonToken as JwtSecurityToken;
            var claim = tokens.Claims.FirstOrDefault(c => c.Type == "nameid");

            if (claim != null)
            {
                var role = claim.Value;
                return role;
            }

            return "Could not decode JWT";
        }

        public async Task<string> DecodeTokenForRole(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokens = jsonToken as JwtSecurityToken;
            var claim = tokens.Claims.FirstOrDefault(c => c.Type == "role");

            if (claim != null)
            {
                var id = claim.Value;
                return id;
            }

            return "Could not decode JWT";
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}
