using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DB_Project.TravellerPages
{
    public partial class TravellerReview : UserControl, INotifyPropertyChanged
    {
        private TripReviewItem _selectedTrip;
        private HotelReviewItem _selectedHotel;
        private int _selectedTripRating;
        private int _selectedHotelRating;
        private string _tripFeedback;
        private string _hotelFeedback;

        private ObservableCollection<TripReviewItem> _availableTrips;
        private ObservableCollection<HotelReviewItem> _availableHotels;
        private ObservableCollection<int> _ratings;

        public TripReviewItem SelectedTrip
        {
            get => _selectedTrip;
            set { _selectedTrip = value; OnPropertyChanged(); }
        }

        public HotelReviewItem SelectedHotel
        {
            get => _selectedHotel;
            set { _selectedHotel = value; OnPropertyChanged(); }
        }

        public int SelectedTripRating
        {
            get => _selectedTripRating;
            set { _selectedTripRating = value; OnPropertyChanged(); }
        }

        public int SelectedHotelRating
        {
            get => _selectedHotelRating;
            set { _selectedHotelRating = value; OnPropertyChanged(); }
        }

        public string TripFeedback
        {
            get => _tripFeedback;
            set { _tripFeedback = value; OnPropertyChanged(); }
        }

        public string HotelFeedback
        {
            get => _hotelFeedback;
            set { _hotelFeedback = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TravellerReview(string username)
        {
            InitializeComponent();
            DataContext = this;

            LoadAvailableTrips();
            LoadAvailableHotels();
            LoadRatings();
            
            TripSelector.ItemsSource = _availableTrips;
            HotelSelector.ItemsSource = _availableHotels;
            TripRatingSelector.ItemsSource = _ratings;
            HotelRatingSelector.ItemsSource = _ratings;
        }

        private void LoadAvailableTrips()
        {
            _availableTrips = new ObservableCollection<TripReviewItem>
            {
                new TripReviewItem { TripID = 1, Title = "Paris Adventure" },
                new TripReviewItem { TripID = 2, Title = "Tokyo Explorer" }
            };
        }

        private void LoadAvailableHotels()
        {
            _availableHotels = new ObservableCollection<HotelReviewItem>
            {
                new HotelReviewItem { HUsername = "hotel_paris", Name = "Hotel Paris" },
                new HotelReviewItem { HUsername = "tokyo_inn", Name = "Tokyo Inn" }
            };
        }

        private void LoadRatings()
        {
            _ratings = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        }

        private async void SubmitTripReview_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedTrip != null && SelectedTripRating > 0)
            {
                var review = new ReviewItem
                {
                    Stars = SelectedTripRating,
                    Feedback = TripFeedback,
                    ReviewTime = DateTime.Now,
                    Reviewer = "currentUsername"
                };
                
                await ShowMessageDialog($"Trip Review Submitted:\nTrip: {SelectedTrip.Title}\nRating: {review.Stars}\nFeedback: {review.Feedback}");
            }
            else
            {
                await ShowMessageDialog("Please select a trip and provide a valid rating.");
            }
        }

        private async void SubmitHotelReview_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedHotel != null && SelectedHotelRating > 0)
            {
                var review = new ReviewItem
                {
                    Stars = SelectedHotelRating,
                    Feedback = HotelFeedback,
                    ReviewTime = DateTime.Now,
                    Reviewer = "currentUsername"
                };
                
                await ShowMessageDialog($"Hotel Review Submitted:\nHotel: {SelectedHotel.Name}\nRating: {review.Stars}\nFeedback: {review.Feedback}");
            }
            else
            {
                await ShowMessageDialog("Please select a hotel and provide a valid rating.");
            }
        }

        private async Task ShowMessageDialog(string message)
        {
            var dialog = new Window
            {
                Width = 400,
                Height = 200,
                Content = new TextBlock
                {
                    Text = message,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                }
            };

            await dialog.ShowDialog((Window)this.VisualRoot);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class TripReviewItem
    {
        public int TripID { get; set; }
        public string Title { get; set; }
    }

    public class HotelReviewItem
    {
        public string HUsername { get; set; }
        public string Name { get; set; }
    }

    public class ReviewItem
    {
        public int ReviewID { get; set; }
        public int Stars { get; set; }
        public string Feedback { get; set; }
        public string Response { get; set; }
        public DateTime ReviewTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string Reviewer { get; set; }
    }
}