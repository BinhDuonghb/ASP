namespace HUBTSOCIAL.Src.Features.Auth.Responses
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public AuthResponse(bool success, int statusCode, string message, object? data = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }    
}

