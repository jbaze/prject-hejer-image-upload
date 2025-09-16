using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Application.Services;
using CustomerImageApi.DTOs.Common;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CustomerImageApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login with email and password")]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 401)]
    public async Task<ActionResult<ApiResponse<AuthenticationResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authenticationService.LoginAsync(request);
            return Ok(new ApiResponse<AuthenticationResponse>(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<AuthenticationResponse>(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AuthenticationResponse>($"An error occurred during login: {ex.Message}"));
        }
    }

    [HttpPost("refresh")]
    [SwaggerOperation(Summary = "Refresh access token using refresh token")]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 401)]
    public async Task<ActionResult<ApiResponse<AuthenticationResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authenticationService.RefreshTokenAsync(request);
            return Ok(new ApiResponse<AuthenticationResponse>(result, "Token refreshed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<AuthenticationResponse>(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AuthenticationResponse>($"An error occurred during token refresh: {ex.Message}"));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "Logout and revoke refresh token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] string refreshToken)
    {
        try
        {
            var result = await _authenticationService.RevokeTokenAsync(refreshToken);
            return Ok(new ApiResponse<bool>(result, "Logout successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>($"An error occurred during logout: {ex.Message}"));
        }
    }

    [HttpGet("validate")]
    [Authorize]
    [SwaggerOperation(Summary = "Validate current token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken()
    {
        try
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var isValid = await _authenticationService.ValidateTokenAsync(token);
            return Ok(new ApiResponse<bool>(isValid, isValid ? "Token is valid" : "Token is invalid"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>($"An error occurred during token validation: {ex.Message}"));
        }
    }

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Get currently logged user")]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 200)]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        try
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var user = await _authenticationService.GetCurrentUserAsync(token);
            if(user != null)
                return Ok(new ApiResponse<UserInfo>(user, "User retrieved successfully"));
            else
                return Ok(new ApiResponse<UserInfo>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>($"An error occurred during user validation: {ex.Message}"));
        }
    }
}