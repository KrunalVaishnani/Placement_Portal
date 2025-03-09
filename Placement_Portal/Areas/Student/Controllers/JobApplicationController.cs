using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Placement_Portal.Areas.Student.Models;
using System.Net.Http.Headers;

namespace Placement_Portal.Areas.Student.Controllers
{
    [CheckAccess]

    [Area("Student")]
    public class JobApplicationController : Controller
    {
        private readonly Uri _baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;

        public JobApplicationController()
        {
            _client = new HttpClient { BaseAddress = _baseAddress };
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

        // Get all job applications
        [HttpGet]
        public async Task<IActionResult> JobApplications()
        {
            SetAuthorizationHeader();
            List<JobApplicationModel> applications = new List<JobApplicationModel>();

            try
            {
                HttpResponseMessage response = await _client.GetAsync("JobApplication");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    applications = JsonConvert.DeserializeObject<List<JobApplicationModel>>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching job applications.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(applications);
        }

        // Get job applications by UserID
        [HttpGet]
        public async Task<IActionResult> GetJobsByUserID()
        {
            SetAuthorizationHeader();
            List<JobApplicationModel> applications = new List<JobApplicationModel>();

            try
            {
                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/user/{UserId}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    applications = JsonConvert.DeserializeObject<List<JobApplicationModel>>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "No job applications found for the given User ID.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(applications);
        }


        // Get job application by ID
        [HttpGet]
        public async Task<IActionResult> JobApplicationDetails(int id)
        {
            SetAuthorizationHeader();
            JobApplicationModel application = new JobApplicationModel();

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    application = JsonConvert.DeserializeObject<JobApplicationModel>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching job application details.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(application);
        }

        public async Task<int> GetCompanyIdByJobId(int jobId)
        {
            SetAuthorizationHeader();
            HttpResponseMessage response = await _client.GetAsync($"JobApplication/GetCompanyId/{jobId}");

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return int.TryParse(result, out int companyId) ? companyId : 0;
            }

            return 0; // Return 0 if not found or error occurs
        }


        // Add Job Application
        [HttpGet]
        public IActionResult AddJobApplication()
        {
            SetAuthorizationHeader();
            return View(new Placement_Portal.Areas.Student.Models.JobApplicationModel());
        }
        [HttpPost]
        public async Task<IActionResult> AddJobApplication(int id) // 'id' is the JobID from the route
        {
            SetAuthorizationHeader();
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix validation errors and try again.";
                return RedirectToAction("JobList", "Job");
            }

            try
            {
                // Get the UserId from the session
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    TempData["ErrorMessage"] = "User not logged in.";
                    return RedirectToAction("JobList", "Job");
                }
                int UserId = Convert.ToInt32(userIdString);

                // Fetch the company ID from API
                int companyId = await GetCompanyIdByJobId(id);
                if (companyId == 0)
                {
                    TempData["ErrorMessage"] = "Invalid Job ID or Company ID not found.";
                    return RedirectToAction("JobList", "Job");
                }

                // Create the JobApplicationModel
                var jobApplication = new Placement_Portal.Areas.Student.Models.JobApplicationModel
                {
                    JobID = id,
                    StudentID = UserId,
                    CompanyID = companyId, // Include the required field
                    ApplicationDateTime = DateTime.Now,
                    UserID = UserId
                };

                // Call the API to add the job application
                var jsonContent = new StringContent(JsonConvert.SerializeObject(jobApplication), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"JobApplication", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Job application submitted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error adding job application. Response: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("JobList", "Job");
        }


        // Edit Job Application
        [HttpGet]
        public async Task<IActionResult> EditJobApplication(int id)
        {
            JobApplicationModel application = new JobApplicationModel();

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    application = JsonConvert.DeserializeObject<JobApplicationModel>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching application details.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> EditJobApplication(JobApplicationModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix validation errors and try again.";
                return View(model);
            }

            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PutAsync("JobApplication", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Job application updated successfully.";
                    return RedirectToAction("JobApplicationList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error updating job application.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(model);
        }

        // Delete Job Application
        [HttpPost]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            SetAuthorizationHeader();
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync($"JobApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Job application deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting job application.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("JobApplicationList");
        }
    }
}
