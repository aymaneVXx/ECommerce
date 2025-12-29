using ECommerce.Application.Interfaces.Identity;
using ECommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public class RoleManagement : IRoleManagement
{
    private readonly UserManager<AppUser> _userManager;

    public RoleManagement(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> AddUserToRole(AppUser user, string roleName)
    {
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<string?> GetUserRole(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }
}
