//using System.Net.Http.Headers;

//namespace Placement_Portal.Services
//{
//    public class APIServices
//    {
//        private readonly HttpClient _client;
//        private const string BaseApiUrl = "http://localhost:5075/api/";
//        private readonly string Token;

//        public APIServices()
//        {
//            _client = new HttpClient { BaseAddress = new Uri(BaseApiUrl) };
//            Token = HttpContext.Session.GetString("token");
//            HttpContext httpContext = new HttpContext();
//        }

//        public async Task<HttpResponseMessage> GetAsync(string endpoint) {
//            var request = new HttpRequestMessage(HttpMethod.Get, BaseApiUrl + "Job/GetJobCount");
//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",);
//            var response = await _client.SendAsync(request);

//            return response;
//        }
//    }
//}
