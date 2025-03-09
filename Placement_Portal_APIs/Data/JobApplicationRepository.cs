using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Data
{
    public class JobApplicationRepository
    {
        private readonly string _connectionString;

        public JobApplicationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectByUserID
        public IEnumerable<JobApplicationModel> SelectByUserID(int UserId)
        {
            var jobApplications = new List<JobApplicationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_SelectByUserID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserId;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jobApplications.Add(new JobApplicationModel
                    {
                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                        StudentName = reader["StudentName"].ToString(),
                        JobTitle = reader["JobTitle"].ToString(),
                        CompanyName = reader["CompanyName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"].ToString(),
                        ApplicationDateTime = Convert.ToDateTime(reader["ApplicationDateTime"])
                    });
                }
            }
            return jobApplications;
        }
        #endregion

        #region SelectAll
        public IEnumerable<JobApplicationModel> SelectAll()
        {
            var jobApplications = new List<JobApplicationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //cmd.Parameters.AddWithValue("@UserID", CommonVariable.UserID());
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jobApplications.Add(new JobApplicationModel
                    {
                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                        StudentName = reader["StudentName"].ToString(),
                        JobTitle = reader["JobTitle"].ToString(),
                        CompanyName = reader["CompanyName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"].ToString(),
                        ApplicationDateTime = Convert.ToDateTime(reader["ApplicationDateTime"])
                    });
                }
            }
            return jobApplications;
        }
        #endregion

        #region SelectByPk
        public JobApplicationModel SelectByPk(int applicationID)
        {
            JobApplicationModel jobApplication = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ApplicationID", applicationID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    jobApplication = new JobApplicationModel
                    {
                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                        StudentName = reader["StudentName"].ToString(),
                        JobTitle = reader["JobTitle"].ToString(),
                        CompanyName = reader["CompanyName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"].ToString(),
                        ApplicationDateTime = Convert.ToDateTime(reader["ApplicationDateTime"])
                    };
                }
            }
            return jobApplication;
        }
        #endregion



        public bool Add(JobApplicationModel jobApplication, int UserId)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplication_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@JobID", jobApplication.JobID);
                cmd.Parameters.AddWithValue("@StudentID", jobApplication.StudentID);
                cmd.Parameters.AddWithValue("@CompanyID", jobApplication.CompanyID);
                cmd.Parameters.AddWithValue("@ApplicationDateTime", jobApplication.ApplicationDateTime);
                cmd.Parameters.AddWithValue("@UserID", UserId);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        public int GetCompanyIdByJobId(int jobId)
        {
            int companyId = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CompanyID FROM Job WHERE JobID = @JobID", conn);
                cmd.Parameters.AddWithValue("@JobID", jobId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    companyId = Convert.ToInt32(result);
                }
            }
            return companyId;
        }


        #region Update
        public bool Update(JobApplicationModel jobApplication)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ApplicationID", jobApplication.ApplicationID);
                cmd.Parameters.AddWithValue("@JobID", jobApplication.JobID);
                cmd.Parameters.AddWithValue("@StudentID", jobApplication.StudentID);
                cmd.Parameters.AddWithValue("@CompanyID", jobApplication.CompanyID);
                cmd.Parameters.AddWithValue("@ApplicationDateTime", jobApplication.ApplicationDateTime);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Delete
        public bool Delete(int applicationID)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ApplicationID", applicationID);
                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region SelectTop10
        public IEnumerable<JobApplicationModel> SelectTop10()
        {
            var jobApplications = new List<JobApplicationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_SelectTop10", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jobApplications.Add(new JobApplicationModel
                    {
                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                        StudentName = reader["StudentName"].ToString(),
                        JobTitle = reader["JobTitle"].ToString(),
                        CompanyName = reader["CompanyName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"].ToString(),
                        ApplicationDateTime = Convert.ToDateTime(reader["ApplicationDateTime"])
                    });
                }
            }
            return jobApplications;
        }
        #endregion

        #region Count
        public int Count()
        {
            int totalCount = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_JobApplications_Count", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                totalCount = (int)cmd.ExecuteScalar();
            }
            return totalCount;
        }
        #endregion
       

        public DataTable GetJobApplicationsDataTable()
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Change the stored procedure to one that retrieves all students as needed
                using (SqlCommand cmd = new SqlCommand("PR_JobApplications_SelectAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Load(reader);
                    }
                }
            }
            return data;
        }

    }
}
