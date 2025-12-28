using HolidaysAPI.Application.DTOs;

namespace HolidaysAPI.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
}

