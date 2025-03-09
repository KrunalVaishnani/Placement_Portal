using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Placement_Portal.Areas.Admin.Models
{
    public class JobModel
    {
        public int? JobID { get; set; } // Primary Key, not nullable

        [Required]
        [StringLength(100)]
        public string? JobTitle { get; set; } // Not nullable, nvarchar(100)

        [Required]
        public int CompanyID { get; set; } // Foreign Key, not nullable
        public string? CompanyName { get; set; }

        [StringLength(15)]
        public string? Contact_No { get; set; } // Nullable, nvarchar(15)

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; } // Nullable, nvarchar(100)

        [StringLength(100)]
        public string? Position { get; set; } // Nullable, nvarchar(100)

        public string? JobProfile { get; set; } // Nullable, nvarchar(MAX)

        public DateTime? DateOfDrive { get; set; } // Nullable, date

        [Range(0, double.MaxValue)]
        public decimal? SalaryPerMonth { get; set; } // Nullable, decimal(10, 2)

        public string? Location { get; set; } // Nullable, nvarchar(MAX)
    }
    public class CompanyDropDownModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
}
