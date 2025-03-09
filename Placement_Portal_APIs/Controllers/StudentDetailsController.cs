using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using OfficeOpenXml;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class StudentDetailsController : ControllerBase
    {
        private readonly string _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        private readonly StudentDetailsRepository _studentDetailsRepository;

        public StudentDetailsController(StudentDetailsRepository studentDetailsRepository)
        {
            _studentDetailsRepository = studentDetailsRepository;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = _studentDetailsRepository.SelectAll();
            return Ok(students);
        }



        [Authorize]
        [HttpGet("{UserId}")]
        public IActionResult GetAllStudents(int UserId)
        {
            var students = _studentDetailsRepository.SelectAll(UserId);
            return Ok(students);
        }

        // GET: api/StudentStatus/GetStudentsWithStatus
        [Authorize]

        [HttpGet("GetStudentsWithStatus")]
        public IActionResult GetStudentsWithStatus()
        {
            var students = _studentDetailsRepository.GetStudentsWithStatus();
            return Ok(students);
        }

        // GET: api/StudentStatus/GetStudentsWithStatus
        [Authorize]

        [HttpGet]
        public IActionResult GetStudent_Placed()
        {
            var students = _studentDetailsRepository.Placed_Student_All();
            return Ok(students);
        }

        // GET: api/StudentStatus/GetStudentsWithStatus
        [Authorize]

        [HttpGet]
        public IActionResult GetStudent_NotPlaced()
        {
            var students = _studentDetailsRepository.Not_Placed_Student_All();
            return Ok(students);
        }

        [Authorize]

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student = _studentDetailsRepository.SelectByPk(id);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            return Ok(student);
        }
        [Authorize]

        [HttpPost]
        public IActionResult AddStudent([FromForm] StudentDetailsModel student, IFormFile resumeFile, IFormFile imageFile)
        {
            var isAdded = _studentDetailsRepository.Add(student, resumeFile, imageFile);
            if (!isAdded)
            {
                return BadRequest("Failed to add student.");
            }
            return Ok("Student added successfully.");
        }


        [Authorize]

        [HttpPut]
        public IActionResult UpdateStudent([FromForm] StudentDetailsModel student, IFormFile resumeFile, IFormFile imageFile)
        {
            var isUpdated = _studentDetailsRepository.Update(student, resumeFile, imageFile);
            if (!isUpdated)
            {
                return BadRequest("Failed to update student.");
            }
            return Ok("Student updated successfully.");
        }

        [Authorize]

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var isDeleted = _studentDetailsRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound("Student not found.");
            }
            return NoContent();
        }

        // GET: api/StudentDetails/GetResume/{fileName}
        [HttpGet("Resume/{fileName}")]
        public IActionResult GetResume(string fileName)
        {
            // Ensure the file path points to the Resumes subdirectory
            var filePath = Path.Combine(_uploadsPath, "Resumes", fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Resume not found.");
            }

            // Read the file content
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var extension = Path.GetExtension(fileName).ToLower();

            // Determine the content type based on the file extension
            string contentType = extension switch
            {
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream",
            };
            return File(fileBytes, contentType, fileName);
        }



        [HttpGet("Image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(_uploadsPath, "Images", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
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

        //// GET: api/StudentDetails/GetImage/{fileName}
        //[HttpGet("Image/{fileName}")]
        //public IActionResult GetImage(string fileName)
        //{
        //    var filePath = Path.Combine(_uploadsPath, "Images", fileName);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound("Image not found.");
        //    }
        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);

        //    // Check the file extension to return the appropriate content type
        //    var extension = Path.GetExtension(fileName).ToLower();
        //    string contentType;

        //    switch (extension)
        //    {
        //        case ".jpg":
        //        case ".jpeg":
        //            contentType = "image/jpeg";
        //            break;
        //        case ".png":
        //            contentType = "image/png";
        //            break;
        //        case ".gif":
        //            contentType = "image/gif";
        //            break;
        //        default:
        //            contentType = "application/octet-stream"; // Default content type
        //            break;
        //    }

        //    return File(fileBytes, contentType, fileName);
        //}

        [Authorize]

        [HttpGet("top10")]
        public IActionResult GetTop10Students()
        {
            var top10Students = _studentDetailsRepository.GetTop10Students();
            if (top10Students == null || !top10Students.Any())
            {
                return NotFound("No students found.");
            }

            return Ok(top10Students);
        }

        // GET: api/studentdetails/count
        [Authorize]

        [HttpGet("count")]
        public IActionResult GetStudentCount()
        {
            int totalCount = _studentDetailsRepository.GetStudentCount();
            return Ok(totalCount);
        }

        [Authorize]

        [HttpGet]
        public IActionResult FilterStudents(
    [FromQuery] string studentName = null,
    [FromQuery] string enrollmentNo = null)
        {
            var filteredStudents = _studentDetailsRepository.SelectAllFiltered(
                                        string.IsNullOrWhiteSpace(studentName) ? null : studentName,
                                        string.IsNullOrWhiteSpace(enrollmentNo) ? null : enrollmentNo);

            if (filteredStudents == null || !filteredStudents.Any())
            {
                return NotFound("No students found matching the criteria.");
            }
            return Ok(filteredStudents);
        }

        [Authorize]

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            // Set the EPPlus license context (NonCommercial for example)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve the DataTable containing the student data
            DataTable data = _studentDetailsRepository.GetStudentsDataTable();

            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add("Student Data");

                // Add the header row. Adjust column names as needed.
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
                // You can add more columns if needed (like Resume, Image, etc.)

                // Add the data rows from the DataTable
                int row = 2;
                foreach (DataRow dr in data.Rows)
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

                // Optional: Auto-fit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the package to a stream and return it
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"StudentsData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
}
    }
