using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DB_Project.Repositories;

namespace DB_Project.AdminPages
{
    public partial class AdminReviews : UserControl
    {
        private ObservableCollection<ReviewViewModel> _reviews;
        private ReviewViewModel _selectedReview;
        private AdminRepository _repository;
        private List<string> _trips = new List<string>();
        private List<string> _hotels = new List<string>();
        
        private readonly List<string> _inappropriateWords = new List<string>
        {
            "awful", "terrible", "horrible", "stupid", "idiot", "hate", "crap", "garbage", 
            "damn", "hell", "shit", "suck"
            // Add more inappropriate words as needed
        };

        public AdminReviews()
        {
            InitializeComponent();
            _reviews = new ObservableCollection<ReviewViewModel>();
            _repository = new AdminRepository();
            ReviewsDataGrid.ItemsSource = _reviews;
    
            var entityTypeComboBox = this.FindControl<ComboBox>("EntityTypeComboBox");
            if (entityTypeComboBox != null)
                entityTypeComboBox.SelectionChanged += EntityTypeComboBox_SelectionChanged;
        
            this.AttachedToVisualTree += (s, e) => LoadReviewsAsync();
        }

        private async void LoadReviewsAsync()
        {
            _reviews.Clear();
            _trips.Clear();
            _hotels.Clear();

            try
            {
                var reviews = await _repository.GetAllReviews();

                foreach (var review in reviews)
                {
                    // Process review flags and colors (existing code)
            
                    _reviews.Add(review);
            
                    // Collect unique trips and hotels
                    if (!string.IsNullOrEmpty(review.TripName) && !_trips.Contains(review.TripName))
                        _trips.Add(review.TripName);
                
                    if (!string.IsNullOrEmpty(review.HotelName) && !_hotels.Contains(review.HotelName))
                        _hotels.Add(review.HotelName);
                }
        
                // Update EntityComboBox based on current selection
                UpdateEntityComboBox();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reviews: {ex.Message}");
            }
        }

        private void UpdateEntityComboBox()
        {
            var entityTypeComboBox = this.FindControl<ComboBox>("EntityTypeComboBox");
            var entityComboBox = this.FindControl<ComboBox>("EntityComboBox");
    
            if (entityTypeComboBox == null || entityComboBox == null)
                return;
        
            entityComboBox.ItemsSource = null;
    
            var selectedType = entityTypeComboBox.SelectedItem as ComboBoxItem;
            if (selectedType == null)
                return;
        
            string typeText = selectedType.Content.ToString();
    
            if (typeText == "Trip Reviews")
            {
                entityComboBox.IsEnabled = true;
                entityComboBox.ItemsSource = _trips.OrderBy(t => t).ToList();
            }
            else if (typeText == "Hotel Reviews")
            {
                entityComboBox.IsEnabled = true;
                entityComboBox.ItemsSource = _hotels.OrderBy(h => h).ToList();
            }
            else
            {
                entityComboBox.IsEnabled = false;
                entityComboBox.ItemsSource = null;
            }
        }

// Add event handler for EntityTypeComboBox
        private void EntityTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEntityComboBox();
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
        
        private async void ApproveReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            bool success = await _repository.UpdateReviewStatus(_selectedReview.ReviewID, "Approved");
            if (success)
            {
                _selectedReview.Status = "Approved";
                _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#66aa66"));
                ReviewsDataGrid.ItemsSource = null;
                ReviewsDataGrid.ItemsSource = _reviews;
            }
        }

