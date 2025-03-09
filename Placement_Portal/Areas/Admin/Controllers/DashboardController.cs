using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Placement_Portal.Areas.Admin.Models;
using Placement_Portal.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly HttpClient _client;
        private const string BaseApiUrl = "http://localhost:5075/api/";

        public DashboardController()
        {
            _client = new HttpClient { BaseAddress = new Uri(BaseApiUrl) };
        }
        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("⚠ No JWT Token found in session.");
            }
        }

        // GET: Dashboard Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            var model = new Dashboard
            {
                jobs = await FetchTop10Jobs<Placement_Portal.Areas.Admin.Models.JobModel>("Job"),
                TotalJobs = await FetchCountJobs("Job"),
                Companies = await FetchTop10Company<Placement_Portal.Areas.Admin.Models.CompanyModel>("Company"),
                TotalCompanies = await FetchCountCompany("Company"),
                Students = await FetchTop10Student<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>("Student"),
                TotalStudents = await FetchCountStudent("Student"),
                JobApplications = await FetchTop10JobApplication<Placement_Portal.Areas.Admin.Models.JobApplicationModel>("JobApplication"),
                TotalApplications = await FetchCountJobApplication("JobApplication"),
                Users = await FetchTop10User<Placement_Portal.Areas.Admin.Models.UserModel>("User"),
                TotalUsers = await FetchCountUser("User")
            };
            return View(model);
        }

        // Fetch top 10 Jobs
        private async Task<List<T>> FetchTop10Jobs<T>(string endpoint)
        {
            var response = await _client.GetAsync($"Job/GetTop10Jobs/top10");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(data);
            }
            TempData["ErrorMessage"] = $"Error fetching top 3 {endpoint}.";
            return new List<T>();
        }

        // Fetch count job
        private async Task<int> FetchCountJobs(string endpoint)
        {
            var response = await _client.GetAsync("Job/GetJobCount");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                // If API returns { "count": 25 }, deserialize it correctly
                var result = JsonConvert.DeserializeObject<dynamic>(data);
                return result.count;
            }

            TempData["ErrorMessage"] = $"Error fetching {endpoint} count.";
            return 0;
        }

        // Fetch top 10 Company
        private async Task<List<T>> FetchTop10Company<T>(string endpoint)
        {
            var response = await _client.GetAsync($"Company/GetTop10Companies");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(data);
            }
            TempData["ErrorMessage"] = $"Error fetching top 3 {endpoint}.";
            return new List<T>();
        }

        // Fetch count Company
        private async Task<int> FetchCountCompany(string endpoint)
        {
            var response = await _client.GetAsync("Company/GetCompanyCount");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                // If API returns { "count": 25 }, deserialize it correctly
                var result = JsonConvert.DeserializeObject<dynamic>(data);
                return result.count;
            }

            TempData["ErrorMessage"] = $"Error fetching {endpoint} count.";
            return 0;
        }

        // Fetch top 10 Student
        private async Task<List<T>> FetchTop10Student<T>(string endpoint)
        {
            var response = await _client.GetAsync($"StudentDetails/GetTop10Students/top10");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(data);
            }
            TempData["ErrorMessage"] = $"Error fetching top 3 {endpoint}.";
            return new List<T>();
        }

        // Fetch count Student
        private async Task<int> FetchCountStudent(string endpoint)
        {
            var response = await _client.GetAsync($"StudentDetails/GetStudentCount/count");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                // Directly parse if the response is a plain number
                if (int.TryParse(data, out int count))
                {
                    return count;
                }

                // If the response is a JSON object { "count": 11 }, deserialize it correctly
                var result = JsonConvert.DeserializeObject<dynamic>(data);
                return result.count;
            }

            TempData["ErrorMessage"] = $"Error fetching {endpoint} count.";
            return 0;
        }




        // Fetch top 10 JobApplication
        private async Task<List<T>> FetchTop10JobApplication<T>(string endpoint)
        {
            var response = await _client.GetAsync($"JobApplication/top10");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(data);
            }
            TempData["ErrorMessage"] = $"Error fetching top 3 {endpoint}.";
            return new List<T>();
        }

        private async Task<int> FetchCountJobApplication(string endpoint)
        {
            var response = await _client.GetAsync("JobApplication/count");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                // Case 1: API returns a plain number (e.g., "25")
                if (int.TryParse(data, out int count))
                {
                    return count;
                }

                // Case 2: API returns a JSON object { "count": 25 }
                var result = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
                return result.ContainsKey("count") ? result["count"] : 0;
            }

            TempData["ErrorMessage"] = $"Error fetching {endpoint} count.";
            return 0;
        }


        // Fetch top 10 User
        private async Task<List<T>> FetchTop10User<T>(string endpoint)
        {
            var response = await _client.GetAsync($"User/GetTop10Users");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(data);
            }
            TempData["ErrorMessage"] = $"Error fetching top 3 {endpoint}.";
            return new List<T>();
        }

        //User Count
        private async Task<int> FetchCountUser(string endpoint)
        {
            var response = await _client.GetAsync("User/GetUserCount");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {data}"); // Debugging

                // Directly parse as int
                int count = JsonConvert.DeserializeObject<int>(data);

                Console.WriteLine($"Parsed count: {count}"); // Debugging

                return count;
            }

            TempData["ErrorMessage"] = $"Error fetching {endpoint} count.";
            return 0;
        }



    }
}
