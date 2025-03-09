using Placement_Portal_APIs.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Placement_Portal_APIs.Data
{
    public class StudentDetailsRepository
    {
        private readonly string _connectionString;

        public StudentDetailsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region SelectAll
        public IEnumerable<StudentDetailsModel> SelectAll()
        {
            var students = new List<StudentDetailsModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        SSC_Percentage = Convert.ToSingle(reader["SSC_Percentage"]),
                        YearOfPassingSSC = Convert.ToInt32(reader["YearOfPassingSSC"]),
                        HSC_Percentage = Convert.ToSingle(reader["HSC_Percentage"]),
                        YearOfPassingHSC = Convert.ToInt32(reader["YearOfPassingHSC"]),
                        UG_CGPA = reader["UG_CGPA"] != DBNull.Value ? Convert.ToSingle(reader["UG_CGPA"]) : (float?)null,
                        YearOfPassingUG = reader["YearOfPassingUG"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingUG"]) : (int?)null,
                        Skills = reader["Skills"]?.ToString(),
                        Resume = reader["Resume"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region SelectAllByUserID
        public IEnumerable<StudentDetailsModel> SelectAll(int UserId)
        {
            var students = new List<StudentDetailsModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectByUserID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserId;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        SSC_Percentage = Convert.ToSingle(reader["SSC_Percentage"]),
                        YearOfPassingSSC = Convert.ToInt32(reader["YearOfPassingSSC"]),
                        HSC_Percentage = Convert.ToSingle(reader["HSC_Percentage"]),
                        YearOfPassingHSC = Convert.ToInt32(reader["YearOfPassingHSC"]),
                        UG_CGPA = reader["UG_CGPA"] != DBNull.Value ? Convert.ToSingle(reader["UG_CGPA"]) : (float?)null,
                        YearOfPassingUG = reader["YearOfPassingUG"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingUG"]) : (int?)null,
                        Skills = reader["Skills"]?.ToString(),
                        Resume = reader["Resume"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region GetStudentsWithStatus
        public IEnumerable<StudentDetailsModel> GetStudentsWithStatus()
        {
            var students = new List<StudentDetailsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectAll_Status", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Status = reader["Status"].ToString(),
                        Image = reader["Image"]?.ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region PlacedStudent List
        public IEnumerable<StudentDetailsModel> Placed_Student_All()
        {
            var students = new List<StudentDetailsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[PR_StudentDetails_SelectAll_Placed]", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Status = reader["Status"].ToString(),
                        Image = reader["Image"]?.ToString(),
                       Phone_No = reader["Phone_No"]?.ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region  Not PlacedStudent List
        public IEnumerable<StudentDetailsModel> Not_Placed_Student_All()
        {
            var students = new List<StudentDetailsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[PR_StudentDetails_SelectAll_Not_Placed]", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Status = reader["Status"].ToString(),
                        Image = reader["Image"]?.ToString(),
                        Phone_No = reader["Phone_No"]?.ToString()
                    });
                }
            }
            return students;
        }
        #endregion

        #region SelectByPk
        public StudentDetailsModel SelectByPk(int studentID)
        {
            StudentDetailsModel student = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectByPk", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    student = new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                        Phone_No = reader["Phone_No"].ToString(),
                        Email = reader["Email"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        SSC_Percentage = Convert.ToSingle(reader["SSC_Percentage"]),
                        YearOfPassingSSC = Convert.ToInt32(reader["YearOfPassingSSC"]),
                        HSC_Percentage = Convert.ToSingle(reader["HSC_Percentage"]),
                        YearOfPassingHSC = Convert.ToInt32(reader["YearOfPassingHSC"]),
                        UG_CGPA = reader["UG_CGPA"] != DBNull.Value ? Convert.ToSingle(reader["UG_CGPA"]) : (float?)null,
                        YearOfPassingUG = reader["YearOfPassingUG"] != DBNull.Value ? Convert.ToInt32(reader["YearOfPassingUG"]) : (int?)null,
                        Skills = reader["Skills"]?.ToString(),
                        Resume = reader["Resume"]?.ToString(),
                        Image = reader["Image"]?.ToString()
                    };
                }
            }
            return student;
        }
        #endregion

        public bool Add(StudentDetailsModel student, IFormFile resumeFile, IFormFile imageFile)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Save resume file
                if (resumeFile != null && resumeFile.Length > 0)
                {
                    var resumeDirectory = Path.Combine("Uploads", "Resumes");
                    if (!Directory.Exists(resumeDirectory))
                    {
                        Directory.CreateDirectory(resumeDirectory);
                    }

                    var resumeFileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
                    var resumePath = Path.Combine(resumeDirectory, resumeFileName);
                    using (var stream = new FileStream(resumePath, FileMode.Create))
                    {
                        resumeFile.CopyTo(stream);
                    }
                    student.Resume = resumeFileName;
                }

                // Save image file
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageDirectory = Path.Combine("Uploads", "Images");
                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    var imageFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var imagePath = Path.Combine(imageDirectory, imageFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    student.Image = imageFileName;
                }

                cmd.Parameters.AddWithValue("@StudentName", student.StudentName);
                cmd.Parameters.AddWithValue("@Enrollment_No", student.Enrollment_No);
                cmd.Parameters.AddWithValue("@Gender", student.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone_No", student.Phone_No);
                cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_Percentage", student.SSC_Percentage);
                cmd.Parameters.AddWithValue("@YearOfPassingSSC", student.YearOfPassingSSC);
                cmd.Parameters.AddWithValue("@HSC_Percentage", student.HSC_Percentage);
                cmd.Parameters.AddWithValue("@YearOfPassingHSC", student.YearOfPassingHSC);
                cmd.Parameters.AddWithValue("@UG_CGPA", student.UG_CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingUG", student.YearOfPassingUG ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Skills", student.Skills ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", student.Status ?? "Active");
                cmd.Parameters.AddWithValue("@IsActive", student.IsActive );
                cmd.Parameters.AddWithValue("@Resume", student.Resume ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", student.Image ?? (object)DBNull.Value);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }

        #region Update
        public bool Update(StudentDetailsModel student, IFormFile resumeFile, IFormFile imageFile)
        {
            int effect = 0;
            if (student.StudentID == 0)
            {
                return false; // Prevent updates with invalid ID
            }
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", student.StudentID);


                // Retrieve existing resume and image filenames if new files are not provided
                string existingResume = student.Resume;
                string existingImage = student.Image;

                // Save updated resume file
                if (resumeFile != null && resumeFile.Length > 0)
                {
                    var resumeDirectory = Path.Combine("Uploads", "Resumes");
                    if (!Directory.Exists(resumeDirectory))
                    {
                        Directory.CreateDirectory(resumeDirectory);
                    }

                    var resumeFileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
                    var resumePath = Path.Combine(resumeDirectory, resumeFileName);
                    using (var stream = new FileStream(resumePath, FileMode.Create))
                    {
                        resumeFile.CopyTo(stream);
                    }
                    student.Resume = resumeFileName; // Store only the filename
                }
                else
                {
                    student.Resume = existingResume; // Retain existing value
                }

                // Save updated image file
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageDirectory = Path.Combine("Uploads", "Images");
                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    var imageFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var imagePath = Path.Combine(imageDirectory, imageFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    student.Image = imageFileName; // Store only the filename
                }
                else
                {
                    student.Image = existingImage; // Retain existing value
                }

                //cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
                cmd.Parameters.AddWithValue("@StudentName", student.StudentName);
                cmd.Parameters.AddWithValue("@Enrollment_No", student.Enrollment_No);
                cmd.Parameters.AddWithValue("@Gender", student.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone_No", student.Phone_No);
                cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_Percentage", student.SSC_Percentage);
                cmd.Parameters.AddWithValue("@YearOfPassingSSC", student.YearOfPassingSSC);
                cmd.Parameters.AddWithValue("@HSC_Percentage", student.HSC_Percentage);
                cmd.Parameters.AddWithValue("@YearOfPassingHSC", student.YearOfPassingHSC);
                cmd.Parameters.AddWithValue("@UG_CGPA", student.UG_CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfPassingUG", student.YearOfPassingUG ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Skills", student.Skills ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", student.Status ?? "Active"); // Default to "Active"
                cmd.Parameters.AddWithValue("@IsActive", student.IsActive); // Default to true
                cmd.Parameters.AddWithValue("@Resume", student.Resume ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", student.Image ?? (object)DBNull.Value);

                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }

        #endregion



        #region Delete
        public bool Delete(int studentID)
        {
            int effect = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                conn.Open();
                effect = cmd.ExecuteNonQuery();
            }
            return effect > 0;
        }
        #endregion

        #region Top10Students
        public IEnumerable<StudentDetailsModel> GetTop10Students()
        {
            var students = new List<StudentDetailsModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectTop10", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new StudentDetailsModel
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        StudentName = reader["StudentName"].ToString(),
                        Enrollment_No = reader["Enrollment_No"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                        Phone_No = reader["Phone_No"].ToString(),
                        Status = reader["Status"].ToString()
                    });
                }
            }
            return students;
        }
        #endregion


        #region GetStudentCount
        public int GetStudentCount()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_StudentDetails_Count", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }
        #endregion

        public IEnumerable<StudentDetailsModel> SelectAllFiltered(string studentName, string enrollmentNo)
        {
            var students = new List<StudentDetailsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectAllFiltered", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If parameters are null, pass DBNull.Value to let the SP ignore the filter
                    cmd.Parameters.Add("@StudentName", SqlDbType.VarChar, 100).Value =
                        string.IsNullOrEmpty(studentName) ? (object)DBNull.Value : studentName;
                    cmd.Parameters.Add("@Enrollment_No", SqlDbType.VarChar, 100).Value =
                        string.IsNullOrEmpty(enrollmentNo) ? (object)DBNull.Value : enrollmentNo;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new StudentDetailsModel
                            {
                                StudentID = Convert.ToInt32(reader["StudentID"]),
                                StudentName = reader["StudentName"].ToString(),
                                Enrollment_No = reader["Enrollment_No"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Phone_No = reader["Phone_No"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Skills = reader["Skills"]?.ToString(),
                                Image = reader["Image"]?.ToString(),
                                Resume = reader["Resume"]?.ToString()
                            });
                        }
                    }
                }
            }
            return students;
        }


        public DataTable GetStudentsDataTable()
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Change the stored procedure to one that retrieves all students as needed
                using (SqlCommand cmd = new SqlCommand("PR_StudentDetails_SelectAll", conn))
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
