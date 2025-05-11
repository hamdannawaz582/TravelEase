using Microsoft.Data.SqlClient;
using System;

namespace DB_Project.Services
{
    public class DatabaseService
    {
        private static readonly string ConnectionString = "Server=localhost;Database=TravelEase;User Id=sa;Password=Admin123;Encrypt=false;TrustServerCertificate=true;";
        private static DatabaseService _instance;
        
        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseService();
                }
                return _instance;
            }
        }

        private DatabaseService() { }
        
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
        
        public bool TestConnection()
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                return false;
            }
        }
    }
}