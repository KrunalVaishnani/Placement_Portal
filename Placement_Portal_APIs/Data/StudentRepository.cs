using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Placement_Portal_APIs.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Placement_Portal_APIs.Data
{
    public class StudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<StudentModel> SelectAll()
        {
            var students = new List<StudentModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Student_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = Convert.ToInt64(reader["Enrollment_No"]),
                        Gender = reader["Gender"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Address = reader["Address"].ToString(),
                        DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                        Contact_No = reader["Contact_No"].ToString(),
                        Image = reader["Image"].ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region SelectByPk
        public StudentModel SelectByPk(int studentID)
        {
            StudentModel student = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Student_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student = new StudentModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = Convert.ToInt64(reader["Enrollment_No"]),
                        Gender = reader["Gender"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone_No = reader["Phone_No"].ToString(),
                        Address = reader["Address"].ToString(),
                        DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                        Contact_No = reader["Contact_No"].ToString(),
                        Image = reader["Image"].ToString()
                    };
                }
            }
            return student;
        }
        #endregion

        #region Delete
        public bool Delete(int studentID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Student_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Add
        public bool Add(StudentModel studentModel)
        {
            int effect = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Student (StudentName, Enrollment_No, Gender, Email, Phone_No, Address, DateOfBirth, Contact_No, Image) " +
                        "VALUES (@StudentName, @Enrollment_No, @Gender, @Email, @Phone_No, @Address, @DateOfBirth, @Contact_No, @Image)", conn)
                    {
                        CommandType = CommandType.Text
                    };
                    cmd.Parameters.AddWithValue("@StudentName", studentModel.StudentName);
                    cmd.Parameters.AddWithValue("@Enrollment_No", studentModel.Enrollment_No);
                    cmd.Parameters.AddWithValue("@Gender", studentModel.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", studentModel.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone_No", studentModel.Phone_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", studentModel.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", studentModel.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Contact_No", studentModel.Contact_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Image", studentModel.Image);

                    conn.Open();
                    effect = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return effect > 0;
        }
        #endregion

        #region Update
        public bool Update(StudentModel studentModel)
        {
            int effect = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Student SET StudentName = @StudentName, Enrollment_No = @Enrollment_No, Gender = @Gender, Email = @Email, " +
                        "Phone_No = @Phone_No, Address = @Address, DateOfBirth = @DateOfBirth, Contact_No = @Contact_No, Image = @Image " +
                        "WHERE StudentID = @StudentID", conn)
                    {
                        CommandType = CommandType.Text
                    };
                    cmd.Parameters.AddWithValue("@StudentID", studentModel.StudentID);
                    cmd.Parameters.AddWithValue("@StudentName", studentModel.StudentName);
                    cmd.Parameters.AddWithValue("@Enrollment_No", studentModel.Enrollment_No);
                    cmd.Parameters.AddWithValue("@Gender", studentModel.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", studentModel.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone_No", studentModel.Phone_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", studentModel.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", studentModel.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Contact_No", studentModel.Contact_No ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Image", studentModel.Image);

                    conn.Open();
                    effect = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return effect > 0;
        }
        #endregion
    }
}
