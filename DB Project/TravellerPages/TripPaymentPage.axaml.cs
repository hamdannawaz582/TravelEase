using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.TravellerPages;

public partial class TripPaymentPage : UserControl
{
    public string TripID { get; set; }
    public string TravellerUsername { get; set; }

    public TripPaymentPage(string tripId, string travellerUsername)
    {
        InitializeComponent();
        TripID = tripId;
        TravellerUsername = travellerUsername;
        LoadTripDetails();
    }
    
    private void LoadTripDetails()
    {
        string query = @"
            SELECT Title, Type, CancellationPolicy, GroupSize, StartDate, EndDate, PriceRange, OperatorUsername
            FROM Trip
            WHERE TripID = @TripID";

        try
        {
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TripID", TripID);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        TitleText.Text = reader["Title"].ToString();
                        TypeText.Text = reader["Type"].ToString();
                        PolicyText.Text = reader["CancellationPolicy"].ToString();
                        GroupSizeText.Text = reader["GroupSize"].ToString();
                        StartDateText.Text = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd");
                        EndDateText.Text = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd");
                        PriceText.Text = reader["PriceRange"].ToString();
                        OperatorText.Text = reader["OperatorUsername"].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading trip details: {ex.Message}");
        }
    }
    
    private void OnMakePaymentClick(object? sender, RoutedEventArgs e)
    {
        string insertQuery = @"
            INSERT INTO Trip_Booking (Username, TripID)
            VALUES (@Username, @TripID)";

    try
    {
        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@Username", TravellerUsername);
            command.Parameters.AddWithValue("@TripID", TripID);
            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Trip booking confirmed successfully.");
                // You can show a success message to the user here
            }
            else
            {
                Console.WriteLine("Failed to confirm booking.");
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Error during booking: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General error during booking: {ex.Message}");
    }

    }

}