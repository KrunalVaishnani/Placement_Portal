using System.ComponentModel.DataAnnotations;

namespace Placement_Portal_APIs.Models;
public class StudentModel
{
    [Key]
    public int? StudentID { get; set; } // Primary Key

    [Required]
    [StringLength(100)]
    public string StudentName { get; set; } // Not Null

    [Required]
    public long Enrollment_No { get; set; } // Not Null

    [StringLength(10)]
    public string Gender { get; set; } // Nullable

    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } // Nullable

    [StringLength(15)]
    public string Phone_No { get; set; } // Nullable

    public string Address { get; set; } // Nullable (nvarchar(MAX))

    public DateTime? DateOfBirth { get; set; } // Nullable

    [StringLength(15)]
    public string Contact_No { get; set; } // Nullable

    [Required]
    public string Image { get; set; } // Not Null (nvarchar(MAX))
}
