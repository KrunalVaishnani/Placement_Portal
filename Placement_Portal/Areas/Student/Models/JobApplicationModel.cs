using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Placement_Portal.Areas.Student.Models
{
    public class JobApplicationModel
    {
        public int ApplicationID { get; set; }

        [Required]
        public int JobID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Phone]
        public string Phone_No { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int CompanyID { get; set; }

        public DateTime ApplicationDateTime { get; set; }

        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public int UserID { get; internal set; }
    }
}
