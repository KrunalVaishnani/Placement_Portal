using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System;
using System.IO;
using OfficeOpenXml;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyRepository _companyRepository;

        public CompanyController(CompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        #region GetAllCompanies
        [HttpGet]
        [Authorize]
        public IActionResult GetAllCompanies()
        {
            var companies = _companyRepository.SelectAll();
            return Ok(companies);
        }
        #endregion

        #region GetCompanyById
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetCompanyById(int id)
        {
            var company = _companyRepository.SelectByPk(id);
            if (company == null)
            {
                return NotFound(new { success = false, message = "Company not found." });
            }
            return Ok(company);
        }
        #endregion

        [HttpPost]
        [Authorize]

        public IActionResult AddCompany([FromForm] CompanyModel company, IFormFile? imageFile)
        {
            try
            {
                bool isAdded = _companyRepository.Add(company, imageFile);

                if (isAdded)
                {
                    return Ok(new { success = true, message = "Company added successfully." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Unexpected error occurred, but data might be inserted." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal Server Error", error = ex.Message });
            }
        }


        #region UpdateCompany
        [HttpPut]
        [Authorize]

        public IActionResult UpdateCompany([FromForm] CompanyModel company, IFormFile? imageFile)
        {
            bool isUpdated = _companyRepository.Update(company, imageFile);
            if (!isUpdated)
            {
                return BadRequest(new { success = false, message = "Failed to update company." });
            }
            return Ok(new { success = true, message = "Company updated successfully." });
        }
        #endregion

        #region DeleteCompany
        [Authorize]

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(int id)
        {
            bool isDeleted = _companyRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound(new { success = false, message = "Company not found." });
            }
            return NoContent();
        }
        #endregion
        [Authorize]

        [HttpGet("dropdown")]
        public IActionResult GetCompanyDropdown()
        {
            var companies = _companyRepository.GetCompanyDropdown();
            return Ok(companies);
        }

        #region GetCompanyImage
        [HttpGet("Image/{fileName}")]
        public IActionResult GetCompanyImage(string fileName)
        {
            var filePath = _companyRepository.GetImagePath(fileName);
            if (filePath == null)
            {
                return NotFound(new { success = false, message = "Image not found." });
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
        #endregion

        #region GetTop10Companies
        [Authorize]

        [HttpGet]
        public IActionResult GetTop10Companies()
        {
            var companies = _companyRepository.GetTop10Companies();
            return Ok(companies);
        }
        #endregion

        #region GetCompanyCount
        [Authorize]

        [HttpGet]
        public IActionResult GetCompanyCount()
        {
            int count = _companyRepository.GetCompanyCount();
            return Ok(new { success = true, count });
        }
        #endregion

        [HttpGet]
        [Authorize]

        public IActionResult ExportToExcel()
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Retrieve company data as a DataTable from the repository
            DataTable dataTable = _companyRepository.GetCompaniesDataTable();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Companies");

                // Add header row – adjust these column names as necessary.
                worksheet.Cells[1, 1].Value = "CompanyID";
                worksheet.Cells[1, 2].Value = "CompanyName";
                worksheet.Cells[1, 3].Value = "Contact_No";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Pincode";

                // Populate worksheet with data from the DataTable
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

                // Auto-fit columns for a better presentation
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the Excel package into a MemoryStream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Generate a unique file name based on current date/time
                string excelName = $"CompaniesData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
