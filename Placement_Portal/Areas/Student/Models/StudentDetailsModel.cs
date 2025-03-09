using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Placement_Portal.Areas.Student.Models
{
    public class StudentDetailsModel
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [StringLength(100)]
        public string StudentName { get; set; }

        [Required]
        [StringLength(50)]
        public string Enrollment_No { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone_No { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; }

        [Required]
        [Range(0, 100)]
        public float SSC_Percentage { get; set; }

        [Required]
        public int YearOfPassingSSC { get; set; }

        [Required]
        [Range(0, 100)]
        public float HSC_Percentage { get; set; }

        [Required]
        public int YearOfPassingHSC { get; set; }

        [Range(0, 10)]
        public float? UG_CGPA { get; set; }

        public int? YearOfPassingUG { get; set; }

        [StringLength(500)]
        public string? Skills { get; set; }

        public string? Status { get; set; }

        public bool IsActive { get; set; }

        [SwaggerIgnore]
        public string? Resume { get; set; }

        [SwaggerIgnore]
        public string? Image { get; set; }
    }
}
