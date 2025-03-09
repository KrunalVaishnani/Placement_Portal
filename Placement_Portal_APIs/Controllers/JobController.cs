using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly JobRepository _jobRepository;

        public JobController(JobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // Get all jobs
        [HttpGet]
        public IActionResult GetAllJobs()
        {
            var jobs = _jobRepository.SelectAll();
            return Ok(jobs);
        }

        // Get job by ID
        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            var job = _jobRepository.SelectByPk(id);
            if (job == null)
            {
                return NotFound(new { message = "Job not found." });
            }
            return Ok(job);
        }

        // Add a new job
        [HttpPost]
        public IActionResult AddJob([FromForm] JobModel job)
        {
            if (job == null)
            {
                return BadRequest(new { message = "Invalid job data." });
            }

            bool isAdded = _jobRepository.Add(job);
            if (!isAdded)
            {
                return BadRequest(new { message = "Failed to add job." });
            }
            return CreatedAtAction(nameof(GetJobById), new { id = job.JobID }, new { message = "Job added successfully." });
        }

        // Update job details
        [HttpPut]
        public IActionResult UpdateJob([FromForm] JobModel job)
        {
            if (job == null || job.JobID == 0)
            {
                return BadRequest(new { message = "Invalid job data." });
            }

            bool isUpdated = _jobRepository.Update(job);
            if (!isUpdated)
            {
                return BadRequest(new { message = "Failed to update job." });
            }
            return Ok(new { message = "Job updated successfully." });
        }

        // Delete job by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteJob(int id)
        {
            bool isDeleted = _jobRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound(new { message = "Job not found." });
            }
            return Ok(new { message = "Job deleted successfully." });
        }

        [HttpGet("top10")]
        public ActionResult<IEnumerable<JobModel>> GetTop10Jobs()
        {
            try
            {
                var jobs = _jobRepository.SelectTop10();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        #region GetJobCount
        [HttpGet]
        public IActionResult GetJobCount()
        {
            int count = _jobRepository.Count();
            return Ok(new { success = true, count });
        }
        #endregion

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve job data as a DataTable from the repository
            DataTable dataTable = _jobRepository.GetJobsDataTable();

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Jobs");

                // Add header row (adjust the header names as needed)
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

                // Populate worksheet with data from the DataTable
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
    }
}
