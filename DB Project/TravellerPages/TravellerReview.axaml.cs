using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DB_Project.Models;
using DB_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Project.TravellerPages
{
    public partial class TravellerReview : UserControl
    {
        private string Username;
        private TravellerRepository repo;
        private List<Trip> pastTrips;
        
        public TravellerReview(string username)
        {
            InitializeComponent();
            Username = username;
            repo = new TravellerRepository();
            
            LoadAsync();
        }

        private async void LoadAsync()
        {
            await LoadPastTripsAsync();
            await LoadPastReviewsAsync();
        }

        private async Task LoadPastTripsAsync()
        {
            try
            {
                pastTrips = await repo.GetTripHistory(Username);
                
                // Populate trip dropdown
                var tripComboBox = this.FindControl<ComboBox>("TripComboBox");
                tripComboBox.ItemsSource = pastTrips;
                tripComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("Title");
                
                // Get and populate hotels from past bookings
                var hotelCommand = new Microsoft.Data.SqlClient.SqlCommand(
                    @"SELECT DISTINCT h.HUsername, h.Name
                      FROM Hotel h
                      JOIN Room_Booking rb ON h.HUsername = rb.HUsername
                      WHERE rb.Username = @Username", 
                    DB_Project.Services.DatabaseService.Instance.CreateConnection());
                
                hotelCommand.Parameters.AddWithValue("@Username", Username);
                
                var hotels = new List<Hotel>();
                using (var connection = hotelCommand.Connection)
                {
                    await connection.OpenAsync();
                    using var reader = await hotelCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        hotels.Add(new Hotel 
                        { 
                            HUsername = reader["HUsername"].ToString(),
                            Name = reader["Name"].ToString() 
                        });
                    }
                }
                
                var hotelComboBox = this.FindControl<ComboBox>("HotelComboBox");
                hotelComboBox.ItemsSource = hotels;
                hotelComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("Name");
            }
            catch (Exception ex)
            {
                var errorTextBlock = this.FindControl<TextBlock>("ErrorMessage");
                errorTextBlock.Text = $"Error loading data: {ex.Message}";
                errorTextBlock.IsVisible = true;
            }
        }

        private async Task LoadPastReviewsAsync()
        {
            try
            {
                var reviewsPanel = this.FindControl<StackPanel>("ReviewsPanel");
                reviewsPanel.Children.Clear();
                
                // Get trip reviews
                var tripReviewsCommand = new Microsoft.Data.SqlClient.SqlCommand(
                    @"SELECT r.ReviewID, r.Stars, r.Feedback, r.ReviewTime, r.Response,
                             t.Title as ItemName, 'Trip' as ReviewType
                      FROM Review r
                      JOIN TripReview tr ON r.ReviewID = tr.ReviewID
                      JOIN Trip t ON tr.TripID = t.TripID
                      WHERE r.Reviewer = @Username
                      ORDER BY r.ReviewTime DESC", 
                    DB_Project.Services.DatabaseService.Instance.CreateConnection());
                
                tripReviewsCommand.Parameters.AddWithValue("@Username", Username);
                
                // Get hotel reviews
                var hotelReviewsCommand = new Microsoft.Data.SqlClient.SqlCommand(
                    @"SELECT r.ReviewID, r.Stars, r.Feedback, r.ReviewTime, r.Response,
                             h.Name as ItemName, 'Hotel' as ReviewType
                      FROM Review r
                      JOIN HotelReview hr ON r.ReviewID = hr.ReviewID
                      JOIN Hotel h ON hr.HUsername = h.HUsername
                      WHERE r.Reviewer = @Username
                      ORDER BY r.ReviewTime DESC", 
                    DB_Project.Services.DatabaseService.Instance.CreateConnection());
                
                hotelReviewsCommand.Parameters.AddWithValue("@Username", Username);
                
                var reviews = new List<ReviewDisplay>();
                
                // Process trip reviews
                using (var connection = tripReviewsCommand.Connection)
                {
                    await connection.OpenAsync();
                    using var reader = await tripReviewsCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(CreateReviewDisplayFromReader(reader));
                    }
                }
                
                // Process hotel reviews
                using (var connection = hotelReviewsCommand.Connection)
                {
                    await connection.OpenAsync();
                    using var reader = await hotelReviewsCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(CreateReviewDisplayFromReader(reader));
                    }
                }
                
                // Display reviews
                foreach (var review in reviews)
                {
                    var reviewCard = CreateReviewCard(review);
                    reviewsPanel.Children.Add(reviewCard);
                }
                
                if (reviews.Count == 0)
                {
                    var noReviews = new TextBlock
                    {
                        Text = "You haven't written any reviews yet.",
                        Margin = new Avalonia.Thickness(10),
                        FontSize = 16
                    };
                    reviewsPanel.Children.Add(noReviews);
                }
            }
            catch (Exception ex)
            {
                var errorTextBlock = this.FindControl<TextBlock>("ErrorMessage");
                errorTextBlock.Text = $"Error loading reviews: {ex.Message}";
                errorTextBlock.IsVisible = true;
            }
        }

        private ReviewDisplay CreateReviewDisplayFromReader(Microsoft.Data.SqlClient.SqlDataReader reader)
        {
            return new ReviewDisplay
            {
                ReviewID = Convert.ToInt32(reader["ReviewID"]),
                Stars = Convert.ToInt32(reader["Stars"]),
                Feedback = reader["Feedback"].ToString(),
                ReviewDate = Convert.ToDateTime(reader["ReviewTime"]).ToString("MMM dd, yyyy"),
                ItemName = reader["ItemName"].ToString(),
                ReviewType = reader["ReviewType"].ToString(),
                Response = reader["Response"] is DBNull ? null : reader["Response"].ToString()
            };
        }

        private Border CreateReviewCard(ReviewDisplay review)
        {
            var card = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Avalonia.Thickness(1),
                CornerRadius = new Avalonia.CornerRadius(5),
                Padding = new Avalonia.Thickness(10),
                Margin = new Avalonia.Thickness(0, 0, 0, 10),
                Background = new SolidColorBrush(Color.Parse("#333333"))
            };
            
            var content = new StackPanel();
            
            // Header with item name and type
            var header = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Margin = new Avalonia.Thickness(0, 0, 0, 5)
            };
            
            header.Children.Add(new TextBlock
            {
                Text = $"{review.ReviewType}: {review.ItemName}",
                FontWeight = Avalonia.Media.FontWeight.Bold,
                FontSize = 16
            });
            
            content.Children.Add(header);
            
            // Rating as stars
            var ratingPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Margin = new Avalonia.Thickness(0, 0, 0, 5)
            };
            
            for (int i = 0; i < review.Stars; i++)
            {
                ratingPanel.Children.Add(new TextBlock
                {
                    Text = "★",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Gold)
                });
            }
            
            for (int i = review.Stars; i < 5; i++)
            {
                ratingPanel.Children.Add(new TextBlock
                {
                    Text = "☆",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Gray)
                });
            }
            
            ratingPanel.Children.Add(new TextBlock
            {
                Text = $" {review.ReviewDate}",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10, 0, 0, 0),
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.LightGray)
            });
            
            content.Children.Add(ratingPanel);
            
            // Feedback
            content.Children.Add(new TextBlock
            {
                Text = review.Feedback,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(0, 5, 0, 5)
            });
            
            // Response (if any)
            if (!string.IsNullOrEmpty(review.Response))
            {
                var responsePanel = new StackPanel
                {
                    Margin = new Avalonia.Thickness(10, 5, 0, 0)
                };
                
                responsePanel.Children.Add(new TextBlock
                {
                    Text = "Response:",
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize = 14
                });
                
                responsePanel.Children.Add(new TextBlock
                {
                    Text = review.Response,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Margin = new Avalonia.Thickness(0, 5, 0, 0),
                    FontStyle = Avalonia.Media.FontStyle.Italic
                });
                
                content.Children.Add(responsePanel);
            }
            
            card.Child = content;
            return card;
        }

        private async void SubmitTripReview_Click(object sender, RoutedEventArgs e)
        {
            var tripComboBox = this.FindControl<ComboBox>("TripComboBox");
            var tripRatingSlider = this.FindControl<Slider>("TripRatingSlider");
            var tripFeedbackTextBox = this.FindControl<TextBox>("TripFeedbackTextBox");
            var errorMessageBlock = this.FindControl<TextBlock>("ErrorMessage");
            
            errorMessageBlock.IsVisible = false;
            
            if (tripComboBox.SelectedItem == null)
            {
                errorMessageBlock.Text = "Please select a trip to review.";
                errorMessageBlock.IsVisible = true;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(tripFeedbackTextBox.Text))
            {
                errorMessageBlock.Text = "Please enter feedback for your review.";
                errorMessageBlock.IsVisible = true;
                return;
            }
            
            var selectedTrip = tripComboBox.SelectedItem as Trip;
            int stars = (int)tripRatingSlider.Value;
            
            try
            {
                bool result = await repo.AddTripReview(
                    Username, 
                    selectedTrip.TripID, 
                    stars, 
                    tripFeedbackTextBox.Text
                );
                
                if (result)
                {
                    tripComboBox.SelectedItem = null;
                    tripRatingSlider.Value = 3;
                    tripFeedbackTextBox.Text = "";
                    
                    var successMessage = this.FindControl<TextBlock>("SuccessMessage");
                    successMessage.Text = "Trip review submitted successfully!";
                    successMessage.IsVisible = true;
                    
                    // Reload reviews
                    await LoadPastReviewsAsync();
                }
                else
                {
                    errorMessageBlock.Text = "Failed to submit trip review. Please try again.";
                    errorMessageBlock.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                errorMessageBlock.Text = $"Error submitting review: {ex.Message}";
                errorMessageBlock.IsVisible = true;
            }
        }

        private async void SubmitHotelReview_Click(object sender, RoutedEventArgs e)
        {
            var hotelComboBox = this.FindControl<ComboBox>("HotelComboBox");
            var hotelRatingSlider = this.FindControl<Slider>("HotelRatingSlider");
            var hotelFeedbackTextBox = this.FindControl<TextBox>("HotelFeedbackTextBox");
            var errorMessageBlock = this.FindControl<TextBlock>("ErrorMessage");
            
            errorMessageBlock.IsVisible = false;
            
            if (hotelComboBox.SelectedItem == null)
            {
                errorMessageBlock.Text = "Please select a hotel to review.";
                errorMessageBlock.IsVisible = true;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(hotelFeedbackTextBox.Text))
            {
                errorMessageBlock.Text = "Please enter feedback for your review.";
                errorMessageBlock.IsVisible = true;
                return;
            }
            
            var selectedHotel = hotelComboBox.SelectedItem as Hotel;
            int stars = (int)hotelRatingSlider.Value;
            
            try
            {
                bool result = await repo.AddHotelReview(
                    Username, 
                    selectedHotel.HUsername, 
                    stars, 
                    hotelFeedbackTextBox.Text
                );
                
                if (result)
                {
                    hotelComboBox.SelectedItem = null;
                    hotelRatingSlider.Value = 3;
                    hotelFeedbackTextBox.Text = "";
                    
                    var successMessage = this.FindControl<TextBlock>("SuccessMessage");
                    successMessage.Text = "Hotel review submitted successfully!";
                    successMessage.IsVisible = true;
                    
                    // Reload reviews
                    await LoadPastReviewsAsync();
                }
                else
                {
                    errorMessageBlock.Text = "Failed to submit hotel review. Please try again.";
                    errorMessageBlock.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                errorMessageBlock.Text = $"Error submitting review: {ex.Message}";
                errorMessageBlock.IsVisible = true;
            }
        }

        // Helper class for hotel dropdown
        public class Hotel
        {
            public string HUsername { get; set; }
            public string Name { get; set; }
        }

        // Helper class for review display
        public class ReviewDisplay
        {
            public int ReviewID { get; set; }
            public int Stars { get; set; }
            public string Feedback { get; set; }
            public string ReviewDate { get; set; }
            public string ItemName { get; set; }
            public string ReviewType { get; set; }
            public string Response { get; set; }
        }
    }
}