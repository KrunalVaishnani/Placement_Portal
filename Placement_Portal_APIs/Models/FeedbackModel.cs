using System;
using System.ComponentModel.DataAnnotations;

namespace Placement_Portal_APIs.Models
{
    public class FeedbackModel
    {
        [Key]
        public int FeedbackID { get; set; } // Primary Key, not nullable

        [Required]
        public int UserID { get; set; } // Foreign Key to User table, not nullable

        public string FeedbackText { get; set; } // Nullable, nvarchar(MAX)

        public int? Rating { get; set; } // Nullable, int

        [Required]
        public DateTime CreatedDate { get; set; } // Not nullable, datetime
    }
}
