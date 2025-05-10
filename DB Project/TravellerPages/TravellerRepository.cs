using DB_Project.Models;
using DB_Project.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Project.Repositories
{
    public class TravellerRepository
    {
        public bool AuthenticateTraveller(string username, string password)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            connection.Open();
            
            var command = new SqlCommand(
                "SELECT u.Username FROM [User] u " +
                "JOIN Traveller t ON u.Username = t.Username " +
                "WHERE u.Username = @Username AND u.Password = @Password", 
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }
        
        public bool RegisterTraveller(string username, string email, string password, 
            string firstName, string lastName, string nationality, int age, int budget)
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

                var travellerCommand = new SqlCommand(
                    "INSERT INTO Traveller (Username, FName, LName, Nationality, Age, Budget) " +
                    "VALUES (@Username, @FName, @LName, @Nationality, @Age, @Budget)",
                    connection, transaction);

                travellerCommand.Parameters.AddWithValue("@Username", username);
                travellerCommand.Parameters.AddWithValue("@FName", firstName);
                travellerCommand.Parameters.AddWithValue("@LName", lastName);
                travellerCommand.Parameters.AddWithValue("@Nationality", nationality);
                travellerCommand.Parameters.AddWithValue("@Age", age);
                travellerCommand.Parameters.AddWithValue("@Budget", budget);
                travellerCommand.ExecuteNonQuery();
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

        public async Task<bool> UpdateProfile(TravellerProfile profile)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var userCommand = new SqlCommand(
                    "UPDATE [User] SET Email = @Email WHERE Username = @Username",
                    connection, transaction);
                
                userCommand.Parameters.AddWithValue("@Username", profile.Username);
                userCommand.Parameters.AddWithValue("@Email", profile.Email);
                await userCommand.ExecuteNonQueryAsync();

                var travellerCommand = new SqlCommand(
                    "UPDATE Traveller SET FName = @FName, LName = @LName, " +
                    "Nationality = @Nationality, Age = @Age, Budget = @Budget " +
                    "WHERE Username = @Username",
                    connection, transaction);

                travellerCommand.Parameters.AddWithValue("@Username", profile.Username);
                travellerCommand.Parameters.AddWithValue("@FName", profile.FirstName);
                travellerCommand.Parameters.AddWithValue("@LName", profile.LastName);
                travellerCommand.Parameters.AddWithValue("@Nationality", profile.Nationality);
                travellerCommand.Parameters.AddWithValue("@Age", profile.Age);
                travellerCommand.Parameters.AddWithValue("@Budget", profile.Budget);
                await travellerCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<Trip>> SearchTrips(string destination, DateTime? startDate, 
            DateTime? endDate, string tripType, int maxPrice)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT t.TripID, t.Title, t.Type, t.CancelStatus, t.CancellationPolicy, 
                       t.GroupSize, t.StartDate, t.EndDate, t.PriceRange, t.OperatorUsername,
                       d.City + ', ' + d.Country AS Destination
                FROM Trip t
                JOIN Trip_Destination td ON t.TripID = td.TripID
                JOIN Destination d ON td.DestID = d.DestID
                WHERE 1=1";

            if (!string.IsNullOrEmpty(destination))
                query += " AND (d.City LIKE @Destination OR d.Country LIKE @Destination)";

            if (startDate.HasValue)
                query += " AND t.StartDate >= @StartDate";

            if (endDate.HasValue)
                query += " AND t.EndDate <= @EndDate";

            if (!string.IsNullOrEmpty(tripType))
                query += " AND t.Type = @TripType";

            if (maxPrice > 0)
                query += " AND t.PriceRange <= @MaxPrice";

            var command = new SqlCommand(query, connection);

            if (!string.IsNullOrEmpty(destination))
                command.Parameters.AddWithValue("@Destination", $"%{destination}%");

            if (startDate.HasValue)
                command.Parameters.AddWithValue("@StartDate", startDate.Value);

            if (endDate.HasValue)
                command.Parameters.AddWithValue("@EndDate", endDate.Value);

            if (!string.IsNullOrEmpty(tripType))
                command.Parameters.AddWithValue("@TripType", tripType);

            if (maxPrice > 0)
                command.Parameters.AddWithValue("@MaxPrice", maxPrice);

            var trips = new List<Trip>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                trips.Add(new Trip
                {
                    TripID = Convert.ToInt32(reader["TripID"]),
                    Title = reader["Title"].ToString(),
                    Type = reader["Type"].ToString(),
                    CancellationPolicy = reader["CancellationPolicy"].ToString(),
                    GroupSize = Convert.ToInt32(reader["GroupSize"]),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    PriceRange = Convert.ToInt32(reader["PriceRange"]),
                    OperatorUsername = reader["OperatorUsername"].ToString(),
                    Destination = reader["Destination"].ToString()
                });
            }
            
            return trips;
        }

        public async Task<List<Trip>> GetUpcomingTrips(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT t.TripID, t.Title, t.Type, t.CancellationPolicy, 
                       t.StartDate, t.EndDate, t.PriceRange,
                       d.City + ', ' + d.Country AS Destination
                FROM Trip t
                JOIN Trip_Booking tb ON t.TripID = tb.TripID
                JOIN Trip_Destination td ON t.TripID = td.TripID
                JOIN Destination d ON td.DestID = d.DestID
                WHERE tb.Username = @Username AND t.StartDate > GETDATE()",
                connection);
            
            command.Parameters.AddWithValue("@Username", username);

            var trips = new List<Trip>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                trips.Add(new Trip
                {
                    TripID = Convert.ToInt32(reader["TripID"]),
                    Title = reader["Title"].ToString(),
                    Type = reader["Type"].ToString(),
                    CancellationPolicy = reader["CancellationPolicy"].ToString(),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    PriceRange = Convert.ToInt32(reader["PriceRange"]),
                    Destination = reader["Destination"].ToString()
                });
            }
            
            return trips;
        }
        public async Task<List<TravelPass>> GetTravelPasses(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var command = new SqlCommand(@"
        SELECT t.TripID, t.Title, t.StartDate, t.EndDate, 
               d.City + ', ' + d.Country AS Destination,
               h.Name AS HotelName
        FROM Trip t
        JOIN Trip_Booking tb ON t.TripID = tb.TripID
        JOIN Trip_Destination td ON t.TripID = td.TripID
        JOIN Destination d ON td.DestID = d.DestID
        LEFT JOIN Trip_Hotels th ON t.TripID = th.TripID
        LEFT JOIN Hotel h ON th.HUsername = h.HUsername
        WHERE tb.Username = @Username AND t.StartDate > GETDATE()",
                connection);
    
            command.Parameters.AddWithValue("@Username", username);

            var passes = new List<TravelPass>();
            using var reader = await command.ExecuteReaderAsync();
    
            while (await reader.ReadAsync())
            {
                string passCode = $"PASS-{Convert.ToInt32(reader["TripID"])}-{username.GetHashCode():X8}";
        
                passes.Add(new TravelPass
                {
                    TripName = reader["Title"].ToString(),
                    ValidFrom = Convert.ToDateTime(reader["StartDate"]).ToString("MMM dd, yyyy"),
                    ValidTo = Convert.ToDateTime(reader["EndDate"]).ToString("MMM dd, yyyy"),
                    PassCode = passCode,
                    HotelVoucher = !reader.IsDBNull(reader.GetOrdinal("HotelName")) 
                        ? $"{reader["HotelName"]} - Check-in on {Convert.ToDateTime(reader["StartDate"]):MMM dd}" 
                        : "No hotel booking",
                    ActivityPass = $"{reader["Destination"]} Explorer Pass"
                });
            }
    
            return passes;
        }
        public async Task<List<Trip>> GetTripHistory(string username)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT t.TripID, t.Title, t.Type, t.CancellationPolicy, 
                       t.StartDate, t.EndDate, t.PriceRange,
                       d.City + ', ' + d.Country AS Destination
                FROM Trip t
                JOIN Trip_Booking tb ON t.TripID = tb.TripID
                JOIN Trip_Destination td ON t.TripID = td.TripID
                JOIN Destination d ON td.DestID = d.DestID
                WHERE tb.Username = @Username AND t.EndDate < GETDATE()",
                connection);
            
            command.Parameters.AddWithValue("@Username", username);

            var trips = new List<Trip>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                trips.Add(new Trip
                {
                    TripID = Convert.ToInt32(reader["TripID"]),
                    Title = reader["Title"].ToString(),
                    Type = reader["Type"].ToString(),
                    CancellationPolicy = reader["CancellationPolicy"].ToString(),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    PriceRange = Convert.ToInt32(reader["PriceRange"]),
                    Destination = reader["Destination"].ToString()
                });
            }
            
            return trips;
        }

        // Get trip itineraries
        public async Task<List<ItineraryItem>> GetTripItineraries(int tripId)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
        
            var command = new SqlCommand(
                "SELECT Event, EventStartDate, EventEndDate FROM Trip_Itinerary " +
                "WHERE TripID = @TripID ORDER BY EventStartDate",
                connection);
            
            command.Parameters.AddWithValue("@TripID", tripId);
        
            var itineraries = new List<ItineraryItem>();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                itineraries.Add(new ItineraryItem
                {
                    Event = reader["Event"].ToString(),
                    EventStartDate = reader["EventStartDate"].ToString(),
                    EventEndDate = reader["EventEndDate"].ToString()
                });
            }
            
            return itineraries;
        }

        public async Task<bool> AddTripReview(string username, int tripId, int stars, string feedback)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var reviewCommand = new SqlCommand(
                    "INSERT INTO Review (Stars, Feedback, ReviewTime, Reviewer) " +
                    "VALUES (@Stars, @Feedback, @ReviewTime, @Reviewer); " +
                    "SELECT SCOPE_IDENTITY()",
                    connection, transaction);

                reviewCommand.Parameters.AddWithValue("@Stars", stars);
                reviewCommand.Parameters.AddWithValue("@Feedback", feedback);
                reviewCommand.Parameters.AddWithValue("@ReviewTime", DateTime.Now);
                reviewCommand.Parameters.AddWithValue("@Reviewer", username);
                
                int reviewId = Convert.ToInt32(await reviewCommand.ExecuteScalarAsync());

                var tripReviewCommand = new SqlCommand(
                    "INSERT INTO TripReview (ReviewID, TripID) VALUES (@ReviewID, @TripID)",
                    connection, transaction);
                
                tripReviewCommand.Parameters.AddWithValue("@ReviewID", reviewId);
                tripReviewCommand.Parameters.AddWithValue("@TripID", tripId);
                await tripReviewCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> AddHotelReview(string username, string hotelUsername, int stars, string feedback)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var reviewCommand = new SqlCommand(
                    "INSERT INTO Review (Stars, Feedback, ReviewTime, Reviewer) " +
                    "VALUES (@Stars, @Feedback, @ReviewTime, @Reviewer); " +
                    "SELECT SCOPE_IDENTITY()",
                    connection, transaction);

                reviewCommand.Parameters.AddWithValue("@Stars", stars);
                reviewCommand.Parameters.AddWithValue("@Feedback", feedback);
                reviewCommand.Parameters.AddWithValue("@ReviewTime", DateTime.Now);
                reviewCommand.Parameters.AddWithValue("@Reviewer", username);
                
                int reviewId = Convert.ToInt32(await reviewCommand.ExecuteScalarAsync());

                var hotelReviewCommand = new SqlCommand(
                    "INSERT INTO HotelReview (ReviewID, HUsername) VALUES (@ReviewID, @HUsername)",
                    connection, transaction);
                
                hotelReviewCommand.Parameters.AddWithValue("@ReviewID", reviewId);
                hotelReviewCommand.Parameters.AddWithValue("@HUsername", hotelUsername);
                await hotelReviewCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> BookTrip(string username, int tripId)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "INSERT INTO Trip_Booking (Username, TripID) VALUES (@Username, @TripID)",
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@TripID", tripId);
            
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> AddToWishlist(string username, int tripId)
        {
            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();
            
            var command = new SqlCommand(
                "INSERT INTO Traveller_Wishlist (Username, TripID) VALUES (@Username, @TripID)",
                connection);
                
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@TripID", tripId);
            
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Get available hotels
        // public async Task<List<HotelReviewItem>> GetAvailableHotels()
        // {
        //     using var connection = DatabaseService.Instance.CreateConnection();
        //     await connection.OpenAsync();
        //     
        //     var command = new SqlCommand("SELECT HUsername, Name FROM Hotel", connection);
        //     
        //     var hotels = new List<HotelReviewItem>();
        //     using var reader = await command.ExecuteReaderAsync();
        //     
        //     while (await reader.ReadAsync())
        //     {
        //         hotels.Add(new HotelReviewItem
        //         {
        //             HUsername = reader["HUsername"].ToString(),
        //             Name = reader["Name"].ToString()
        //         });
        //     }
        //     
        //     return hotels;
        // }
    }
}