using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.Interfaces.Identity;
using ECommerce.Domain.Identity;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure.Identity;

public class TokenManagement : ITokenManagement
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public TokenManagement(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public string GenerateToken(List<Claim> claims)
    {
        var keyString = _config["JWT:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        const int byteSize = 64;
        var randomBytes = new byte[byteSize];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public async Task<bool> ValidateRefreshToken(string refreshToken)
    {
        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        return tokenInDb is not null;
    }

    public async Task<string?> GetUserIdByRefreshToken(string refreshToken)
    {
        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        return tokenInDb?.UserId;
    }

    public async Task<int> AddRefreshToken(string userId, string refreshToken)
    {
        var model = new RefreshToken
        {
            UserId = userId,
            Token = refreshToken
        };

        _context.RefreshTokens.Add(model);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateRefreshToken(string userId, string refreshToken)
    {
        var tokenInDb = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (tokenInDb is null)
        {
            return await AddRefreshToken(userId, refreshToken);
        }

        tokenInDb.Token = refreshToken;
        return await _context.SaveChangesAsync();
    }

    public List<Claim> GetUserClaimsFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.ToList();
        }
        catch
        {
            return new List<Claim>();
        }
    }
}
