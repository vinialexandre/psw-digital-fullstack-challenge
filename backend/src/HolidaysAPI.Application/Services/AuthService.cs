using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using HolidaysAPI.Domain.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HolidaysAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly AuthSettings _authSettings;

    public AuthService(IOptions<JwtSettings> jwtSettings, IOptions<AuthSettings> authSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _authSettings = authSettings.Value;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ApiResponse<LoginResponse>.ErrorResponse("Username and password are required");
        }

        if (request.Username == _authSettings.AdminUsername && request.Password == _authSettings.AdminPassword)
        {
            var token = GenerateJwtToken(request.Username);
            var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

            return ApiResponse<LoginResponse>.SuccessResponse(
                new LoginResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    Username = request.Username
                },
                message: "Login successful"
            );
        }

        return ApiResponse<LoginResponse>.ErrorResponse("Invalid credentials");
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

