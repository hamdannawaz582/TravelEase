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
        private ObservableCollection<ReviewViewModel> _reviews;
        private ReviewViewModel _selectedReview;
        
        //list of inappropriate words to check for
        private readonly List<string> _inappropriateWords = new List<string>
        {
            "awful", "terrible", "horrible", "stupid", "idiot", "hate", "crap", "garbage", 
            "damn", "hell", "shit","suck"
        };

        public AdminReviews()
        {
            InitializeComponent();
            _reviews = new ObservableCollection<ReviewViewModel>();
            ReviewsDataGrid.ItemsSource = _reviews;
            this.AttachedToVisualTree += (s, e) => LoadReviews();
        }

        private async void LoadReviews()
        {
            _reviews.Clear();
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(500);

                //TODO:replace with database connection
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
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var review in sampleReviews)
                    {
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
        
        private void ReviewsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedReview = ReviewsDataGrid.SelectedItem as ReviewViewModel;
            
            if (_selectedReview != null)
            {
                DetailPanel.IsVisible = true;
                SelectedReviewContent.Text = $"Review ID: {_selectedReview.ReviewID}\n" +
                                          $"Reviewer: {_selectedReview.Reviewer}\n" +
                                          $"Date: {_selectedReview.FormattedDate}\n" +
                                          $"Rating: {_selectedReview.Stars} stars\n" +
                                          $"Type: {_selectedReview.ReviewType}\n\n" +
                                          $"Content: {_selectedReview.Feedback}\n\n" +
                                          $"Response: {_selectedReview.Response ?? "No response yet"}";
                
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
                ResponseTextBox.Text = _selectedReview.Response ?? "";
            }
            else
            {
                DetailPanel.IsVisible = false;
            }
        }
        
        private void ApproveReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            //TODO: will update the database to mark this review as approved
            _selectedReview.Status = "Approved";
            _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#66aa66"));
            ReviewsDataGrid.ItemsSource = null;
            ReviewsDataGrid.ItemsSource = _reviews;
        }

        private void RejectReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            //TODO: will update the database to mark this review as rejected
            _selectedReview.Status = "Rejected";
            _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#ff3b3b"));
            
            ReviewsDataGrid.ItemsSource = null;
            ReviewsDataGrid.ItemsSource = _reviews;
        }
        private void SendResponse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            string response = ResponseTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(response))
            {
                return;
            }
            
            _selectedReview.Response = response;
            _selectedReview.ResponseDate = DateTime.Now;
            
            SelectedReviewContent.Text = $"Review ID: {_selectedReview.ReviewID}\n" +
                                      $"Reviewer: {_selectedReview.Reviewer}\n" +
                                      $"Date: {_selectedReview.FormattedDate}\n" +
                                      $"Rating: {_selectedReview.Stars} stars\n" +
                                      $"Type: {_selectedReview.ReviewType}\n\n" +
                                      $"Content: {_selectedReview.Feedback}\n\n" +
                                      $"Response: {_selectedReview.Response}";
        }
        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = SearchTextBox.Text?.ToLower() ?? "";
            
            IEnumerable<ReviewViewModel> filteredReviews = _reviews;
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
            }
            
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredReviews = filteredReviews.Where(r => 
                    r.Feedback.ToLower().Contains(searchText) || 
                    r.Reviewer.ToLower().Contains(searchText));
            }
            
            ReviewsDataGrid.ItemsSource = new ObservableCollection<ReviewViewModel>(filteredReviews);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReviews();
            DetailPanel.IsVisible = false;
        }

        // TODO: methods for database operations
        private void UpdateReviewStatus(int reviewId, string status)
        {
            /*
            string connectionString = "";
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
            /*
            string connectionString = "";
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