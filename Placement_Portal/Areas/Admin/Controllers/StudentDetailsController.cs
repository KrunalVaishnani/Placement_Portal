using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Placement_Portal.Areas.Student.Models;
using OfficeOpenXml;
using System.Data;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
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
            List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>();
            int UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetAllStudents");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>>(data);
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
            List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudentsWithStatus/GetStudentsWithStatus");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>>(data);
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
            List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudent_Placed");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>>(data);
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
            List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel> students = new List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>();
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudent_NotPlaced");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>>(data);
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
            Placement_Portal.Areas.Admin.Models.StudentDetailsModel student = new Placement_Portal.Areas.Admin.Models.StudentDetailsModel();

            if (id.HasValue)
            {
                // Get existing student data for Edit
                HttpResponseMessage response = await _client.GetAsync($"StudentDetails/GetStudentById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    student = JsonConvert.DeserializeObject<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>(data);
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
        public async Task<IActionResult> SaveStudent(Placement_Portal.Areas.Admin.Models.StudentDetailsModel model, IFormFile resumeFile, IFormFile imageFile)
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

        [HttpGet]
        public async Task<IActionResult> FilterStudents(string studentName, string enrollmentNo)
        {
            SetAuthorizationHeader();
            // Build the query string. Only include parameters that are provided.
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                query.Add($"studentName={studentName}");
            }
            if (!string.IsNullOrWhiteSpace(enrollmentNo))
            {
                query.Add($"enrollmentNo={enrollmentNo}");
            }
            string queryString = query.Any() ? "?" + string.Join("&", query) : string.Empty;

            // Call the API FilterStudents endpoint (which should support filtering by either parameter)
            HttpResponseMessage response = await _client.GetAsync($"StudentDetails/FilterStudents{queryString}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<List<Placement_Portal.Areas.Admin.Models.StudentDetailsModel>>(data);
                return View("StudentList", students);
            }
            else
            {
                TempData["ErrorMessage"] = "Error filtering student data.";
                return RedirectToAction("StudentList");
            }
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            SetAuthorizationHeader();
            // Ensure you have EPPlus installed (via NuGet: EPPlus)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve student data by calling your API endpoint or repository method.
            // For this example, we call the API GetAllStudents endpoint.
            // Alternatively, you could call a repository method that returns a DataTable.
            HttpResponseMessage response = _client.GetAsync("StudentDetails/GetAllStudents").Result;
            DataTable dataTable = new DataTable();

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the API result into a DataTable.
                // You might need to write custom logic here to convert JSON to DataTable.
                // For demonstration, we'll use a simple approach.
                string jsonData = response.Content.ReadAsStringAsync().Result;
                // Convert JSON to DataTable using Newtonsoft.Json (requires a little helper code):
                dataTable = JsonConvert.DeserializeObject<DataTable>(jsonData);
            }
            else
            {
                TempData["ErrorMessage"] = "Error fetching student data for export.";
                return RedirectToAction("StudentList");
            }

            // Create Excel package using EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("StudentData");

                // Add header row
                worksheet.Cells[1, 1].Value = "StudentID";
                worksheet.Cells[1, 2].Value = "StudentName";
                worksheet.Cells[1, 3].Value = "Enrollment_No";
                worksheet.Cells[1, 4].Value = "Gender";
                worksheet.Cells[1, 5].Value = "DateOfBirth";
                worksheet.Cells[1, 6].Value = "Phone_No";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Address";
                worksheet.Cells[1, 9].Value = "SSC_Percentage";
                worksheet.Cells[1, 10].Value = "YearOfPassingSSC";
                worksheet.Cells[1, 11].Value = "HSC_Percentage";
                worksheet.Cells[1, 12].Value = "YearOfPassingHSC";
                worksheet.Cells[1, 13].Value = "UG_CGPA";
                worksheet.Cells[1, 14].Value = "YearOfPassingUG";
                worksheet.Cells[1, 15].Value = "Skills";

                // Populate worksheet with DataTable content
                int row = 2;
                foreach (DataRow dr in dataTable.Rows)
                {
                    worksheet.Cells[row, 1].Value = dr["StudentID"];
                    worksheet.Cells[row, 2].Value = dr["StudentName"];
                    worksheet.Cells[row, 3].Value = dr["Enrollment_No"];
                    worksheet.Cells[row, 4].Value = dr["Gender"];
                    worksheet.Cells[row, 5].Value = dr["DateOfBirth"];
                    worksheet.Cells[row, 6].Value = dr["Phone_No"];
                    worksheet.Cells[row, 7].Value = dr["Email"];
                    worksheet.Cells[row, 8].Value = dr["Address"];
                    worksheet.Cells[row, 9].Value = dr["SSC_Percentage"];
                    worksheet.Cells[row, 10].Value = dr["YearOfPassingSSC"];
                    worksheet.Cells[row, 11].Value = dr["HSC_Percentage"];
                    worksheet.Cells[row, 12].Value = dr["YearOfPassingHSC"];
                    worksheet.Cells[row, 13].Value = dr["UG_CGPA"];
                    worksheet.Cells[row, 14].Value = dr["YearOfPassingUG"];
                    worksheet.Cells[row, 15].Value = dr["Skills"];
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"StudentData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
