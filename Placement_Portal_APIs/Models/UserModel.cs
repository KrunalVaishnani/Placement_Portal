using System;
using System.ComponentModel.DataAnnotations;

namespace Placement_Portal_APIs.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; } // Primary Key, not nullable

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } // Not nullable, nvarchar(100)

        [StringLength(15)]
        public string Contact_No { get; set; } // Nullable, nvarchar(15)

        [StringLength(10)]
        public string Gender { get; set; } // Nullable, nvarchar(10)

        public DateTime? DateOfBirth { get; set; } // Nullable, date

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } // Not nullable, nvarchar(100)

        [Required]
        [StringLength(100)]
        public string Password { get; set; } // Not nullable, nvarchar(100)

        public string Image { get; set; } // Nullable, nvarchar(MAX)
        public string Role { get; set; }
    }
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        public string Contact_No { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Role { get; set; }
    }
}
