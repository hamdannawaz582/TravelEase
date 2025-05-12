using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DB_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Project.AdminPages
{
    public partial class AdminReviews : UserControl
    {
        private ObservableCollection<ReviewViewModel> _reviews;
        private ReviewViewModel _selectedReview;
        private AdminRepository _repository;

        // Define inappropriate words to check for
        private readonly List<string> _inappropriateWords = new List<string> {
            "awful", "terrible", "hate", "stupid", "idiot", "dumb", "ridiculous",
            "sucks", "worst", "garbage", "trash", "crap", "rubbish", "horrible",
            "pathetic", "useless", "scam", "fraud", "ripoff", "cheater", "liar",
            "offensive", "disgusting", "nasty", "horrendous", "atrocious"
        };

        public AdminReviews()
        {
            InitializeComponent();
            _reviews = new ObservableCollection<ReviewViewModel>();
            _repository = new AdminRepository();
            ReviewsDataGrid.ItemsSource = _reviews;
            LoadReviewsAsync();
        }

        private async void LoadReviewsAsync()
        {
            _reviews.Clear();

            try
            {
                var reviews = await _repository.GetAllReviews();
                foreach (var review in reviews)
                {
                    if (!string.IsNullOrEmpty(review.Feedback))
                    {
                        var words = review.Feedback.ToLower().Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '-', '\n', '\r' },
                            StringSplitOptions.RemoveEmptyEntries);

                        foreach (var word in words)
                        {
                            if (_inappropriateWords.Contains(word.Trim()))
                            {
                                review.FlaggedWords.Add(word);
                            }
                        }
                        
                        review.FlagCount = review.FlaggedWords.Count.ToString();
                        review.FlagColor = review.FlaggedWords.Count > 0 ? Brushes.OrangeRed : Brushes.Transparent;
                    }

                    _reviews.Add(review);
                    Console.WriteLine(review.Feedback);
                }

                ReviewsDataGrid.ItemsSource = _reviews;
                ReviewCountText.Text = $"Total: {_reviews.Count} | Flagged: {_reviews.Count(r => r.FlaggedWords.Count > 0)}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reviews: {ex.Message}");
            }
        }

        private void ReviewsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewsDataGrid.SelectedItem is ReviewViewModel selected)
            {
                _selectedReview = selected;
                DetailPanel.IsVisible = true;

                // Format the review content
                SelectedReviewContent.Text = $"Reviewer: {selected.Reviewer}\n" +
                                            $"Date: {selected.ReviewDate:MM/dd/yyyy}\n" +
                                            $"Rating: {selected.Stars} stars\n" +
                                            $"Content: {selected.Feedback}\n\n" +
                                            $"{(selected.Response != null ? $"Response: {selected.Response}" : "No response yet")}";

                // Display flagged words if any
                if (selected.FlaggedWords.Count > 0)
                {
                    FlaggedWordsText.Text = $"Flagged inappropriate words: {string.Join(", ", selected.FlaggedWords)}";
                    FlaggedWordsText.IsVisible = true;
                }
                else
                {
                    FlaggedWordsText.IsVisible = false;
                }
            }
            else
            {
                DetailPanel.IsVisible = false;
                _selectedReview = null;
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var showOnlyFlagged = FlaggedOnlyCheckBox.IsChecked ?? false;

            if (showOnlyFlagged)
            {
                ReviewsDataGrid.ItemsSource = new ObservableCollection<ReviewViewModel>(
                    _reviews.Where(r => r.FlaggedWords.Count > 0));
            }
            else
            {
                ReviewsDataGrid.ItemsSource = _reviews;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FlaggedOnlyCheckBox.IsChecked = false;
            LoadReviewsAsync();
            DetailPanel.IsVisible = false;
        }

        private void ViewResponse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview != null)
            {
                ResponseTextBox.Text = _selectedReview.Response ?? "";
                ResponsePanel.IsVisible = true;
            }
        }

        private void CloseResponse_Click(object sender, RoutedEventArgs e)
        {
            ResponsePanel.IsVisible = false;
        }

        private async void SaveResponse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            string response = ResponseTextBox?.Text?.Trim() ?? "";
            bool success = await _repository.UpdateReviewResponse(_selectedReview.ReviewID, response);
            
            if (success)
            {
                _selectedReview.Response = response;
                _selectedReview.ResponseDate = DateTime.Now;
                
                // Update the review details
                SelectedReviewContent.Text = $"Reviewer: {_selectedReview.Reviewer}\n" +
                                           $"Date: {_selectedReview.ReviewDate:MM/dd/yyyy}\n" +
                                           $"Rating: {_selectedReview.Stars} stars\n" +
                                           $"Content: {_selectedReview.Feedback}\n\n" +
                                           $"Response: {_selectedReview.Response}";
            }
            
            ResponsePanel.IsVisible = false;
        }

        private async void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReview == null) return;

            try
            {
                bool result = await _repository.DeleteReview(_selectedReview.ReviewID);
                if (result)
                {
                    _reviews.Remove(_selectedReview);
                    _selectedReview = null;
                    DetailPanel.IsVisible = false;
                    
                    // Update the counter
                    int flaggedCount = _reviews.Count(r => r.FlaggedWords.Count > 0);
                    ReviewCountText.Text = $"Total: {_reviews.Count} | Flagged: {flaggedCount}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting review: {ex.Message}");
            }
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
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public List<string> FlaggedWords { get; set; } = new List<string>();
        public string FlagCount { get; set; }
        public IBrush FlagColor { get; set; }
    }
}