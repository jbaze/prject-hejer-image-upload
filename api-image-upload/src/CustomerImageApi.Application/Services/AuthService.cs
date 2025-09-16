using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CustomerImageApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !user.IsActive || user.PasswordHash != request.Password)
        {
            return null;
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Generate tokens
        var accessToken = await GenerateJwtTokenAsync(user.Id);
        var refreshToken = await GenerateRefreshTokenAsync();

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days
        await _userRepository.UpdateAsync(user);

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expiryMinutes = int.Parse(jwtSettings["AccessTokenExpiryMinutes"] ?? "60");

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            }
        };
    }

    public async Task<string> GenerateJwtTokenAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return string.Empty;

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentException("JWT SecretKey is required");
        var issuer = jwtSettings["Issuer"] ?? throw new ArgumentException("JWT Issuer is required");
        var audience = jwtSettings["Audience"] ?? throw new ArgumentException("JWT Audience is required");
        var expiryMinutes = int.Parse(jwtSettings["AccessTokenExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.FullName),
            new System.Security.Claims.Claim("FirstName", user.FirstName),
            new System.Security.Claims.Claim("LastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        // This would need a method to find user by refresh token
        // For now, we'll keep it simple and return null
        // In a full implementation, you'd want to add a method to find user by refresh token
        return null;
    }
}