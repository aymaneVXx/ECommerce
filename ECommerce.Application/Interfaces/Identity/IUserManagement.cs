using System.Security.Claims;
using ECommerce.Domain.Identity;

namespace ECommerce.Application.Interfaces.Identity;

public interface IUserManagement
{
    Task<bool> CreateUser(AppUser user, string password);
    Task<bool> LoginUser(AppUser user, string password);

    Task<AppUser?> GetUserByEmail(string email);
    Task<AppUser> GetUserById(string id);

    Task<IEnumerable<AppUser>?> GetAllUsers();
    Task<int> RemoveUserByEmail(string email);

    Task<List<Claim>> GetUserClaims(string email);
}
