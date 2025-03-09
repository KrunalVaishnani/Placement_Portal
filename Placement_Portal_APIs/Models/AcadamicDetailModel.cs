using System.ComponentModel.DataAnnotations;

namespace Placement_Portal_APIs.Models
{
    public class AcadamicDetailModel
    {
        [Key]
        public int? AcademicID { get; set; } // Primary Key, not nullable

        [Required]
        public int StudentID { get; set; } // Foreign Key, not nullable

        [Range(0, 100)]
        public decimal? SSC_Percentage { get; set; } // Nullable, decimal(5,2)

        public int? YearOfPassingSSC { get; set; } // Nullable int for SSC

        [Range(0, 100)]
        public decimal? HSC_Percentage { get; set; } // Nullable, decimal(5,2)

        public int? YearOfPassingHSC { get; set; } // Nullable int for HSC

        [Range(0, 10)]
        public decimal? UG_CGPA { get; set; } // Nullable, decimal(5,2)

        public int? YearOfPassingUG { get; set; } // Nullable int for UG

        public string Skills { get; set; } // Nullable, nvarchar(MAX)

        public string Resume { get; set; } // Nullable, nvarchar(MAX)

        [Required]
        public string Image { get; set; } // Not nullable, nvarchar(MAX)
    }


}
