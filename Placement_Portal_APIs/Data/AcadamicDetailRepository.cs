using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Data
{
    public class AcadamicDetailRepository
    {
        private readonly string _connectionString;

        public AcadamicDetailRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<AcadamicDetailModel> SelectAll()
        {
            var academicDetails = new List<AcadamicDetailModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_AcademicDetails_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    academicDetails.Add(new AcadamicDetailModel
                    {
                        AcademicID = Convert.ToInt32(reader["AcademicID"]),
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        SSC_Percentage = reader["SSC_Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["SSC_Percentage"]) : (decimal?)null,
                        YearOfPassingSSC = reader["YearOfPassingSSC"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingSSC"]) : (int?)null,
                        HSC_Percentage = reader["HSC_Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["HSC_Percentage"]) : (decimal?)null,
                        YearOfPassingHSC = reader["YearOfPassingHSC"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingHSC"]) : (int?)null,
                        UG_CGPA = reader["UG_CGPA"] != DBNull.Value ? Convert.ToDecimal(reader["UG_CGPA"]) : (decimal?)null,
                        YearOfPassingUG = reader["YearOfPassingUG"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingUG"]) : (int?)null,
                        Skills = reader["Skills"]?.ToString(),
                        Resume = reader["Resume"]?.ToString(),
                        Image = reader["Image"].ToString()
                    });
                }
            }
            return academicDetails;
        }
        #endregion

        #region SelectByPk
        public AcadamicDetailModel SelectByPk(int academicID)
        {
            AcadamicDetailModel academicDetail = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_AcademicDetails_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AcademicID", academicID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    academicDetail = new AcadamicDetailModel
                    {
                        AcademicID = Convert.ToInt32(reader["AcademicID"]),
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        SSC_Percentage = reader["SSC_Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["SSC_Percentage"]) : (decimal?)null,
                        YearOfPassingSSC = reader["YearOfPassingSSC"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingSSC"]) : (int?)null,
                        HSC_Percentage = reader["HSC_Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["HSC_Percentage"]) : (decimal?)null,
                        YearOfPassingHSC = reader["YearOfPassingHSC"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingHSC"]) : (int?)null,
                        UG_CGPA = reader["UG_CGPA"] != DBNull.Value ? Convert.ToDecimal(reader["UG_CGPA"]) : (decimal?)null,
                        YearOfPassingUG = reader["YearOfPassingUG"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingUG"]) : (int?)null,
                        Skills = reader["Skills"]?.ToString(),
                        Resume = reader["Resume"]?.ToString(),
                        Image = reader["Image"].ToString()
                    };
                }
            }
            return academicDetail;
        }
        #endregion

        #region Add
        public bool Add(AcadamicDetailModel academicDetail)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_AcademicDetails_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", academicDetail.StudentID);
                cmd.Parameters.AddWithValue("@SSC_Percentage", academicDetail.SSC_Percentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingSSC", academicDetail.YearOfPassingSSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HSC_Percentage", academicDetail.HSC_Percentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingHSC", academicDetail.YearOfPassingHSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_CGPA", academicDetail.UG_CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingUG", academicDetail.YearOfPassingUG ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Skills", academicDetail.Skills ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resume", academicDetail.Resume ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", academicDetail.Image);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Update
        public bool Update(AcadamicDetailModel academicDetail)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_AcademicDetails_UpdateByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AcademicID", academicDetail.AcademicID);
                cmd.Parameters.AddWithValue("@StudentID", academicDetail.StudentID);
                cmd.Parameters.AddWithValue("@SSC_Percentage", academicDetail.SSC_Percentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingSSC", academicDetail.YearOfPassingSSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HSC_Percentage", academicDetail.HSC_Percentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingHSC", academicDetail.YearOfPassingHSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_CGPA", academicDetail.UG_CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingUG", academicDetail.YearOfPassingUG ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Skills", academicDetail.Skills ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resume", academicDetail.Resume ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", academicDetail.Image);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Delete
        public bool Delete(int academicID)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_AcademicDetails_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AcademicID", academicID);
                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        public DataTable GetAcademicDetailsDataTable()
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_AcademicDetails_SelectAll", conn))
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
