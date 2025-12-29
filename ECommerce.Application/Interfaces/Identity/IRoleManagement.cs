using ECommerce.Domain.Identity;

namespace ECommerce.Application.Interfaces.Identity;

public interface IRoleManagement
{
    Task<string?> GetUserRole(string email);
    Task<bool> AddUserToRole(AppUser user, string roleName);
}
