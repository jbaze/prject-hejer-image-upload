using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerImageApi.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApiResponse()
        {
        }

        public ApiResponse(T data, string message = "Success")
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(string error)
        {
            Success = false;
            Message = "Operation failed";
            Errors = new List<string> { error };
        }

        public ApiResponse(List<string> errors)
        {
            Success = false;
            Message = "Operation failed";
            Errors = errors;
        }
    }
}
