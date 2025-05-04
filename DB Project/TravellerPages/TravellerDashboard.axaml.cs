using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DB_Project.TravellerPages
{
    public partial class TravellerDashboard : UserControl, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TripItem> _trips;
        private ObservableCollection<TravelPass> _travelPasses;
        private ObservableCollection<ItineraryItem> _itineraries;
        private ObservableCollection<TravelHistoryItem> _travelHistory;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TravellerDashboard(string username)
        {
            InitializeComponent();
            _username = username;
            WelcomeMessage.Text = $"Welcome, {username}!";

            LoadUserProfile();
            LoadUpcomingTrips();
            LoadTravelPasses();
            LoadItineraries();
            LoadTravelHistory();

            UpcomingTripsControl.ItemsSource = _trips;
            TravelPassesControl.ItemsSource = _travelPasses;
            ItinerariesControl.ItemsSource = _itineraries;
            TravelHistoryControl.ItemsSource = _travelHistory;
        }

        private void LoadUserProfile()
        {
            UsernameBlock.Text = $"Username: {_username}";
            EmailBlock.Text = "Email: user@example.com";
            FNameBlock.Text = "First Name: John";
            LNameBlock.Text = "Last Name: Doe";
            NationalityBlock.Text = "Nationality: United States";
            AgeBlock.Text = "Age: 30";
            BudgetBlock.Text = "Budget: $5000";
            JoinDateBlock.Text = "Member Since: January 2025";
        }

        private void LoadUpcomingTrips()
        {
            _trips = new ObservableCollection<TripItem>
            {
                new TripItem { Destination = "Paris Adventure", Date = "May 15-20, 2025", Status = "Confirmed", CancellationPolicy = "Refundable" },
                new TripItem { Destination = "Tokyo Explorer", Date = "June 8-15, 2025", Status = "Pending", CancellationPolicy = "Non-Refundable" }
            };
        }

        private void LoadTravelPasses()
        {
            _travelPasses = new ObservableCollection<TravelPass>
            {
                new TravelPass { TripName = "Paris Adventure", ValidFrom = "May 15, 2025", ValidTo = "May 20, 2025", PassCode = "PA-2025-12345", HotelVoucher = "Hotel Paris Voucher", ActivityPass = "Eiffel Tower Pass" },
                new TravelPass { TripName = "Tokyo Explorer", ValidFrom = "June 8, 2025", ValidTo = "June 15, 2025", PassCode = "TX-2025-67890", HotelVoucher = "Tokyo Hotel Voucher", ActivityPass = "Mt. Fuji Pass" }
            };
        }

        private void LoadItineraries()
        {
            _itineraries = new ObservableCollection<ItineraryItem>
            {
                new ItineraryItem { Event = "Flight to Paris", EventStartDate = "May 15, 2025", EventEndDate = "May 15, 2025" },
                new ItineraryItem { Event = "City Tour", EventStartDate = "May 16, 2025", EventEndDate = "May 16, 2025" }
            };
        }

        private void LoadTravelHistory()
        {
            _travelHistory = new ObservableCollection<TravelHistoryItem>
            {
                new TravelHistoryItem { TripName = "Rome Adventure", StartDate = "March 10, 2025", EndDate = "March 15, 2025" },
                new TravelHistoryItem { TripName = "London Explorer", StartDate = "April 5, 2025", EndDate = "April 10, 2025" }
            };
        }

        private void EditProfile_Click(object? sender, RoutedEventArgs e)
        {
            // Logic for editing profile
        }

        private void SaveProfile_Click(object? sender, RoutedEventArgs e)
        {
            // Logic for saving profile
        }

        private void ViewDetails_Click(object? sender, RoutedEventArgs e)
        {
            // Logic for viewing trip details
        }

        private void ViewPass_Click(object? sender, RoutedEventArgs e)
        {
            // Logic for viewing travel pass
        }
    }

    public class TripItem
    {
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public string CancellationPolicy { get; set; }
    }

    public class TravelPass
    {
        public string TripName { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string PassCode { get; set; }
        public string HotelVoucher { get; set; }
        public string ActivityPass { get; set; }
    }

    public class ItineraryItem
    {
        public string Event { get; set; }
        public string EventStartDate { get; set; }
        public string EventEndDate { get; set; }
    }

    public class TravelHistoryItem
    {
        public string TripName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}