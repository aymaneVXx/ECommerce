using System.Security.Claims;

namespace ECommerce.Application.Interfaces.Identity;

public interface ITokenManagement
{
    string GenerateToken(List<Claim> claims);
    string GenerateRefreshToken();

    Task<bool> ValidateRefreshToken(string refreshToken);
    Task<string?> GetUserIdByRefreshToken(string refreshToken);

    Task<int> AddRefreshToken(string userId, string refreshToken);
    Task<int> UpdateRefreshToken(string userId, string refreshToken);

    List<Claim> GetUserClaimsFromToken(string token);
}
