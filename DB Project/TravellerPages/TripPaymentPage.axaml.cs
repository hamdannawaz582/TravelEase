using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.TravellerPages;

public partial class TripPaymentPage : UserControl
{
    public string TripID { get; set; }
    public string TravellerUsername { get; set; }
    private int tripPrice;
    private int userBudget;

    public TripPaymentPage(string tripId, string travellerUsername)
    {
        InitializeComponent();
        TripID = tripId;
        TravellerUsername = travellerUsername;
        LoadTripDetails();
        LoadUserBudget();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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
                        this.FindControl<TextBlock>("TitleText").Text = reader["Title"].ToString();
                        this.FindControl<TextBlock>("TypeText").Text = reader["Type"].ToString();
                        this.FindControl<TextBlock>("PolicyText").Text = reader["CancellationPolicy"].ToString();
                        this.FindControl<TextBlock>("GroupSizeText").Text = reader["GroupSize"].ToString();
                        this.FindControl<TextBlock>("StartDateText").Text = Convert.ToDateTime(reader["StartDate"]).ToString("MMM dd, yyyy");
                        this.FindControl<TextBlock>("EndDateText").Text = Convert.ToDateTime(reader["EndDate"]).ToString("MMM dd, yyyy");
                        
                        tripPrice = Convert.ToInt32(reader["PriceRange"]);
                        this.FindControl<TextBlock>("PriceText").Text = tripPrice.ToString();
                        this.FindControl<TextBlock>("TripCostText").Text = $"${tripPrice}";
                        
                        this.FindControl<TextBlock>("OperatorText").Text = reader["OperatorUsername"].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading trip details: {ex.Message}");
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
            errorMessage.Text = "Error loading trip details. Please try again.";
            errorMessage.IsVisible = true;
        }
    }

    private void LoadUserBudget()
    {
        string query = @"
            SELECT Budget
            FROM Traveller
            WHERE Username = @Username";

        try
        {
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", TravellerUsername);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userBudget = Convert.ToInt32(reader["Budget"]);
                        var userBudgetText = this.FindControl<TextBlock>("UserBudgetText");
                        userBudgetText.Text = $"${userBudget}";

                        var balanceInfoText = this.FindControl<TextBlock>("BalanceInfoText");
                        var confirmButton = this.FindControl<Button>("ConfirmButton");

                        if (userBudget >= tripPrice)
                        {
                            balanceInfoText.Text = $"Remaining balance after payment: ${userBudget - tripPrice}";
                            balanceInfoText.Foreground = new SolidColorBrush(Color.Parse("LightGreen"));
                            confirmButton.IsEnabled = true;
                        }
                        else
                        {
                            balanceInfoText.Text = $"Insufficient funds. You need ${tripPrice - userBudget} more.";
                            balanceInfoText.Foreground = new SolidColorBrush(Color.Parse("#FF5252"));
                            confirmButton.IsEnabled = false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user budget: {ex.Message}");
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
            errorMessage.Text = "Error loading budget information. Please try again.";
            errorMessage.IsVisible = true;
        }
    }

    private void OnMakePaymentClick(object? sender, RoutedEventArgs e)
    {
        string bookingQuery = @"
            BEGIN TRANSACTION;

            -- Update traveller's budget
            UPDATE Traveller
            SET Budget = Budget - @TripPrice
            WHERE Username = @Username;

            -- Create booking
            INSERT INTO Trip_Booking (Username, TripID)
            VALUES (@Username, @TripID);

            COMMIT;";

        try
        {
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(bookingQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", TravellerUsername);
                command.Parameters.AddWithValue("@TripID", TripID);
                command.Parameters.AddWithValue("@TripPrice", tripPrice);
                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Trip booking confirmed and payment processed successfully.");
                    if (this.Parent is ContentControl contentControl)
                    {
                        contentControl.Content = new TripSearchPage(TravellerUsername);
                    }
                }
                else
                {
                    var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                    errorMessage.Text = "Failed to process payment. Please try again.";
                    errorMessage.IsVisible = true;
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error during payment processing: {ex.Message}");
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
            errorMessage.Text = "Database error during payment processing. Please try again.";
            errorMessage.IsVisible = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error during payment: {ex.Message}");
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
            errorMessage.Text = "An error occurred while processing your payment.";
            errorMessage.IsVisible = true;
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (this.Parent is ContentControl contentControl)
        {
            contentControl.Content = new TripSearchPage(TravellerUsername);
        }
    }
}