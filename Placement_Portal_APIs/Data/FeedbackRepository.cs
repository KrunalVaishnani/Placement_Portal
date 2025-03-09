using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Data
{
    public class FeedbackRepository
    {
        private readonly string _connectionString;

        public FeedbackRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<FeedbackModel> SelectAll()
        {
            var feedbackList = new List<FeedbackModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Feedback_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    feedbackList.Add(new FeedbackModel
                    {
                        FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        FeedbackText = reader["FeedbackText"]?.ToString(),
                        Rating = reader["Rating"] != DBNull.Value ? Convert.ToInt32(reader["Rating"]) : (int?)null,
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }
            return feedbackList;
        }
        #endregion

        #region SelectByPk
        public FeedbackModel SelectByPk(int feedbackID)
        {
            FeedbackModel feedback = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Feedback_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@FeedbackID", feedbackID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    feedback = new FeedbackModel
                    {
                        FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        FeedbackText = reader["FeedbackText"]?.ToString(),
                        Rating = reader["Rating"] != DBNull.Value ? Convert.ToInt32(reader["Rating"]) : (int?)null,
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                }
            }
            return feedback;
        }
        #endregion

        #region Add
        public bool Add(FeedbackModel feedback)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Feedback (UserID, FeedbackText, Rating, CreatedDate) " +
                    "VALUES (@UserID, @FeedbackText, @Rating, @CreatedDate)", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@UserID", feedback.UserID);
                cmd.Parameters.AddWithValue("@FeedbackText", feedback.FeedbackText ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Rating", feedback.Rating ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedDate", feedback.CreatedDate);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Update
        public bool Update(FeedbackModel feedback)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Feedback SET UserID = @UserID, FeedbackText = @FeedbackText, Rating = @Rating, CreatedDate = @CreatedDate " +
                    "WHERE FeedbackID = @FeedbackID", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@FeedbackID", feedback.FeedbackID);
                cmd.Parameters.AddWithValue("@UserID", feedback.UserID);
                cmd.Parameters.AddWithValue("@FeedbackText", feedback.FeedbackText ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Rating", feedback.Rating ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedDate", feedback.CreatedDate);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Delete
        public bool Delete(int feedbackID)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Feedback_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@FeedbackID", feedbackID);
                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion
    }
}
