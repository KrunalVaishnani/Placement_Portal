using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Placement_Portal.Areas.Student.Models;
using OfficeOpenXml;
using System.Data;
using System.Net.Http.Headers;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
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
            List<Placement_Portal.Areas.Admin.Models.JobApplicationModel> applications = new List<Placement_Portal.Areas.Admin.Models.JobApplicationModel>();

            try
            {
                HttpResponseMessage response = await _client.GetAsync("JobApplication");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    applications = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.JobApplicationModel>>(data);
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
            List<Placement_Portal.Areas.Admin.Models.JobApplicationModel> applications = new List<Placement_Portal.Areas.Admin.Models.JobApplicationModel>();

            try
            {
                int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/user/{UserId}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    applications = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.JobApplicationModel>>(data);
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
            Placement_Portal.Areas.Admin.Models.JobApplicationModel application = new Placement_Portal.Areas.Admin.Models.JobApplicationModel();

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    application = JsonConvert.DeserializeObject<Placement_Portal.Areas.Admin.Models.JobApplicationModel>(data);
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
            return View(new Placement_Portal.Areas.Admin.Models.JobApplicationModel());
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
                var jobApplication = new JobApplicationModel
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
            SetAuthorizationHeader();
            Placement_Portal.Areas.Admin.Models.JobApplicationModel application = new Placement_Portal.Areas.Admin.Models.JobApplicationModel();

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"JobApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    application = JsonConvert.DeserializeObject<Placement_Portal.Areas.Admin.Models.JobApplicationModel>(data);
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
        public async Task<IActionResult> EditJobApplication(Placement_Portal.Areas.Admin.Models.JobApplicationModel model)
        {
            SetAuthorizationHeader();
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
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            SetAuthorizationHeader();
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve job applications by calling your API endpoint.
            // Here we call the "JobApplication" endpoint to get all job applications.
            HttpResponseMessage response = _client.GetAsync("JobApplication").Result;
            DataTable dataTable = new DataTable();

            if (response.IsSuccessStatusCode)
            {
                string jsonData = response.Content.ReadAsStringAsync().Result;
                // Convert JSON to DataTable using Newtonsoft.Json
                dataTable = JsonConvert.DeserializeObject<DataTable>(jsonData);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching job applications for export.";
                return RedirectToAction("JobApplications");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("JobApplications");

                // Add header row
                worksheet.Cells[1, 1].Value = "ApplicationID";
                worksheet.Cells[1, 2].Value = "StudentName";
                worksheet.Cells[1, 3].Value = "JobTitle";
                worksheet.Cells[1, 4].Value = "CompanyName";
                worksheet.Cells[1, 5].Value = "Gender";
                worksheet.Cells[1, 6].Value = "Phone_No";
                worksheet.Cells[1, 7].Value = "Email";
                //worksheet.Cells[1, 8].Value = "ApplicationDateTime";

                // Populate worksheet with DataTable content
                int row = 2;
                foreach (DataRow dr in dataTable.Rows)
                {
                    worksheet.Cells[row, 1].Value = dr["ApplicationID"];
                    worksheet.Cells[row, 2].Value = dr["StudentName"];
                    worksheet.Cells[row, 3].Value = dr["JobTitle"];
                    worksheet.Cells[row, 4].Value = dr["CompanyName"];
                    worksheet.Cells[row, 5].Value = dr["Gender"];
                    worksheet.Cells[row, 6].Value = dr["Phone_No"];
                    worksheet.Cells[row, 7].Value = dr["Email"];
                    //worksheet.Cells[row, 8].Value = dr["ApplicationDateTime"];
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"JobApplications-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}