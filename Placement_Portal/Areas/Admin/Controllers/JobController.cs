using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Placement_Portal.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using OfficeOpenXml;
using System.Data;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
    public class JobController : Controller
    {
        private readonly HttpClient _client;

        public JobController()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5075/api/") };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        #region Get All Jobs
        [HttpGet]
        public async Task<IActionResult> JobList()
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _client.GetAsync("Job/GetAllJobs");
                if (response.IsSuccessStatusCode)
                {
                    var jobs = JsonConvert.DeserializeObject<List<JobModel>>(await response.Content.ReadAsStringAsync());
                    return View(jobs);
                }

                TempData["ErrorMessage"] = "Error fetching job data.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
            return View(new List<JobModel>());
        }
        #endregion

        #region Add / Edit Job
        [HttpGet]
        public async Task<IActionResult> AddEditJob(int? id)
        {
            SetAuthorizationHeader();
            var job = new JobModel();
            await PopulateCompanyDropdown();

            if (id.HasValue && id.Value > 0)
            {
                try
                {
                    var response = await _client.GetAsync($"Job/GetJobById/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        job = JsonConvert.DeserializeObject<JobModel>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error fetching job details.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error: {ex.Message}";
                }
            }
            return View(job);
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> SaveJob(JobModel model)
        {
            SetAuthorizationHeader();
            if (!ModelState.IsValid)
            {
                await PopulateCompanyDropdown();
                TempData["ErrorMessage"] = "Validation errors occurred.";
                return View("AddEditJob", model);
            }
            if (model.JobID == null) { model.JobID = 0; }

            using var formData = new MultipartFormDataContent();

            // Add Job Model fields to formData
            formData.Add(new StringContent(model.JobID.ToString()), "JobID");
            formData.Add(new StringContent(model.JobTitle ?? ""), "JobTitle");
            formData.Add(new StringContent(model.CompanyID.ToString()), "CompanyID");
            formData.Add(new StringContent(model.Contact_No ?? ""), "Contact_No");
            formData.Add(new StringContent(model.Email ?? ""), "Email");
            formData.Add(new StringContent(model.Position ?? ""), "Position");
            formData.Add(new StringContent(model.JobProfile ?? ""), "JobProfile");
            formData.Add(new StringContent(model.DateOfDrive?.ToString("yyyy-MM-dd") ?? ""), "DateOfDrive");
            formData.Add(new StringContent(model.SalaryPerMonth.ToString()), "SalaryPerMonth");
            formData.Add(new StringContent(model.Location ?? ""), "Location");

            HttpResponseMessage response;

            try
            {
                await PopulateCompanyDropdown();

                if (model.JobID > 0)
                {
                    response = await _client.PutAsync($"Job/UpdateJob", formData);
                }
                else
                {
                    response = await _client.PostAsync("Job/AddJob", formData);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = model.JobID > 0 ? "Job updated successfully." : "Job created successfully.";
                    return RedirectToAction("JobList");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error saving job. Status: {response.StatusCode}. Details: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View("AddEditJob", model);
        }


        #region Delete Job
        [HttpPost]
        public async Task<IActionResult> DeleteJob(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _client.DeleteAsync($"Job/DeleteJob/{id}");
                TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "Job deleted successfully." : "Error deleting job.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("JobList");
        }
        #endregion

        #region Populate Company Dropdown
        private async Task PopulateCompanyDropdown()
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _client.GetAsync("Company/GetCompanyDropdown/dropdown");

                if (response.IsSuccessStatusCode)
                {
                    var companies = JsonConvert.DeserializeObject<List<CompanyDropDownModel>>(await response.Content.ReadAsStringAsync());
                    ViewBag.CompanyList = new SelectList(companies, "CompanyID", "CompanyName");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching company dropdown.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
        }
        #endregion

        #region Export to Excel
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            SetAuthorizationHeader();
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve job data by calling the GetAllJobs API endpoint
            HttpResponseMessage response = _client.GetAsync("Job/GetAllJobs").Result;
            DataTable dataTable = new DataTable();

            if (response.IsSuccessStatusCode)
            {
                string jsonData = response.Content.ReadAsStringAsync().Result;
                // Convert JSON to DataTable using Newtonsoft.Json (ensure your JSON is structured appropriately)
                dataTable = JsonConvert.DeserializeObject<DataTable>(jsonData);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching job data for export.";
                return RedirectToAction("JobList");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Jobs");

                // Add header row
                worksheet.Cells[1, 1].Value = "JobID";
                worksheet.Cells[1, 2].Value = "JobTitle";
                worksheet.Cells[1, 3].Value = "CompanyID";
                worksheet.Cells[1, 4].Value = "CompanyName";
                worksheet.Cells[1, 5].Value = "Contact_No";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Position";
                worksheet.Cells[1, 8].Value = "JobProfile";
                worksheet.Cells[1, 9].Value = "DateOfDrive";
                worksheet.Cells[1, 10].Value = "SalaryPerMonth";
                worksheet.Cells[1, 11].Value = "Location";

                // Populate worksheet with DataTable content
                int row = 2;
                foreach (DataRow dr in dataTable.Rows)
                {
                    worksheet.Cells[row, 1].Value = dr["JobID"];
                    worksheet.Cells[row, 2].Value = dr["JobTitle"];
                    worksheet.Cells[row, 3].Value = dr["CompanyID"];
                    worksheet.Cells[row, 4].Value = dr["CompanyName"];
                    worksheet.Cells[row, 5].Value = dr["Contact_No"];
                    worksheet.Cells[row, 6].Value = dr["Email"];
                    worksheet.Cells[row, 7].Value = dr["Position"];
                    worksheet.Cells[row, 8].Value = dr["JobProfile"];
                    worksheet.Cells[row, 9].Value = dr["DateOfDrive"];
                    worksheet.Cells[row, 10].Value = dr["SalaryPerMonth"];
                    worksheet.Cells[row, 11].Value = dr["Location"];
                    row++;
                }

                // Auto-fit columns for better presentation
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"JobsData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        #endregion
    }
}
