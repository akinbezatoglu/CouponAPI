using System.Net;

namespace CouponAPI.Models
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<String> ErrorMessages { get; set; }
        public APIResponse()
        {
            ErrorMessages = new List<String>();
        }
    }
}
