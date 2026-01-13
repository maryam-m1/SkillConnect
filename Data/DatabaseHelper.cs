using System.Data.SqlClient;
using SkillConnect.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace SkillConnect.Data
{
    public class DatabaseHelper
    {
        private readonly string connectionString = string.Empty;

        public DatabaseHelper(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection") ??
                               throw new ArgumentNullException("DefaultConnection string is missing.");
        }

        public int RegisterUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO Users (FullName, Email, Password, EducationLevel) " +
                                   "VALUES (@FullName, @Email, @Password, @EducationLevel); " +
                                   "SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // CRITICAL FIX: Trim all inputs to prevent hidden whitespace errors
                    cmd.Parameters.AddWithValue("@FullName", user.FullName.Trim());
                    cmd.Parameters.AddWithValue("@Email", user.Email.Trim());
                    cmd.Parameters.AddWithValue("@Password", user.Password.Trim());
                    cmd.Parameters.AddWithValue("@EducationLevel", user.EducationLevel);

                    object newId = cmd.ExecuteScalar();

                    if (newId != null && newId != DBNull.Value)
                    {
                        return Convert.ToInt32(newId);
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during registration: " + ex.Message);
                return -1;
            }
        }

        public User LoginUser(string email, string password)
        {
            User user = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT UserId, FullName, Email, EducationLevel " +
                                   "FROM Users WHERE Email = @Email AND Password = @Password";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // CRITICAL FIX: Trim inputs here too, matching the saved data format
                    cmd.Parameters.AddWithValue("@Email", email.Trim());
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                EducationLevel = reader.GetString(reader.GetOrdinal("EducationLevel"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login: " + ex.Message);
            }
            return user;
        }

        // UPDATED: Now supports 6 skill dimensions in the database insertion
        public bool SaveQuizResult(QuizResult result)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Corrected SQL Query: Now supports 6 dimensions
                    string query = @"INSERT INTO QuizResults (UserId, AnalyticalScore, CreativeScore, 
                                                              DataScore, ProgrammingScore, CommunicationScore, 
                                                              PeopleScore, RecommendedCareer, TestDate) 
                                     VALUES (@UserId, @Analytical, @Creative, @Data, @Programming, @Communication, 
                                             @People, @Career, @TestDate)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", result.UserId);
                    cmd.Parameters.AddWithValue("@Analytical", result.AnalyticalScore);
                    cmd.Parameters.AddWithValue("@Creative", result.CreativeScore);
                    cmd.Parameters.AddWithValue("@Data", result.DataScore);
                    cmd.Parameters.AddWithValue("@Programming", result.ProgrammingScore);
                    cmd.Parameters.AddWithValue("@Communication", result.CommunicationScore);
                    cmd.Parameters.AddWithValue("@People", result.PeopleScore);
                    cmd.Parameters.AddWithValue("@Career", result.RecommendedCareer);
                    cmd.Parameters.AddWithValue("@TestDate", result.TestDate);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving quiz result: " + ex.Message);
                return false;
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Users WHERE Email = @Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString() ?? string.Empty,
                                Email = reader["Email"].ToString() ?? string.Empty,
                                Password = reader["Password"].ToString() ?? string.Empty,
                                EducationLevel = reader["EducationLevel"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Users";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString() ?? string.Empty,
                                Email = reader["Email"].ToString() ?? string.Empty,
                                EducationLevel = reader["EducationLevel"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return users;
        }
    }
}