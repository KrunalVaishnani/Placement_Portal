using Microsoft.AspNetCore.Builder;
using Placement_Portal.Areas.Admin.Models;
using Placement_Portal.Models;

namespace Placement_Portal.Areas.Admin.Models
{
    public class Dashboard
    {
        public List<JobModel> jobs { get; set; } = new List<JobModel>();
        public int TotalJobs { get; set; }

        // Student details
        public List<StudentDetailsModel> Students { get; set; } = new List<StudentDetailsModel>();
        public int TotalStudents { get; set; }

        // Company details
        public List<CompanyModel> Companies { get; set; } = new List<CompanyModel>();
        public int TotalCompanies { get; set; }

        // Job Applications
        public List<JobApplicationModel> JobApplications { get; set; } = new List<JobApplicationModel>();
        public int TotalApplications { get; set; }

        // Users
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public int TotalUsers { get; set; }
    }


}