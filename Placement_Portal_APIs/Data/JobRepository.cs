using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Placement_Portal_APIs.Data
{
    public class JobRepository
    {
        private readonly string _connectionString;

        public JobRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<JobModel> SelectAll()
        {
            var jobs = new List<JobModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_SelectAll", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(MapJob(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching jobs.", ex);
            }
            return jobs;
        }
        #endregion

        #region SelectByPk
        public JobModel SelectByPk(int jobID)
        {
            JobModel job = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_SelectByPK", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@JobID", jobID);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            job = MapJob(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching the job.", ex);
            }
            return job;
        }
        #endregion

        #region Add
        public bool Add(JobModel job)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_Insert", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Adding parameters, handling null values properly
                    cmd.Parameters.AddWithValue("@JobTitle", job.JobTitle ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    //cmd.Parameters.AddWithValue("@CompanyName", job.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Contact_No", job.Contact_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", job.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Position", job.Position ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JobProfile", job.JobProfile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfDrive", job.DateOfDrive ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalaryPerMonth", job.SalaryPerMonth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Location", job.Location ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Returning success if rowsAffected is positive or -1
                    return rowsAffected > 0 || rowsAffected == -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Update
        public bool Update(JobModel job)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_Update", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@JobID", job.JobID);
                    cmd.Parameters.AddWithValue("@JobTitle", job.JobTitle ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    cmd.Parameters.AddWithValue("@Contact_No", job.Contact_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", job.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Position", job.Position ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JobProfile", job.JobProfile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfDrive", job.DateOfDrive ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalaryPerMonth", job.SalaryPerMonth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Location", job.Location ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 || rowsAffected == -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int jobID)
        {
            int effect = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_DeleteByPK", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@JobID", jobID);
                    conn.Open();
                    effect = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the job.", ex);
            }
            return effect > 0;
        }
        #endregion

        #region ApplyForJob
        public bool ApplyForJob(int jobId, int studentId, string studentName, string gender, DateTime dateOfBirth, string contact_No, string email, int companyID)
        {
            int effect = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_JobApplications_Insert", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@JobID", jobId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@StudentName", studentName);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    cmd.Parameters.AddWithValue("@Contact_No", contact_No);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@CompanyID", companyID);
                    conn.Open();
                    effect = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying for the job.", ex);
            }
            return effect > 0;
        }
        #endregion

        #region Helper Methods
        private JobModel MapJob(SqlDataReader reader)
        {
            return new JobModel
            {
                JobID = Convert.ToInt32(reader["JobID"]),
                JobTitle = reader["JobTitle"].ToString(),
                CompanyID = Convert.ToInt32(reader["CompanyID"]),
                CompanyName = reader["CompanyName"].ToString(),
                Contact_No = reader["Contact_No"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Position = reader["Position"]?.ToString(),
                JobProfile = reader["JobProfile"]?.ToString(),
                DateOfDrive = reader["DateOfDrive"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfDrive"]) : (DateTime?)null,
                SalaryPerMonth = reader["SalaryPerMonth"] != DBNull.Value ? Convert.ToDecimal(reader["SalaryPerMonth"]) : (decimal?)null,
                Location = reader["Location"].ToString(),
            };
        }

        private void AddJobParameters(SqlCommand cmd, JobModel job)
        {
            cmd.Parameters.AddWithValue("@JobTitle", job.JobTitle);
            cmd.Parameters.AddWithValue("@CompanyID", job.CompanyID);
            cmd.Parameters.AddWithValue("@Contact_No", job.Contact_No ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", job.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Position", job.Position ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JobProfile", job.JobProfile ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DateOfDrive", job.DateOfDrive ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SalaryPerMonth", job.SalaryPerMonth ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", job.Location);
        }
        #endregion

        #region SelectTop10
        public IEnumerable<JobModel> SelectTop10()
        {
            var jobs = new List<JobModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_SelectTop10", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new JobModel
                            {
                                JobID = Convert.ToInt32(reader["JobID"]),
                                JobTitle = reader["JobTitle"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                Contact_No = reader["Contact_No"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                DateOfDrive = reader["DateOfDrive"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfDrive"]) : (DateTime?)null
                               
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching top 10 jobs.", ex);
            }
            return jobs;
        }
        #endregion

        #region Count
        public int Count()
        {
            int totalCount = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Job_Count", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    totalCount = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while counting the jobs.", ex);
            }
            return totalCount;
        }
        #endregion

        public DataTable GetJobsDataTable()
        {
            DataTable data = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PR_Job_SelectAll", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            data.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching jobs for export.", ex);
            }
            return data;
        }


    }
}
