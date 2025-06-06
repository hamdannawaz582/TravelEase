using DB_Project.AdminPages;
using DB_Project.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using Avalonia.Media;

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

        public async Task<List<ReviewViewModel>> GetAllReviews()
        {
            var reviews = new List<ReviewViewModel>();

            using var connection = DatabaseService.Instance.CreateConnection();
            await connection.OpenAsync();

            var command = new SqlCommand(@"
        SELECT 
            r.ReviewID, 
            r.Stars, 
            r.Feedback, 
            r.ReviewTime, 
            r.Response, 
            r.ResponseTime, 
            r.Reviewer
        FROM Review r
        ORDER BY r.ReviewTime DESC", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var feedbackText = reader["Feedback"] != DBNull.Value ? reader["Feedback"].ToString() : "";

                var review = new ReviewViewModel
                {
                    ReviewID = Convert.ToInt32(reader["ReviewID"]),
                    Reviewer = reader["Reviewer"].ToString(),
                    ReviewDate = Convert.ToDateTime(reader["ReviewTime"]),
                    FormattedDate = Convert.ToDateTime(reader["ReviewTime"]).ToString("MM/dd/yyyy"),
                    Stars = Convert.ToInt32(reader["Stars"]),
                    Feedback = feedbackText,
                    FeedbackSummary = feedbackText.Length > 50 
                        ? feedbackText.Substring(0, 47) + "..." 
                        : feedbackText,
                    Response = reader["Response"] == DBNull.Value ? null : reader["Response"].ToString(),
                    ResponseDate = reader["ResponseTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["ResponseTime"]),
                    FlaggedWords = new List<string>(),
                    FlagCount = "0",
                    FlagColor = Brushes.Transparent
                };

                reviews.Add(review);
            }

            return reviews;
        }public async Task<bool> UpdateReviewStatus(int reviewId, string status)
{
    using var connection = DatabaseService.Instance.CreateConnection();
    await connection.OpenAsync();
    
    var command = new SqlCommand(
        "UPDATE Review SET Status = @Status WHERE ReviewID = @ReviewID", 
        connection);
        
    command.Parameters.AddWithValue("@Status", status);
    command.Parameters.AddWithValue("@ReviewID", reviewId);
    
    int rowsAffected = await command.ExecuteNonQueryAsync();
    return rowsAffected > 0;
}

public async Task<bool> UpdateReviewResponse(int reviewId, string response)
{
    using var connection = DatabaseService.Instance.CreateConnection();
    await connection.OpenAsync();
    
    var command = new SqlCommand(
        "UPDATE Review SET Response = @Response, ResponseTime = @ResponseTime WHERE ReviewID = @ReviewID", 
        connection);
        
    command.Parameters.AddWithValue("@Response", response);
    command.Parameters.AddWithValue("@ResponseTime", DateTime.Now);
    command.Parameters.AddWithValue("@ReviewID", reviewId);
    
    int rowsAffected = await command.ExecuteNonQueryAsync();
    return rowsAffected > 0;
}

public async Task<bool> DeleteReview(int reviewId)
{
    using var connection = DatabaseService.Instance.CreateConnection();
    await connection.OpenAsync();
    using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
    
    try
    {
        var tripReviewCommand = new SqlCommand("DELETE FROM TripReview WHERE ReviewID = @ReviewID", 
            connection, transaction);
        tripReviewCommand.Parameters.AddWithValue("@ReviewID", reviewId);
        await tripReviewCommand.ExecuteNonQueryAsync();
        
        var hotelReviewCommand = new SqlCommand("DELETE FROM HotelReview WHERE ReviewID = @ReviewID", 
            connection, transaction);
        hotelReviewCommand.Parameters.AddWithValue("@ReviewID", reviewId);
        await hotelReviewCommand.ExecuteNonQueryAsync();
        
        var reviewCommand = new SqlCommand("DELETE FROM Review WHERE ReviewID = @ReviewID", 
            connection, transaction);
        reviewCommand.Parameters.AddWithValue("@ReviewID", reviewId);
        await reviewCommand.ExecuteNonQueryAsync();
        
        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        return false;
    }
}
    }
}