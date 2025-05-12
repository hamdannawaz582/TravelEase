// DB_Project/Repositories/HotelRepository.cs
using DB_Project.Models;
using DB_Project.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Project.Repositories
{
    public class HotelRepository
    {
        public bool AuthenticateHotel(string username, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            connection.Open();
            
            var command = new SqlCommand(
                "SELECT u.Username FROM [User] u " +
                "JOIN Hotel h ON u.Username = h.HUsername " +
                "WHERE u.Username = @Username AND u.Password = @Password", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }
        
        public bool RegisterHotel(string username, string email, string password, string name)
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

                var hotelCommand = new SqlCommand(
                    "INSERT INTO Hotel (HUsername, Name) " +
                    "VALUES (@Username, @Name)",
                    connection, transaction);

                hotelCommand.Parameters.AddWithValue("@Username", username);
                hotelCommand.Parameters.AddWithValue("@Name", name);
                hotelCommand.ExecuteNonQuery();
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Hotel registration error: {ex.Message}");
                return false;
            }
        }

        public async Task<HotelProfile> GetProfile(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "SELECT u.Email, u.JoinDate, h.Name " +
                "FROM [User] u " +
                "JOIN Hotel h ON u.Username = h.HUsername " +
                "WHERE u.Username = @Username", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new HotelProfile
                {
                    Username = username,
                    Email = reader["Email"].ToString(),
                    JoinDate = Convert.ToDateTime(reader["JoinDate"]),
                    Name = reader["Name"].ToString()
                };
            }
            
            return null;
        }
        public class HotelProfile
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime JoinDate { get; set; }
            public string Name { get; set; }
        }
    }
}