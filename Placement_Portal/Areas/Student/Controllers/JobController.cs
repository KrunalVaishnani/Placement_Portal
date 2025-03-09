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
    public class JobController : Controller
    {
        private readonly Uri _baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;

        public JobController()
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

        //public async Task<IActionResult> Index()
        //{
        //    return await JobList();
        //}

        // Existing methods (unchanged)
        [HttpGet]
        public async Task<IActionResult> JobList()
        {
            SetAuthorizationHeader();
            List<JobModel> jobs = new List<JobModel>();

            try
            {
                HttpResponseMessage response = await _client.GetAsync("Job/GetAllJobs");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    jobs = JsonConvert.DeserializeObject<List<JobModel>>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching job data. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(jobs);
        }

        
        public async Task<IActionResult> AddEditJob(int? id)
        {
            JobModel job = new JobModel();

            if (id.HasValue)
            {
                try
                {
                    HttpResponseMessage response = await _client.GetAsync($"Job/GetJobById/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        job = JsonConvert.DeserializeObject<JobModel>(data);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error fetching job details.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }

            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> SaveJob(JobModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
                return View("AddEditJob", model);
            }

            try
            {
                var formData = new MultipartFormDataContent();

                // Add model data to form content
                formData.Add(new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"), "job");

                // Add image file if present
                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                        return View("AddEditJob", model);
                    }

                    if (imageFile.Length > 2 * 1024 * 1024) // 2MB max
                    {
                        ModelState.AddModelError("Image", "Image file size cannot exceed 2MB.");
                        return View("AddEditJob", model);
                    }

                    var streamContent = new StreamContent(imageFile.OpenReadStream());
                    formData.Add(streamContent, "imageFile", imageFile.FileName);
                }

                HttpResponseMessage response;

                if (model.JobID > 0)
                {
                    // Edit existing job
                    response = await _client.PutAsync("Job/UpdateJob", formData);
                }
                else
                {
                    // Add new job
                    response = await _client.PostAsync("Job/AddJob", formData);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = model.JobID > 0 ? "Job updated successfully." : "Job created successfully.";
                    return RedirectToAction("JobList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error saving job. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View("AddEditJob", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync($"Job/DeleteJob/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Job deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting job.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("JobList");
        }




    }
}