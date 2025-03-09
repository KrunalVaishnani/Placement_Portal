using System.ComponentModel.DataAnnotations;

namespace Placement_Portal.Areas.Student.Models
{
    public class CompanyModel
    {
        [Key]
        public int CompanyID { get; set; } // Primary Key, not nullable

        [Required]  
        [StringLength(100)]
        public string CompanyName { get; set; } // Not nullable, nvarchar(100)

        [Required]
        [StringLength(15)]
        public string Contact_No { get; set; } // Not nullable, nvarchar(15)

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } // Nullable, nvarchar(100)

        public string Address { get; set; } // Nullable, nvarchar(MAX)

        public long? Pincode { get; set; } // Nullable, bigint

        public string? Image { get; set; } // Not nullable, nvarchar(MAX)
    }
}
