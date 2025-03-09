using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Placement_Portal.Areas.Admin.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Data;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly Uri baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;

        public CompanyController()
        {
            _client = new HttpClient { BaseAddress = baseAddress };
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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Company List
        [HttpGet]
        public async Task<IActionResult> CompanyList()
        {
            SetAuthorizationHeader();
            List<CompanyModel> companies = new List<CompanyModel>();
            HttpResponseMessage response = await _client.GetAsync("Company/GetAllCompanies");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                companies = JsonConvert.DeserializeObject<List<CompanyModel>>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching company data.";
            }

            return View(companies);
        }

        public async Task<IActionResult> AddEditCompany(int? id)
        {
            SetAuthorizationHeader();
            CompanyModel company = new CompanyModel();

            if (id.HasValue)
            {
                HttpResponseMessage response = await _client.GetAsync($"Company/GetCompanyById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    company = JsonConvert.DeserializeObject<CompanyModel>(data);
                }
            }

            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCompany(CompanyModel model, IFormFile imageFile)
        {
            SetAuthorizationHeader();
            if (!ModelState.IsValid)
            {
                return View("AddEditCompany", model);
            }

            using var formData = new MultipartFormDataContent();
            if (model.CompanyID == null) { model.CompanyID = 0; }
            // Add Company Model fields
            formData.Add(new StringContent(model.CompanyID.ToString()), "CompanyID");
            formData.Add(new StringContent(model.CompanyName ?? ""), "CompanyName");
            formData.Add(new StringContent(model.Contact_No ?? ""), "Contact_No");
            formData.Add(new StringContent(model.Email ?? ""), "Email");
            formData.Add(new StringContent(model.Address ?? ""), "Address");
            formData.Add(new StringContent(model.Pincode.ToString()), "Pincode");
            formData.Add(new StringContent(model.Image ?? ""), "Image");

            // Attach Image File
            if (imageFile != null && imageFile.Length > 0)
            {
                var streamContent = new StreamContent(imageFile.OpenReadStream());
                formData.Add(streamContent, "imageFile", imageFile.FileName);
            }

            HttpResponseMessage response;

            if (model.CompanyID > 0)
            {
                response = await _client.PutAsync("Company/UpdateCompany", formData);
            }
            else
            {
                response = await _client.PostAsync("Company/AddCompany", formData);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = model.CompanyID > 0 ? "Company updated successfully." : "Company created successfully.";
                return RedirectToAction("CompanyList");
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error saving company. Status: {response.StatusCode}. Details: {errorMessage}";
                return View("AddEditCompany", model);
            }
        }


        // POST: Delete a company
        [HttpPost]
        public IActionResult DeleteCompany(int id)
        {
            SetAuthorizationHeader();
            HttpResponseMessage response = _client.DeleteAsync($"Company/DeleteCompany/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting company.";
            }
            return RedirectToAction("CompanyList");
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyDropdown()
        {
            SetAuthorizationHeader();
            List<CompanyModel> dropdownList = new List<CompanyModel>();

            try
            {
                HttpResponseMessage response = await _client.GetAsync("Company/Dropdown");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dropdownList = JsonConvert.DeserializeObject<List<CompanyModel>>(data);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return Json(dropdownList);
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            SetAuthorizationHeader();
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve the company data by calling your API endpoint.
            // Here, we call the GetAllCompanies endpoint and convert the JSON to a DataTable.
            HttpResponseMessage response = _client.GetAsync("Company/GetAllCompanies").Result;
            DataTable dataTable = new DataTable();

            if (response.IsSuccessStatusCode)
            {
                string jsonData = response.Content.ReadAsStringAsync().Result;
                // Convert JSON to DataTable using Newtonsoft.Json
                dataTable = JsonConvert.DeserializeObject<DataTable>(jsonData);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching company data for export.";
                return RedirectToAction("CompanyList");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Companies");

                // Add header row
                worksheet.Cells[1, 1].Value = "CompanyID";
                worksheet.Cells[1, 2].Value = "CompanyName";
                worksheet.Cells[1, 3].Value = "Contact_No";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Pincode";

                // Populate worksheet with DataTable content
                int row = 2;
                foreach (DataRow dr in dataTable.Rows)
                {
                    worksheet.Cells[row, 1].Value = dr["CompanyID"];
                    worksheet.Cells[row, 2].Value = dr["CompanyName"];
                    worksheet.Cells[row, 3].Value = dr["Contact_No"];
                    worksheet.Cells[row, 4].Value = dr["Email"];
                    worksheet.Cells[row, 5].Value = dr["Address"];
                    worksheet.Cells[row, 6].Value = dr["Pincode"];
                    row++;
                }

                // Auto-fit columns for better presentation
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"CompaniesData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}