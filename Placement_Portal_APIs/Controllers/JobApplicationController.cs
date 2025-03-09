using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly JobApplicationRepository _repository;

        public JobApplicationController(JobApplicationRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("user/{UserId}")]
        public IActionResult GetJobsByUserID(int UserId)
        {
            if (UserId <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            var jobs = _repository.SelectByUserID(UserId);

            if (jobs == null || !jobs.Any())
            {
                return NotFound("No job applications found for the given User ID");
            }

            return Ok(jobs);
        }

        // GET: api/jobapplication
        [HttpGet]
        public ActionResult<IEnumerable<JobApplicationModel>> GetAll()
        {
            return Ok(_repository.SelectAll());
        }

        // GET: api/jobapplication/id/5
        [HttpGet("id/{id}")]
        public ActionResult<JobApplicationModel> GetById(int id)
        {
            var jobApplication = _repository.SelectByPk(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            return Ok(jobApplication);
        }

        [HttpGet("GetCompanyId/{jobId}")]
        public IActionResult GetCompanyId(int jobId)
        {
            int companyId = _repository.GetCompanyIdByJobId(jobId);

            if (companyId == 0)
            {
                return NotFound("Company ID not found for the given Job ID.");
            }

            return Ok(companyId);
        }


        [HttpPost]
        public ActionResult<JobApplicationModel> Create(JobApplicationModel jobApplication)
        {
            if (jobApplication == null)
            {
                return BadRequest("Job application data is null.");
            }

            // Get the UserId from the model
            int UserId = jobApplication.UserID;

            // Fetch the CompanyID based on the JobID
            int companyId = _repository.GetCompanyIdByJobId(jobApplication.JobID);
            if (companyId == 0)
            {
                return BadRequest("Invalid Job ID or Company ID not found.");
            }

            // Assign CompanyID to the model
            jobApplication.CompanyID = companyId;

            if (_repository.Add(jobApplication, UserId))
            {
                return CreatedAtAction(nameof(GetById), new { id = jobApplication.ApplicationID }, jobApplication);
            }
            return BadRequest("Failed to add job application.");
        }


        // PUT: api/jobapplication/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, JobApplicationModel jobApplication)
        {
            if (id != jobApplication.ApplicationID)
            {
                return BadRequest();
            }

            if (_repository.Update(jobApplication))
            {
                return NoContent();
            }

            return NotFound();
        }

        // DELETE: api/jobapplication/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repository.Delete(id))
            {
                return NoContent();
            }
            return NotFound();
        }

        // GET: api/jobapplication/top10
        [HttpGet("top10")]
        public ActionResult<IEnumerable<JobApplicationModel>> GetTop10()
        {
            var top10Jobs = _repository.SelectTop10();
            if (top10Jobs == null || !top10Jobs.Any())
            {
                return NotFound("No job applications found.");
            }

            return Ok(top10Jobs);
        }

        // GET: api/jobapplication/count
        [HttpGet("count")]
        public IActionResult GetJobApplicationCount()
        {
            int totalCount = _repository.Count();
            return Ok(totalCount);
        }

        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve job application data as a DataTable from the repository
            DataTable dataTable = _repository.GetJobApplicationsDataTable();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("JobApplications");

                // Add header row – adjust the headers as needed.
                worksheet.Cells[1, 1].Value = "ApplicationID";
                worksheet.Cells[1, 2].Value = "StudentName";
                worksheet.Cells[1, 3].Value = "JobTitle";
                worksheet.Cells[1, 4].Value = "CompanyName";
                worksheet.Cells[1, 5].Value = "Gender";
                worksheet.Cells[1, 6].Value = "Phone_No";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "ApplicationDateTime";

                // Populate worksheet with data from the DataTable
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
                    worksheet.Cells[row, 8].Value = dr["ApplicationDateTime"];
                    row++;
                }

                // Auto-fit columns for better presentation
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
