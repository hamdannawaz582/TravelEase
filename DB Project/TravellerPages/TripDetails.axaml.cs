using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.TravellerPages
{
    public partial class TripDetailsPage : UserControl
    {
        private string _travellerUsername;
        private int _tripId;
        private string _cancellationPolicy;
        private ContentControl _parentControl;

        public TripDetailsPage(ContentControl parentControl, int tripId, string travellerUsername)
        {
            InitializeComponent();
            _parentControl = parentControl;
            _tripId = tripId;
            _travellerUsername = travellerUsername;
            LoadTripDetails();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadTripDetails()
        {
            string query = @"
                SELECT t.Title, t.Type, t.CancellationPolicy, t.GroupSize, t.StartDate, t.EndDate, 
                       t.PriceRange, t.OperatorUsername
                FROM Trip t
                WHERE t.TripID = @TripID";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TripID", _tripId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.FindControl<TextBlock>("TitleText").Text = reader["Title"].ToString();
                            this.FindControl<TextBlock>("TypeText").Text = reader["Type"].ToString();
                            _cancellationPolicy = reader["CancellationPolicy"].ToString();
                            this.FindControl<TextBlock>("PolicyText").Text = _cancellationPolicy;
                            this.FindControl<TextBlock>("GroupSizeText").Text = reader["GroupSize"].ToString();
                            this.FindControl<TextBlock>("StartDateText").Text = Convert.ToDateTime(reader["StartDate"]).ToString("MMM dd, yyyy");
                            this.FindControl<TextBlock>("EndDateText").Text = Convert.ToDateTime(reader["EndDate"]).ToString("MMM dd, yyyy");
                            this.FindControl<TextBlock>("PriceText").Text = reader["PriceRange"].ToString();
                            this.FindControl<TextBlock>("OperatorText").Text = reader["OperatorUsername"].ToString();
                            
                            var refundButton = this.FindControl<Button>("RequestRefundButton");
                            var policyNotice = this.FindControl<TextBlock>("PolicyNotice");
                            
                            if (_cancellationPolicy == "Refundable")
                            {
                                refundButton.IsVisible = true;
                                policyNotice.Text = "This trip is refundable. You can request a refund if needed.";
                                policyNotice.Foreground = new SolidColorBrush(Color.Parse("LightGreen"));
                            }
                            else
                            {
                                refundButton.IsVisible = false;
                                policyNotice.Text = "This trip is non-refundable. Cancellations are not eligible for refunds.";
                                policyNotice.Foreground = new SolidColorBrush(Color.Parse("#FF5252"));
                            }
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

        private void OnBackClick(object? sender, RoutedEventArgs e)
        {
            _parentControl.Content = new TravellerDashboard(_travellerUsername);
        }

        private void OnRequestRefundClick(object? sender, RoutedEventArgs e)
        {
            if (_cancellationPolicy != "Refundable")
            {
                var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                errorMessage.Text = "This trip is not eligible for refunds.";
                errorMessage.IsVisible = true;
                return;
            }

            try
            {
                int tripPrice = 0;
                using (var connection = DatabaseService.Instance.CreateConnection())
                {
                    using (var command = new SqlCommand("SELECT PriceRange FROM Trip WHERE TripID = @TripID", connection))
                    {
                        command.Parameters.AddWithValue("@TripID", _tripId);
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            tripPrice = Convert.ToInt32(result);
                        }
                    }
                }
                string refundQuery = @"
                BEGIN TRANSACTION;
                
                -- Delete booking record
                DELETE FROM Trip_Booking 
                WHERE Username = @Username AND TripID = @TripID;
                
                -- Update traveller budget (refund money)
                UPDATE Traveller 
                SET Budget = Budget + @RefundAmount 
                WHERE Username = @Username;
                
                -- Update trip cancel status
                UPDATE Trip 
                SET CancelStatus = 1 
                WHERE TripID = @TripID;
                
                COMMIT;";

                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(refundQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", _travellerUsername);
                    command.Parameters.AddWithValue("@TripID", _tripId);
                    command.Parameters.AddWithValue("@RefundAmount", tripPrice);
                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        var successMessage = this.FindControl<TextBlock>("SuccessMessage");
                        successMessage.Text = $"Refund processed successfully. ${tripPrice} has been refunded to your account.";
                        successMessage.IsVisible = true;
 
                        var refundButton = this.FindControl<Button>("RequestRefundButton");
                        refundButton.IsVisible = false;
                        var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                        errorMessage.IsVisible = false;
                    }
                    else
                    {
                        var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                        errorMessage.Text = "Failed to process refund. Please try again later.";
                        errorMessage.IsVisible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing refund: {ex.Message}");
                var errorMessage = this.FindControl<TextBlock>("ErrorMessage");
                errorMessage.Text = $"Error processing refund: {ex.Message}";
                errorMessage.IsVisible = true;
            }
        }
    }
}