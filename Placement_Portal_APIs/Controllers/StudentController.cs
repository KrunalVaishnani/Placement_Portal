using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository _studentRepository;

        public StudentController(StudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = _studentRepository.SelectAll();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student = _studentRepository.SelectByPk(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var isDeleted = _studentRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult Add(StudentModel studentModel)
        {
            var isAdded = _studentRepository.Add(studentModel);
            if (!isAdded)
            {
                return BadRequest("Failed to add the student.");
            }
            return Ok("Student added successfully.");
        }

        [HttpPut]
        public IActionResult Update(StudentModel studentModel)
        {
            var isUpdated = _studentRepository.Update(studentModel);
            if (!isUpdated)
            {
                return BadRequest("Failed to update the student.");
            }
            return Ok("Student updated successfully.");
        }
    }
}
