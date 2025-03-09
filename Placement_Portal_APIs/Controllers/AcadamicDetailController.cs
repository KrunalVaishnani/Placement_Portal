using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System.Data;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class AcadamicDetailController : ControllerBase
    {
        private readonly AcadamicDetailRepository _academicDetailRepository;

        public AcadamicDetailController(AcadamicDetailRepository academicDetailRepository)
        {
            _academicDetailRepository = academicDetailRepository;
        }

        [HttpGet]
        public IActionResult GetAllAcadamicDetails()
        {
            var academicDetails = _academicDetailRepository.SelectAll();
            return Ok(academicDetails);
        }

        [HttpGet("{id}")]
        public IActionResult GetAcadamicDetailById(int id)
        {
            var academicDetail = _academicDetailRepository.SelectByPk(id);
            if (academicDetail == null)
            {
                return NotFound("Academic detail not found.");
            }
            return Ok(academicDetail);
        }

        [HttpPost]
        public IActionResult AddAcadamicDetail(AcadamicDetailModel academicDetail)
        {
            var isAdded = _academicDetailRepository.Add(academicDetail);
            if (!isAdded)
            {
                return BadRequest("Failed to add academic detail.");
            }
            return Ok("Academic detail added successfully.");
        }

        [HttpPut]
        public IActionResult UpdateAcadamicDetail(AcadamicDetailModel academicDetail)
        {
            var isUpdated = _academicDetailRepository.Update(academicDetail);
            if (!isUpdated)
            {
                return BadRequest("Failed to update academic detail.");
            }
            return Ok("Academic detail updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAcadamicDetail(int id)
        {
            var isDeleted = _academicDetailRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound("Academic detail not found.");
            }
            return NoContent();
        }
        [HttpGet]
        public IActionResult ExportAcademicDetailsToExcel()
        {
            // Ensure EPPlus license context is set appropriately.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve academic details as a DataTable from the repository
            DataTable dataTable = _academicDetailRepository.GetAcademicDetailsDataTable();

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("AcademicDetails");

                // Add header row (adjust the header names to match your DataTable columns)
                worksheet.Cells[1, 1].Value = "AcademicID";
                worksheet.Cells[1, 2].Value = "StudentID";
                worksheet.Cells[1, 3].Value = "SSC_Percentage";
                worksheet.Cells[1, 4].Value = "YearOfPassingSSC";
                worksheet.Cells[1, 5].Value = "HSC_Percentage";
                worksheet.Cells[1, 6].Value = "YearOfPassingHSC";
                worksheet.Cells[1, 7].Value = "UG_CGPA";
                worksheet.Cells[1, 8].Value = "YearOfPassingUG";
                worksheet.Cells[1, 9].Value = "Skills";
                worksheet.Cells[1, 10].Value = "Resume";
                worksheet.Cells[1, 11].Value = "Image";

                // Populate worksheet with data from the DataTable
                int row = 2;
                foreach (DataRow dr in dataTable.Rows)
                {
                    worksheet.Cells[row, 1].Value = dr["AcademicID"];
                    worksheet.Cells[row, 2].Value = dr["StudentID"];
                    worksheet.Cells[row, 3].Value = dr["SSC_Percentage"];
                    worksheet.Cells[row, 4].Value = dr["YearOfPassingSSC"];
                    worksheet.Cells[row, 5].Value = dr["HSC_Percentage"];
                    worksheet.Cells[row, 6].Value = dr["YearOfPassingHSC"];
                    worksheet.Cells[row, 7].Value = dr["UG_CGPA"];
                    worksheet.Cells[row, 8].Value = dr["YearOfPassingUG"];
                    worksheet.Cells[row, 9].Value = dr["Skills"];
                    worksheet.Cells[row, 10].Value = dr["Resume"];
                    worksheet.Cells[row, 11].Value = dr["Image"];
                    row++;
                }

                // Auto-fit columns for better presentation
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the Excel package to a stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"AcademicDetails-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}