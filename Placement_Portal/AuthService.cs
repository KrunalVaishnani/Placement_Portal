using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Placement_Portal.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> AuthenticateUserAsync(string username, string password, string role)
        {
            var requestData = new { UserName = username, Password = password, Role = role };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            // Adjust the URL as necessary
            HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:5075/api/Auth", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                // Assuming API returns JSON like: { "Message": "Login successful", "Token": "eyJhbGc..." }
                var resultObject = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                return resultObject?.Token;
            }

            return null;
        }
    }
}
