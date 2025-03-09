using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Placement_Portal.Areas.Student.Models;

namespace Placement_Portal.Areas.Student.Controllers
{
    [CheckAccess]

    [Area("Student")]
    public class StudentDetailsController : Controller
    {
        private readonly Uri baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;

        public StudentDetailsController()
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

        // GET: Student List
        [HttpGet]
        public async Task<IActionResult> StudentList()
        {
            SetAuthorizationHeader();
            List<StudentDetailsModel> students = new List<StudentDetailsModel>();
            int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetAllStudents/{UserId}");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<StudentDetailsModel>>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching student data.";
            }

            return View(students);
        }
        // GET: Student/StudentStatusList
        [HttpGet]
        public async Task<IActionResult> StudentStatusList()
        {
            SetAuthorizationHeader();
            List<StudentDetailsModel> students = new List<StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudentsWithStatus/GetStudentsWithStatus");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<StudentDetailsModel>>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching student status data.";
            }

            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> PlacedStudentList()
        {
            SetAuthorizationHeader();
            List<Placement_Portal.Areas.Student.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Student.Models.StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudent_Placed");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Student.Models.StudentDetailsModel>>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching student status data.";
            }

            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> NotPlacedStudentList()
        {
            SetAuthorizationHeader();
            List<Placement_Portal.Areas.Student.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Student.Models.StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudent_NotPlaced");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Student.Models.StudentDetailsModel>>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching student status data.";
            }

            return View(students);
        }
        public async Task<IActionResult> AddEditStudent(int? id)
        {
            SetAuthorizationHeader();

            StudentDetailsModel student = new StudentDetailsModel();

            if (id.HasValue)
            {
                // Get existing student data for Edit
                HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudentById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    student = JsonConvert.DeserializeObject<StudentDetailsModel>(data);
                }
            }

            // Ensure Status and IsActive fields are set when editing
            if (student.Status == null)
                student.Status = "Active"; // Default value

            if (student.IsActive == null)
            {
                student.IsActive = false; // Default to false if null
            }

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> SaveStudent(StudentDetailsModel model, IFormFile resumeFile, IFormFile imageFile)
        {
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View("AddEditStudent", model);
            }

            using var formData = new MultipartFormDataContent();

            // Add Student Model as Separate Fields
            if (model != null)
            {
                formData.Add(new StringContent(model.StudentID.ToString()), "StudentID"); // Add StudentID
                formData.Add(new StringContent(model.StudentName ?? ""), "StudentName");
                formData.Add(new StringContent(model.Enrollment_No ?? ""), "Enrollment_No");
                formData.Add(new StringContent(model.Gender ?? ""), "Gender");
                formData.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd") ?? ""), "DateOfBirth");
                formData.Add(new StringContent(model.Phone_No ?? ""), "Phone_No");
                formData.Add(new StringContent(model.Email ?? ""), "Email");
                formData.Add(new StringContent(model.Address ?? ""), "Address");
                formData.Add(new StringContent(model.SSC_Percentage.ToString() ?? ""), "SSC_Percentage");
                formData.Add(new StringContent(model.YearOfPassingSSC.ToString() ?? ""), "YearOfPassingSSC");
                formData.Add(new StringContent(model.HSC_Percentage.ToString() ?? ""), "HSC_Percentage");
                formData.Add(new StringContent(model.YearOfPassingHSC.ToString() ?? ""), "YearOfPassingHSC");
                formData.Add(new StringContent(model.UG_CGPA?.ToString() ?? ""), "UG_CGPA");
                formData.Add(new StringContent(model.YearOfPassingUG?.ToString() ?? ""), "YearOfPassingUG");
                formData.Add(new StringContent(model.Skills ?? ""), "Skills");
                formData.Add(new StringContent(model.Status ?? "Placed"), "Status");
                formData.Add(new StringContent(model.IsActive.ToString()), "IsActive");

                // Add Resume and Image fields
                formData.Add(new StringContent(model.Resume ?? ""), "Resume");
                formData.Add(new StringContent(model.Image ?? ""), "Image");
            }


            // Attach Resume File
            if (resumeFile != null && resumeFile.Length > 0)
            {
                var streamContent = new StreamContent(resumeFile.OpenReadStream());
                formData.Add(streamContent, "resumeFile", resumeFile.FileName);
            }

            // Attach Image File
            if (imageFile != null && imageFile.Length > 0)
            {
                var streamContent = new StreamContent(imageFile.OpenReadStream());
                formData.Add(streamContent, "imageFile", imageFile.FileName);
            }

            HttpResponseMessage response;

            if (model.StudentID > 0)
            {
                response = await _client.PutAsync($"{_client.BaseAddress}StudentDetails/UpdateStudent", formData);
            }
            else
            {
                response = await _client.PostAsync($"{_client.BaseAddress}StudentDetails/AddStudent", formData);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = model.StudentID > 0 ? "Student updated successfully." : "Student created successfully.";
                return RedirectToAction("StudentList");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error saving student. Status: {response.StatusCode}. Details: {errorMessage}";
                return View("AddEditStudent", model);
            }
        }

        

        // POST: Delete a student
        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            SetAuthorizationHeader();

            HttpResponseMessage response = _client.DeleteAsync($"StudentDetails/DeleteStudent/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Student deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting student.";
            }

            return RedirectToAction("StudentList");
        }
    }
}
