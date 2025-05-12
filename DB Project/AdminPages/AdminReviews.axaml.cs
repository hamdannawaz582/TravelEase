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
using Avalonia.Layout;

namespace DB_Project.AdminPages
{
    public partial class AdminReviews : UserControl
    {
        private ObservableCollection<ReviewViewModel> Reviews;
        private ReviewViewModel SelectedReview;
        private AdminRepository Repository;
        private StackPanel ReviewsStackPanel;
        
        private readonly List<string> _inappropriateWords = new List<string> {
            "awful", "terrible", "hate", "stupid", "idiot", "dumb", "ridiculous",
            "sucks", "worst", "garbage", "trash", "crap", "rubbish", "horrible",
            "pathetic", "useless", "scam", "fraud", "ripoff", "cheater", "liar",
            "offensive", "disgusting", "nasty", "horrendous", "atrocious"
        };

        public AdminReviews()
        {
            InitializeComponent();
            Reviews = new ObservableCollection<ReviewViewModel>();
            Repository = new AdminRepository();
            ReviewsStackPanel = this.FindControl<StackPanel>("ReviewsPanel");
            
            this.AttachedToVisualTree += (s, e) => LoadReviewsAsync();
        }

        private async void LoadReviewsAsync()
        {
            Reviews.Clear();
            ReviewsStackPanel.Children.Clear();

            try
            {
                var reviews = await Repository.GetAllReviews();
                var flaggedCount = 0;

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

                        if (review.FlaggedWords.Count > 0)
                            flaggedCount++;
                    }

                    Reviews.Add(review);
                    var reviewCard = CreateReviewCard(review);
                    ReviewsStackPanel.Children.Add(reviewCard);
                }

                ReviewCountText.Text = $"Total: {reviews.Count} | Flagged: {flaggedCount}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reviews: {ex.Message}");
            }
        }

        private Control CreateReviewCard(ReviewViewModel review)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Color.Parse("#3c3c3c")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Background = new SolidColorBrush(Color.Parse("#2d2d2d"))
            };
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*, Auto")
            };
            var contentPanel = new StackPanel { Spacing = 5 };
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            headerPanel.Children.Add(new TextBlock
            {
                Text = review.Reviewer,
                FontWeight = FontWeight.Bold,
                FontSize = 16
            });
            
            var starsText = new TextBlock
            {
                Text = $"{review.Stars} ★",
                Foreground = new SolidColorBrush(Color.Parse("#FFD700")),
                FontWeight = FontWeight.Bold
            };
            headerPanel.Children.Add(starsText);
            
            var dateText = new TextBlock
            {
                Text = review.FormattedDate,
                Foreground = new SolidColorBrush(Color.Parse("#aaaaaa")),
                FontSize = 12
            };
            headerPanel.Children.Add(dateText);
            contentPanel.Children.Add(headerPanel);
            contentPanel.Children.Add(new TextBlock
            {
                Text = review.Feedback,
                TextWrapping = TextWrapping.Wrap
            });

            if (!string.IsNullOrEmpty(review.Response))
            {
                var responsePanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
                responsePanel.Children.Add(new TextBlock
                {
                    Text = "Response:",
                    FontWeight = FontWeight.Bold
                });
                responsePanel.Children.Add(new TextBlock
                {
                    Text = review.Response,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.Parse("#aaaaaa"))
                });
                contentPanel.Children.Add(responsePanel);
            }
            if (review.FlaggedWords.Count > 0)
            {
                contentPanel.Children.Add(new TextBlock
                {
                    Text = $"Flagged words: {string.Join(", ", review.FlaggedWords)}",
                    Foreground = new SolidColorBrush(Color.Parse("#ff6b6b")),
                    FontWeight = FontWeight.Bold,
                    Margin = new Thickness(0, 5, 0, 0)
                });
            }

            grid.Children.Add(contentPanel);
            Grid.SetColumn(contentPanel, 0);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 5,
                Width = 100
            };

            var responseButton = new Button
            {
                Content = "Response",
                Width = 100
            };
            responseButton.Click += (s, e) => ShowResponseDialog(review);

            var deleteButton = new Button
            {
                Content = "Delete",
                Width = 100,
                Background = new SolidColorBrush(Color.Parse("#8b0000"))
            };
            deleteButton.Click += async (s, e) => await DeleteReview(review);

            buttonPanel.Children.Add(responseButton);
            buttonPanel.Children.Add(deleteButton);

            grid.Children.Add(buttonPanel);
            Grid.SetColumn(buttonPanel, 1);

            border.Child = grid;
            return border;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var showOnlyFlagged = FlaggedOnlyCheckBox.IsChecked ?? false;
            ReviewsStackPanel.Children.Clear();

            var filteredReviews = showOnlyFlagged 
                ? Reviews.Where(r => r.FlaggedWords.Count > 0)
                : Reviews;

            foreach (var review in filteredReviews)
            {
                var reviewCard = CreateReviewCard(review);
                ReviewsStackPanel.Children.Add(reviewCard);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FlaggedOnlyCheckBox.IsChecked = false;
            LoadReviewsAsync();
        }

        private void ShowResponseDialog(ReviewViewModel review)
        {
            SelectedReview = review;
            ResponseTextBox.Text = review.Response ?? "";
            ResponsePanel.IsVisible = true;
        }

        private void CloseResponse_Click(object sender, RoutedEventArgs e)
        {
            ResponsePanel.IsVisible = false;
        }

        private async void SaveResponse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReview == null) return;

            string response = ResponseTextBox?.Text?.Trim() ?? "";
            bool success = await Repository.UpdateReviewResponse(SelectedReview.ReviewID, response);

            if (success)
            {
                SelectedReview.Response = response;
                SelectedReview.ResponseDate = DateTime.Now;
                LoadReviewsAsync();
            }

            ResponsePanel.IsVisible = false;
        }

        private async Task DeleteReview(ReviewViewModel review)
        {
            try
            {
                bool result = await Repository.DeleteReview(review.ReviewID);
                if (result)
                {
                    Reviews.Remove(review);
                    LoadReviewsAsync();
                    
                    // Update the counter
                    int flaggedCount = Reviews.Count(r => r.FlaggedWords.Count > 0);
                    ReviewCountText.Text = $"Total: {Reviews.Count} | Flagged: {flaggedCount}";
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