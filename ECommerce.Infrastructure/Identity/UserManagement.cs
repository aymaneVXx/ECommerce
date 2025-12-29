using System.Security.Claims;
using ECommerce.Application.Interfaces.Identity;
using ECommerce.Domain.Identity;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity;

public class UserManagement : IUserManagement
{
    private readonly IRoleManagement _roleManagement;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public UserManagement(
        IRoleManagement roleManagement,
        UserManager<AppUser> userManager,
        AppDbContext context)
    {
        _roleManagement = roleManagement;
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> CreateUser(AppUser user, string password)
    {
        var existingUser = await GetUserByEmail(user.Email!);
        if (existingUser is not null) return false;

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<bool> LoginUser(AppUser user, string password)
    {
        var foundUser = await GetUserByEmail(user.Email!);
        if (foundUser is null) return false;

        var role = await _roleManagement.GetUserRole(foundUser.Email!);
        if (string.IsNullOrWhiteSpace(role)) return false;

        return await _userManager.CheckPasswordAsync(foundUser, password);
    }

    public async Task<AppUser?> GetUserByEmail(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<AppUser> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user!;
    }

    public async Task<IEnumerable<AppUser>?> GetAllUsers()
        => await _context.Users.ToListAsync();

    public async Task<int> RemoveUserByEmail(string email)
    {
        var user = await GetUserByEmail(email);
        if (user is null) return 0;

        _context.Users.Remove(user);
        return await _context.SaveChangesAsync();
    }

    public async Task<List<Claim>> GetUserClaims(string email)
    {
        var user = await GetUserByEmail(email);
        if (user is null) return new List<Claim>();

        var role = await _roleManagement.GetUserRole(user.Email!);

        return new List<Claim>
        {
            new Claim("FullName", user.FullName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, role ?? string.Empty)
        };
    }
}
