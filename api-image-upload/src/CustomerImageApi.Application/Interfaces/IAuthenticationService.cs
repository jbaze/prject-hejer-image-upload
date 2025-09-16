using CustomerImageApi.Domain.Entities;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserInfo?> GetCurrentUserAsync(string token);
}

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
    DateTime GetTokenExpiryDate(string token);
}