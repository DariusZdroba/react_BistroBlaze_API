using System.Net;

namespace BistroBlaze_API.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
        public ApiResponse()
        {
            ErrorMessages = new List<string>();
        }
    }
}
