using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<string> GenerateJwtTokenAsync(int userId);
    Task<string> GenerateRefreshTokenAsync();
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
}