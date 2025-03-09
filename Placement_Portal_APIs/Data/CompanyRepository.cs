using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Placement_Portal_APIs.Data
{
    public class CompanyRepository
    {
        private readonly string _connectionString;
        private readonly string _imageDirectory = Path.Combine("Uploads", "CompanyImages");

        public CompanyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<CompanyModel> SelectAll()
        {
            var companies = new List<CompanyModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    companies.Add(new CompanyModel
                    {
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        CompanyName = reader["CompanyName"].ToString(),
                        Contact_No = reader["Contact_No"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        Pincode = reader["Pincode"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    });
                }
            }
            return companies;
        }
        #endregion

        #region SelectByPk
        public CompanyModel SelectByPk(int companyID)
        {
            CompanyModel company = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_SelectByPk", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CompanyID", companyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    company = new CompanyModel
                    {
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        CompanyName = reader["CompanyName"].ToString(),
                        Contact_No = reader["Contact_No"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        Pincode = reader["Pincode"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    };
                }
            }
            return company;
        }
        #endregion

        public bool Add(CompanyModel company, IFormFile? imageFile)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_Company_Insert", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Handle Image Upload
                    company.Image = imageFile != null ? SaveImage(imageFile) : null;

                    cmd.Parameters.AddWithValue("@CompanyName", company.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Contact_No", company.Contact_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", company.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", company.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pincode", company.Pincode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Image", company.Image ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Fix: Consider the operation successful if rowsAffected is not -1
                    return rowsAffected > 0 || rowsAffected == -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public bool Update(CompanyModel company, IFormFile? imageFile)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // If a new image is provided, update the image path; otherwise, retain the existing image
                company.Image = SaveImage(imageFile) ?? company.Image;

                cmd.Parameters.AddWithValue("@CompanyID", company.CompanyID);
                cmd.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                cmd.Parameters.AddWithValue("@Contact_No", company.Contact_No);
                cmd.Parameters.AddWithValue("@Email", company.Email);
                cmd.Parameters.AddWithValue("@Address", company.Address);
                cmd.Parameters.AddWithValue("@Pincode", company.Pincode);
                cmd.Parameters.AddWithValue("@Image", (object?)company.Image ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        #region Delete
        public bool Delete(int companyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CompanyID", companyID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        public IEnumerable<CompanyModel> GetCompanyDropdown()
        {
            var companies = new List<CompanyModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_Company_Dropdown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    companies.Add(new CompanyModel
                    {
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        CompanyName = reader["CompanyName"].ToString()
                    });
                }
            }
            return companies;
        }


        private string SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null) return null;

            string imageName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            string imagePath = Path.Combine(_imageDirectory, imageName);

            // Ensure directory exists
            Directory.CreateDirectory(_imageDirectory);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return imageName;
        }


        #region Get Image Path
        public string GetImagePath(string fileName)
        {
            string filePath = Path.Combine(_imageDirectory, fileName);
            return File.Exists(filePath) ? filePath : null;
        }
        #endregion

        public IEnumerable<CompanyModel> GetTop10Companies()
        {
            var companies = new List<CompanyModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_SelectTop10", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    companies.Add(new CompanyModel
                    {
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        CompanyName = reader["CompanyName"].ToString(),
                        Contact_No = reader["Contact_No"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        Pincode = reader["Pincode"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    });
                }
            }
            return companies;
        }

        public int GetCompanyCount()
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Company_Count", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        public DataTable GetCompaniesDataTable()
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_Company_SelectAll", conn))
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
