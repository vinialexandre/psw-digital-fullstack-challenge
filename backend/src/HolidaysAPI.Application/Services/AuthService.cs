using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HolidaysAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ApiResponse<LoginResponse>.ErrorResponse("Username and password are required");
        }

        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = GenerateJwtToken(request.Username);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return ApiResponse<LoginResponse>.SuccessResponse(
                new LoginResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt
                },
                message: "Login successful"
            );
        }

        return ApiResponse<LoginResponse>.ErrorResponse("Invalid credentials");
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

