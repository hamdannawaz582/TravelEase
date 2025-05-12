using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DB_Project.Models;
using DB_Project.Services;

namespace DB_Project.CompanyPages
{
    public class OperatorRepository
    {
        public bool AuthenticateOperator(string username, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            connection.Open();
            
            var command = new SqlCommand(
                "SELECT u.Username FROM [User] u " +
                "JOIN Operator o ON u.Username = o.Username " +
                "WHERE u.Username = @Username AND u.Password = @Password", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }
        
        public bool RegisterOperator(string username, string email, string password, string companyName)
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

                // Insert into Operator table
                var operatorCommand = new SqlCommand(
                    "INSERT INTO Operator (Username) VALUES (@Username)",
                    connection, transaction);

                operatorCommand.Parameters.AddWithValue("@Username", username);
                operatorCommand.ExecuteNonQuery();
                
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
        
        public async Task<OperatorProfile> GetProfile(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "SELECT u.Email, u.JoinDate FROM [User] u " +
                "JOIN Operator o ON u.Username = o.Username " +
                "WHERE u.Username = @Username", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new OperatorProfile
                {
                    Username = username,
                    Email = reader["Email"].ToString(),
                    JoinDate = Convert.ToDateTime(reader["JoinDate"])
                };
            }
            
            return null;
        }
        
        // Method to get trips created by this operator
        public async Task<List<Trip>> GetOperatorTrips(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var command = new SqlCommand(
                "SELECT t.TripID, t.Title, t.Type, t.CancellationPolicy, " +
                "t.GroupSize, t.StartDate, t.EndDate, t.PriceRange, " +
                "d.City + ', ' + d.Country AS Destination " +
                "FROM Trip t " +
                "JOIN Trip_Destination td ON t.TripID = td.TripID " +
                "JOIN Destination d ON td.DestID = d.DestID " +
                "WHERE t.OperatorUsername = @Username " +
                "ORDER BY t.StartDate",
                connection);
            
            command.Parameters.AddWithValue("@Username", username);

            var trips = new List<Trip>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                trips.Add(new Trip
                {   //TODO: GetOperatorTrips()
                    // Title = reader["Title"].ToString(),
                    // GroupSize = Convert.ToInt32(reader["GroupSize"]),
                    // StartDate = Convert.ToDateTime(reader["StartDate"]),
                    // EndDate = Convert.ToDateTime(reader["EndDate"]),
                    
                });
            }
            
            return trips;
        }
        
        // Method to get booking statistics
        public async Task<Dictionary<DateTime, int>> GetBookingStats(string username, string period)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            string groupByClause;
            switch (period.ToLower())
            {
                case "daily":
                    groupByClause = "CAST(t.StartDate AS DATE)";
                    break;
                case "weekly":
                    groupByClause = "DATEADD(week, DATEDIFF(week, 0, t.StartDate), 0)";
                    break;
                case "monthly":
                    groupByClause = "DATEADD(month, DATEDIFF(month, 0, t.StartDate), 0)";
                    break;
                case "yearly":
                    groupByClause = "DATEADD(year, DATEDIFF(year, 0, t.StartDate), 0)";
                    break;
                default:
                    groupByClause = "CAST(t.StartDate AS DATE)";
                    break;
            }
            
            var query = $@"
                SELECT {groupByClause} as PeriodDate, COUNT(*) as BookingCount
                FROM Trip_Booking tb
                JOIN Trip t ON tb.TripID = t.TripID
                WHERE t.OperatorUsername = @Username
                GROUP BY {groupByClause}
                ORDER BY PeriodDate";
                
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            
            var bookingStats = new Dictionary<DateTime, int>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var date = Convert.ToDateTime(reader["PeriodDate"]);
                var count = Convert.ToInt32(reader["BookingCount"]);
                bookingStats[date] = count;
            }
            
            return bookingStats;
        }
        
        public class OperatorProfile
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime JoinDate { get; set; }
            public string CompanyName { get; set; }
        }
    }
}