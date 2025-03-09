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
    public class CompanyController : Controller
    {
        private readonly HttpClient _client;
        private const string BaseApiUrl = "http://localhost:5075/api/Company/";

        public CompanyController()
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

        // GET: Company List
        [HttpGet]
        public async Task<IActionResult> CompanyList()
        {
            SetAuthorizationHeader();
            var response = await _client.GetAsync("GetAllCompanies");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var companies = JsonConvert.DeserializeObject<List<CompanyModel>>(data);
                return View(companies);
            }
            TempData["ErrorMessage"] = "Error fetching company data.";
            return View(new List<CompanyModel>());
        }

        // GET: Create/Edit Company View
        [HttpGet]
        public async Task<IActionResult> AddEditCompany(int? id)
        {
            if (!id.HasValue)
                return View(new CompanyModel());

            var response = await _client.GetAsync($"GetCompanyById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var company = JsonConvert.DeserializeObject<CompanyModel>(data);
                return View(company);
            }
            TempData["ErrorMessage"] = "Error fetching company details.";
            return View(new CompanyModel());
        }

        // POST: Save Company (Add or Edit)
        [HttpPost]
        public async Task<IActionResult> SaveCompany([FromForm] CompanyModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please correct errors and try again.";
                return View("AddEditCompany", model);
            }

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"), "company");

            if (imageFile != null && imageFile.Length > 0)
            {
                var streamContent = new StreamContent(imageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                formData.Add(streamContent, "imageFile", imageFile.FileName);
            }

            HttpResponseMessage response = model.CompanyID > 0
                ? await _client.PutAsync("UpdateCompany", formData)
                : await _client.PostAsync("AddCompany", formData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = model.CompanyID > 0 ? "Company updated successfully." : "Company created successfully.";
                return RedirectToAction("CompanyList");
            }

            TempData["ErrorMessage"] = "Error saving company. Please check the details.";
            return View("AddEditCompany", model);
        }

        // POST: Delete Company
        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var response = await _client.DeleteAsync($"DeleteCompany/{id}");
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

        // GET: Retrieve Company Image
        [HttpGet("Image/{fileName}")]
        public async Task<IActionResult> GetCompanyImage(string fileName)
        {
            var response = await _client.GetAsync($"GetCompanyImage/{fileName}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Image not found.");
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            var extension = Path.GetExtension(fileName).ToLower();

            string contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };

            return File(fileBytes, contentType, fileName);
        }
    }
}
