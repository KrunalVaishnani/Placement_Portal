using Placement_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Placement_Portal.Areas.Student.Controllers
{
    [CheckAccess]
    [Area("Student")]
    public class StudentController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;

        public StudentController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StudentList()
        {
            List<StudentModel> students = new List<StudentModel>();

            // Fetch data from the API
            HttpResponseMessage response = _client.GetAsync("Student").Result;

            if (response.IsSuccessStatusCode)
            {
                // Read response data
                string data = response.Content.ReadAsStringAsync().Result;

                // Deserialize JSON into List<StudentModel>
                students = JsonConvert.DeserializeObject<List<StudentModel>>(data);
            }
            else
            {
                // Add error to the ModelState for debugging
                ModelState.AddModelError("", $"Error fetching data: {response.StatusCode}");
            }

            return View("StudentList", students); // Bind the data to the view
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            HttpResponseMessage response = _client.DeleteAsync($"Student/DeleteStudent/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Student deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting student.";
            }
            return RedirectToAction("Get");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync("Student/Add", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Get");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            StudentModel student = new StudentModel();
            HttpResponseMessage response = await _client.GetAsync($"Student/GetStudentByID/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                student = JsonConvert.DeserializeObject<StudentModel>(data);
            }

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PutAsync("Student/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Get");
                }
            }
            TempData["ErrorMessage"] = "Invalid data. Please try again.";
            return View(model);
        }
    }
}
