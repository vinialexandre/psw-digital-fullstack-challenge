namespace HolidaysAPI.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalRecords { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, int totalRecords = 0, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            TotalRecords = totalRecords,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}

