using Primeflix.Models;

namespace Primeflix.Services.RoleService
{
    public interface IRoleRepository
    {
        Task<bool> RoleExists(int roleId);
        Task<bool> IsDuplicate(string name);
        Task<ICollection<Role>> GetRoles();
        Task<Role> GetRole(int roleId);
        Task<Role> GetRoleOfAUser(int userId);
        Task<bool> CreateRole(Role role);
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<bool> Save();
    }
}
