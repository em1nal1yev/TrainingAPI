namespace TelimAPI.API.Common.Helper
{
    public class ApiResponses
    {
        public static ApiResponse<T> Success<T>(T data = default, string message = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

        public static ApiResponse<T> Fail<T>(string message, List<string> errors = null)
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors
            };
    }
}