        private async void RejectReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            // If the review has inappropriate content, we might want to delete it
            if (_selectedReview.FlaggedWords.Count > 0)
            {
                // Show a confirmation dialog (using a simple ContentDialog)
                var dialog = new Window
                {
                    Title = "Confirm Rejection",
                    Width = 400,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new StackPanel
                    {
                        Margin = new Thickness(20),
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "This review contains inappropriate content. Do you want to delete it?",
                                TextWrapping = TextWrapping.Wrap,
                                Margin = new Thickness(0, 0, 0, 20)
                            },
                            new StackPanel
                            {
                                Orientation = Avalonia.Layout.Orientation.Horizontal,
                                Spacing = 10,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                Children =
                                {
                                    new Button
                                    {
                                        Content = "Just Reject",
                                        Tag = "Reject"
                                    },
                                    new Button
                                    {
                                        Content = "Delete Review",
                                        Tag = "Delete",
                                        Background = new SolidColorBrush(Color.Parse("#ff3b3b"))
                                    },
                                    new Button
                                    {
                                        Content = "Cancel",
                                        Tag = "Cancel"
                                    }
                                }
                            }
                        }
                    }
                };
                
                string result = null;
                foreach (var child in ((StackPanel)((StackPanel)dialog.Content).Children[1]).Children)
                {
                    if (child is Button button)
                    {
                        button.Click += (s, args) =>
                        {
                            result = button.Tag.ToString();
                            dialog.Close();
                        };
                    }
                }
                
                await dialog.ShowDialog(this.VisualRoot as Window);
                
                if (result == "Delete")
                {
                    bool deleteSuccess = await _repository.DeleteReview(_selectedReview.ReviewID);
                    if (deleteSuccess)
                    {
                        _reviews.Remove(_selectedReview);
                        DetailPanel.IsVisible = false;
                        return;
                    }
                }
                else if (result != "Reject")
                {
                    // Canceled
                    return;
                }
            }
            
            // Standard rejection (mark as rejected)
            bool success = await _repository.UpdateReviewStatus(_selectedReview.ReviewID, "Rejected");
            if (success)
            {
                _selectedReview.Status = "Rejected";
                _selectedReview.StatusColor = new SolidColorBrush(Color.Parse("#ff3b3b"));
                
                ReviewsDataGrid.ItemsSource = null;
                ReviewsDataGrid.ItemsSource = _reviews;
            }
        }
        
        private async void SendResponse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            string response = ResponseTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(response))
            {
                return;
            }
            
            bool success = await _repository.UpdateReviewResponse(_selectedReview.ReviewID, response);
            if (success)
            {
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
        }
        
private void ApplyFilter_Click(object sender, RoutedEventArgs e)
{
    var statusFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
    var entityTypeFilter = (EntityTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
    var entityFilter = EntityComboBox.SelectedItem?.ToString();
    string searchText = SearchTextBox.Text?.ToLower() ?? "";

    IEnumerable<ReviewViewModel> filteredReviews = _reviews;
    
    // Apply status filter
    switch (statusFilter)
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
    
    // Apply entity type filter
    switch (entityTypeFilter)
    {
        case "Trip Reviews":
            filteredReviews = filteredReviews.Where(r => r.ReviewType == "Trip");
            break;
        case "Hotel Reviews":
            filteredReviews = filteredReviews.Where(r => r.ReviewType == "Hotel");
            break;
    }
    
    // Apply entity name filter
    if (!string.IsNullOrEmpty(entityFilter))
    {
        if (entityTypeFilter == "Trip Reviews")
            filteredReviews = filteredReviews.Where(r => r.TripName == entityFilter);
        else if (entityTypeFilter == "Hotel Reviews")
            filteredReviews = filteredReviews.Where(r => r.HotelName == entityFilter);
    }

    // Apply text search
    if (!string.IsNullOrEmpty(searchText))
    {
        filteredReviews = filteredReviews.Where(r =>
            r.Feedback?.ToLower().Contains(searchText) == true ||
            r.Reviewer?.ToLower().Contains(searchText) == true ||
            r.TripName?.ToLower().Contains(searchText) == true ||
            r.HotelName?.ToLower().Contains(searchText) == true);
    }

    ReviewsDataGrid.ItemsSource = new ObservableCollection<ReviewViewModel>(filteredReviews);
}
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReviewsAsync();
            DetailPanel.IsVisible = false;
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
        public string TripName { get; set; }
        public string HotelName { get; set; }
        public string RelatedEntityName => ReviewType == "Trip" ? TripName : 
            ReviewType == "Hotel" ? HotelName : 
            "Unknown";
    }
}