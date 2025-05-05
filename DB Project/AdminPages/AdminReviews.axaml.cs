using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Avalonia.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Project.AdminPages
{
    public partial class AdminReviews : UserControl
    {
        // Collection to store review data
        private ObservableCollection<ReviewViewModel> _reviews;
        
        // Currently selected review
        private ReviewViewModel _selectedReview;
        
        // List of inappropriate words to check for
        private readonly List<string> _inappropriateWords = new List<string>
        {
            "awful", "terrible", "horrible", "stupid", "idiot", "hate", "crap", "garbage", 
            "damn", "hell", "shit", "bitch", "bastard", "suck"
        };

        public AdminReviews()
        {
            InitializeComponent();
            
            // Initialize the reviews collection
            _reviews = new ObservableCollection<ReviewViewModel>();
            ReviewsDataGrid.ItemsSource = _reviews;
            
            // Load reviews when control is loaded
            this.AttachedToVisualTree += (s, e) => LoadReviews();
        }

        private async void LoadReviews()
        {
            // Clear existing reviews
            _reviews.Clear();
            
            // In a real implementation, this would load from the database
            // For now, we'll use some sample data
            await Task.Run(() =>
            {
                // Simulate database delay
                System.Threading.Thread.Sleep(500);

                // FIXME: Replace this mock data with actual database queries using the TravelEase database schema
                var sampleReviews = new List<ReviewViewModel>
                {
                    CreateReview(1, "Traveller1", DateTime.Now.AddDays(-5), 4,
                        "Great trip! Really enjoyed the experience.", "Trip", 12),
                    CreateReview(2, "Traveller2", DateTime.Now.AddDays(-3), 2,
                        "Hotel was terrible. The service was awful and the staff were idiots.", "Hotel", 2),
                    CreateReview(3, "Traveller3", DateTime.Now.AddDays(-2), 1,
                        "What a garbage experience. I hate this place and will never return.", "Trip", 3),
                    CreateReview(4, "Traveller4", DateTime.Now.AddDays(-1), 3,
                        "The destination was beautiful but the transportation was not well organized.", "Destination", 7),
                    CreateReview(5, "Traveller5", DateTime.Now.AddHours(-12), 5,
                        "Amazing hotel with wonderful staff!", "Hotel", 9)
                };

                // Add sample reviews to the collection
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var review in sampleReviews)
                    {
                        // Check for inappropriate content
                        review.FlaggedWords = CheckForInappropriateWords(review.Feedback);
                        review.FlagCount = review.FlaggedWords.Count.ToString();
                        review.Status = review.FlaggedWords.Count > 0 ? "Flagged" : "Pending";
                        review.StatusColor = review.FlaggedWords.Count > 0 ? new SolidColorBrush(Color.Parse("#ff3b3b")) : new SolidColorBrush(Color.Parse("#ffa500"));
                        review.FlagColor = review.FlaggedWords.Count > 0 ? new SolidColorBrush(Color.Parse("#ff3b3b")) : new SolidColorBrush(Color.Parse("#333333"));
                        _reviews.Add(review);
                    }
                });
            });
        }

        // Helper method to create review view model objects
        private ReviewViewModel CreateReview(int id, string reviewer, DateTime date, int stars, string feedback, string type, int tripId)
        {
            return new ReviewViewModel
            {
                ReviewID = id,
                Reviewer = reviewer,
                ReviewDate = date,
                FormattedDate = date.ToString("MM/dd/yyyy"),
                Stars = stars,
                Feedback = feedback,
                FeedbackSummary = feedback.Length > 50 ? feedback.Substring(0, 47) + "..." : feedback,
                ReviewType = type,
                RelatedEntityId = tripId
            };
        }

        // Check for inappropriate words in a given text
        private List<string> CheckForInappropriateWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            var foundWords = new List<string>();
            string lowerText = text.ToLower();
            
            foreach (var word in _inappropriateWords)
            {
                if (lowerText.Contains(word))
                {
                    foundWords.Add(word);
                }
            }
            
            return foundWords;
        }

        // Event handler for selection changes in the data grid
        private void ReviewsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedReview = ReviewsDataGrid.SelectedItem as ReviewViewModel;
            
            if (_selectedReview != null)
            {
                // Show details for the selected review
                DetailPanel.IsVisible = true;
                
                // Update the content in the detail panel
                SelectedReviewContent.Text = $"Review ID: {_selectedReview.ReviewID}\n" +
                                          $"Reviewer: {_selectedReview.Reviewer}\n" +
                                          $"Date: {_selectedReview.FormattedDate}\n" +
                                          $"Rating: {_selectedReview.Stars} stars\n" +
                                          $"Type: {_selectedReview.ReviewType}\n\n" +
                                          $"Content: {_selectedReview.Feedback}\n\n" +
                                          $"Response: {_selectedReview.Response ?? "No response yet"}";

                // Show flagged words if any
                if (_selectedReview.FlaggedWords.Count > 0)
                {
                    FlaggedWordsText.Text = $"Flagged words: {string.Join(", ", _selectedReview.FlaggedWords)}";
                    FlaggedWordsText.Foreground = new SolidColorBrush(Color.Parse("#ff3b3b"));
                }
                else
                {
                    FlaggedWordsText.Text = "No inappropriate content detected";
                    FlaggedWordsText.Foreground = new SolidColorBrush(Color.Parse("#66aa66"));
                }

                // If review already has a response, show it in the response box
                ResponseTextBox.Text = _selectedReview.Response ?? "";
            }
            else
            {
                DetailPanel.IsVisible = false;
            }
        }

        // Event handlers for review actions
        private void ApproveReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            // FIXME: Update the database to mark this review as approved
            _selectedReview.Status = "Approved";
            _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#66aa66"));

            // Update the database (in a real implementation)
            // UpdateReviewStatus(_selectedReview.ReviewID, "Approved");

            // Refresh the data grid
            ReviewsDataGrid.ItemsSource = null;
            ReviewsDataGrid.ItemsSource = _reviews;
        }

        private void RejectReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            // FIXME: Update the database to mark this review as rejected
            _selectedReview.Status = "Rejected";
            _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#ff3b3b"));

            // Update the database (in a real implementation)
            // UpdateReviewStatus(_selectedReview.ReviewID, "Rejected");

            // Refresh the data grid
            ReviewsDataGrid.ItemsSource = null;
            ReviewsDataGrid.ItemsSource = _reviews;
        }
        private void SendResponse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            string response = ResponseTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(response))
            {
                // Show error - response cannot be empty
                return;
            }

            // FIXME: Update the database with the response
            _selectedReview.Response = response;
            _selectedReview.ResponseDate = DateTime.Now;
            
            // In a real implementation, update the database:
            // UpdateReviewResponse(_selectedReview.ReviewID, response);
            
            // Update the details panel
            SelectedReviewContent.Text = $"Review ID: {_selectedReview.ReviewID}\n" +
                                      $"Reviewer: {_selectedReview.Reviewer}\n" +
                                      $"Date: {_selectedReview.FormattedDate}\n" +
                                      $"Rating: {_selectedReview.Stars} stars\n" +
                                      $"Type: {_selectedReview.ReviewType}\n\n" +
                                      $"Content: {_selectedReview.Feedback}\n\n" +
                                      $"Response: {_selectedReview.Response}";
        }

        // Filter and refresh handlers
        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = SearchTextBox.Text?.ToLower() ?? "";
            
            IEnumerable<ReviewViewModel> filteredReviews = _reviews;
            
            // Apply status filter
            switch (selectedFilter)
            {
                case "Flagged Reviews":
                    filteredReviews = filteredReviews.Where(r => r.FlaggedWords.Count > 0);
                    break;
                case "Pending Reviews":
                    filteredReviews = filteredReviews.Where(r => r.Status == "Pending");
                    break;
                case "Approved Reviews":
                    filteredReviews = filteredReviews.Where(r => r.Status == "Approved");
                    break;
                case "Rejected Reviews":
                    filteredReviews = filteredReviews.Where(r => r.Status == "Rejected");
                    break;
                // "All Reviews" - no filtering needed
            }
            
            // Apply text search if provided
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredReviews = filteredReviews.Where(r => 
                    r.Feedback.ToLower().Contains(searchText) || 
                    r.Reviewer.ToLower().Contains(searchText));
            }
            
            // Update the data grid
            ReviewsDataGrid.ItemsSource = new ObservableCollection<ReviewViewModel>(filteredReviews);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReviews();
            DetailPanel.IsVisible = false;
        }

        // FIXME: The following methods would connect to the database in a real implementation
        private void UpdateReviewStatus(int reviewId, string status)
        {
            // Using the database schema provided in the class diagram:
            /*
            string connectionString = "your_connection_string_here";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Review SET Status = @Status WHERE ReviewID = @ReviewID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@ReviewID", reviewId);
                    command.ExecuteNonQuery();
                }
            }
            */
        }

        private void UpdateReviewResponse(int reviewId, string response)
        {
            // Using the database schema provided in the class diagram:
            /*
            string connectionString = "your_connection_string_here";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Review SET Response = @Response, ResponseTime = @ResponseTime WHERE ReviewID = @ReviewID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Response", response);
                    command.Parameters.AddWithValue("@ResponseTime", DateTime.Now);
                    command.Parameters.AddWithValue("@ReviewID", reviewId);
                    command.ExecuteNonQuery();
                }
            }
            */
        }
    }

    // View model for reviews to be displayed in the data grid
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }
        public string Reviewer { get; set; }
        public DateTime ReviewDate { get; set; }
        public string FormattedDate { get; set; }
        public int Stars { get; set; }
        public string Feedback { get; set; }
        public string FeedbackSummary { get; set; }
        public string ReviewType { get; set; }
        public string Status { get; set; }
        public IBrush StatusColor { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int RelatedEntityId { get; set; }
        public List<string> FlaggedWords { get; set; } = new List<string>();
        public string FlagCount { get; set; }
        public IBrush FlagColor { get; set; }
    }
}