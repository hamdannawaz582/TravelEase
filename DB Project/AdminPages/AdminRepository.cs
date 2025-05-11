using DB_Project.Models;
using DB_Project.Services;
using Microsoft.Data.SqlClient;
using System;

namespace DB_Project.Repositories
{
    public class AdminRepository
    {
        public bool AuthenticateAdmin(string username, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            connection.Open();
            
            var command = new SqlCommand(
                "SELECT u.Username FROM [User] u " +
                "JOIN Admin a ON u.Username = a.Username " +
                "WHERE u.Username = @Username AND u.Password = @Password", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }
        
        public bool RegisterAdmin(string username, string email, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            connection.Open();
            using var transaction = (SqlTransaction)connection.BeginTransaction();

            try
            {
                var userCommand = new SqlCommand(
                    "INSERT INTO [User] (Username, Email, Password, JoinDate) " +
                    "VALUES (@Username, @Email, @Password, @JoinDate)",
                    connection, transaction);

                userCommand.Parameters.AddWithValue("@Username", username);
                userCommand.Parameters.AddWithValue("@Email", email);
                userCommand.Parameters.AddWithValue("@Password", password);
                userCommand.Parameters.AddWithValue("@JoinDate", DateTime.Now);
                userCommand.ExecuteNonQuery();

                var adminCommand = new SqlCommand(
                    "INSERT INTO Admin (Username) VALUES (@Username)",
                    connection, transaction);

                adminCommand.Parameters.AddWithValue("@Username", username);
                adminCommand.ExecuteNonQuery();
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }
    }
}