using System.ComponentModel.DataAnnotations;

namespace Placement_Portal_APIs.Models
{
    public class JobApplicationModel
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        public int JobID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public DateTime ApplicationDateTime { get; set; }
        public string? StudentName { get;  set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? Gender { get; set; }
        public string? Phone_No { get; set; }
        public string? Email { get; set; }
        public int UserID { get; set; }
    }
}