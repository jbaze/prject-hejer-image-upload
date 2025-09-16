using AutoMapper;
using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;
using Microsoft.Extensions.Logging;

namespace CustomerImageApi.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthenticationService> _logger;
    private const int RefreshTokenExpiryDays = 7;

    public AuthenticationService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("User not found or inactive for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        _logger.LogInformation("User found: {UserId}, Email: {Email}, IsActive: {IsActive}",
            user.Id, user.Email, user.IsActive);
        _logger.LogInformation("Stored password hash: {Hash}", user.PasswordHash);

        // Test the password verification with detailed logging
        var passwordVerificationResult = VerifyPassword(request.Password, user.PasswordHash);
        _logger.LogInformation("Password verification result: {Result} for password length: {Length}",
            passwordVerificationResult, request.Password.Length);

        if (!passwordVerificationResult)
        {
            _logger.LogWarning("Password verification failed for user: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        user.LastLoginAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Login successful for user: {Email}", request.Email);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtTokenService.GetTokenExpiryDate(accessToken),
            User = _mapper.Map<UserInfo>(user)
        };
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (!_jwtTokenService.ValidateToken(request.AccessToken))
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null || !user.IsActive || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        await _userRepository.UpdateAsync(user);

        return new AuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _jwtTokenService.GetTokenExpiryDate(newAccessToken),
            User = _mapper.Map<UserInfo>(user)
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        if (!_jwtTokenService.ValidateToken(token))
        {
            return false;
        }

        var userId = _jwtTokenService.GetUserIdFromToken(token);
        if (userId == null)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        return user != null && user.IsActive;
    }

    public async Task<UserInfo?> GetCurrentUserAsync(string token)
    {
        if (!_jwtTokenService.ValidateToken(token))
        {
            return null;
        }

        var userId = _jwtTokenService.GetUserIdFromToken(token);
        if (userId == null)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        return _mapper.Map<UserInfo>(user);
    }

    private bool VerifyPassword(string password, string currentPassword)
    {
        return password == currentPassword;
    }
}