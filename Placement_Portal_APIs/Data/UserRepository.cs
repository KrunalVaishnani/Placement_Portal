using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Placement_Portal_APIs.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Placement_Portal_APIs.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAll
        public IEnumerable<UserModel> SelectAll()
        {
            var users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new UserModel
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            UserName = reader["UserName"].ToString(),
                            Contact_No = reader["Contact_No"]?.ToString(),
                            Gender = reader["Gender"]?.ToString(),
                            DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(),
                            Image = reader["Image"]?.ToString()
                        };

                        // Check if "Role" exists before accessing it
                        if (HasColumn(reader, "Role"))
                        {
                            user.Role = reader["Role"]?.ToString();
                        }
                        else
                        {
                            user.Role = null;
                        }

                        users.Add(user);
                    }
                }
            }
            return users;
        }

        private bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

        #region SelectByPk
        public UserModel SelectByPk(int userID)
        {
            UserModel user = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            UserName = reader["UserName"].ToString(),
                            Contact_No = reader["Contact_No"]?.ToString(),
                            Gender = reader["Gender"]?.ToString(),
                            DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(),
                            Image = reader["Image"]?.ToString()
                        };

                        if (HasColumn(reader, "Role"))
                        {
                            user.Role = reader["Role"]?.ToString();
                        }
                    }
                }
            }
            return user;
        }
        #endregion

        #region Add
        public bool Add(UserModel user)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO [User] (UserName, Contact_No, Gender, DateOfBirth, Email, Password, Image, Role) " +
                    "VALUES (@UserName, @Contact_No, @Gender, @DateOfBirth, @Email, @Password, @Image, @Role)", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Contact_No", user.Contact_No ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", user.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Image", user.Image ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", user.Role ?? (object)DBNull.Value);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Update
        public bool Update(UserModel user)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE [User] SET UserName = @UserName, Contact_No = @Contact_No, Gender = @Gender, " +
                    "DateOfBirth = @DateOfBirth, Email = @Email, Password = @Password, Image = @Image, Role = @Role " +
                    "WHERE UserID = @UserID", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@UserID", user.UserID);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Contact_No", user.Contact_No ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", user.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Image", user.Image ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", user.Role ?? (object)DBNull.Value);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Delete
        public bool Delete(int userID)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region LoginUser
        public DataTable LoginUser(string userName, string password, string role)
        {
            DataTable userTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_User_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        userTable.Load(reader);
                    }
                }
            }

            return userTable;
        }
        #endregion

        #region RegisterUser
        public DataTable RegisterUser(string userName, string password, string email, string contactNo, string role)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PR_User_Register", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Contact_No", contactNo);
                        cmd.Parameters.AddWithValue("@Role", role);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            resultTable.Load(reader);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Exception: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                throw;
            }

            return resultTable;
        }
        #endregion

        #region GetTop10Users
        public IEnumerable<UserModel> GetTop10Users()
        {
            var users = new List<UserModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_SelectTop10", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Contact_No = reader["Contact_No"]?.ToString(),
                        Role = reader["Role"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                    });
                }
            }

            return users;
        }
        #endregion

        #region GetUserCount
        public int GetUserCount()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_Count", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }

            return count;
        }
        #endregion
    }
}
