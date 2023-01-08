using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.RoleService
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseContext _databaseContext;

        public RoleRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public async Task<bool> RoleExists(int roleId)
        {
            return _databaseContext.Roles.Any(r => r.Id == roleId);
        }

        public async Task<bool> IsDuplicate(string name)
        {
            var role = _databaseContext.Roles
                .Where(r => r.Name.Trim().ToUpper() == name.Trim().ToUpper())
                .FirstOrDefault();
            return role == null ? false : true;
        }

        public async Task<ICollection<Role>> GetRoles()
        {
            return _databaseContext.Roles
                .OrderBy(r => r.Name)
                .ToList();
        }

        public async Task<Role> GetRole(int roleId)
        {
            return _databaseContext.Roles
                .Where(r => r.Id == roleId)
                .FirstOrDefault();
        }

        public async Task<Role> GetRoleOfAUser(int userId)
        {
            return _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Role)
                .FirstOrDefault();
        }

        public async Task<bool> CreateRole(Role role)
        {
            _databaseContext.Add(role);
            return await Save();
        }

        public async Task<bool> UpdateRole(Role role)
        {
            _databaseContext.Update(role);
            return await Save();
        }

        public async Task<bool> DeleteRole(Role role)
        {
            _databaseContext.Remove(role);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
