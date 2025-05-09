using DB_Project.Models;
using DB_Project.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace DB_Project.Repositories
{
    public class TravellerRepository
    {
        public async Task<bool> AuthenticateUser(string username, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "SELECT * FROM [User] WHERE Username = @Username AND Password = @Password", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            using var reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }
        
        public async Task<TravellerProfile> GetProfile(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "SELECT u.Email, u.JoinDate, t.FName, t.LName, t.Nationality, t.Age, t.Budget " +
                "FROM [User] u " +
                "JOIN Traveller t ON u.Username = t.Username " +
                "WHERE u.Username = @Username", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TravellerProfile
                {
                    Username = username,
                    Email = reader["Email"].ToString(),
                    JoinDate = Convert.ToDateTime(reader["JoinDate"]),
                    FirstName = reader["FName"].ToString(),
                    LastName = reader["LName"].ToString(),
                    Nationality = reader["Nationality"].ToString(),
                    Age = Convert.ToInt32(reader["Age"]),
                    Budget = Convert.ToInt32(reader["Budget"])
                };
            }
            
            return null;
        }
        
    }
}