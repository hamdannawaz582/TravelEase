using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Threading.Tasks;
using DB_Project.Models;
using DB_Project.Repositories;

namespace DB_Project.TravellerPages
{
    public partial class TravellerDashboard : UserControl, INotifyPropertyChanged
    {
        private readonly TravellerRepository _repository = new TravellerRepository();
        private string _username;
        private TravellerProfile _profile;
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

            LoadUserProfileAsync();
            LoadUpcomingTripsAsync();
            LoadTravelPassesAsync();
            LoadItinerariesAsync();
            LoadTravelHistoryAsync();
        }

        private async void LoadUserProfileAsync()
        {
            try
            {
                _profile = await _repository.GetProfile(_username);
                
                if (_profile != null)
                {
                    UsernameBlock.Text = $"Username: {_profile.Username}";
                    EmailBlock.Text = $"Email: {_profile.Email}";
                    FNameBlock.Text = $"First Name: {_profile.FirstName}";
                    LNameBlock.Text = $"Last Name: {_profile.LastName}";
                    NationalityBlock.Text = $"Nationality: {_profile.Nationality}";
                    AgeBlock.Text = $"Age: {_profile.Age}";
                    BudgetBlock.Text = $"Budget: ${_profile.Budget}";
                    JoinDateBlock.Text = $"Member Since: {_profile.JoinDate.ToShortDateString()}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile: {ex.Message}");
            }
        }

        private async void LoadUpcomingTripsAsync()
        {
            try
            {
                var upcomingTrips = await _repository.GetUpcomingTrips(_username);
                _trips = new ObservableCollection<TripItem>();
                
                foreach (var trip in upcomingTrips)
                {
                    _trips.Add(new TripItem 
                    { 
                        TripID = trip.TripID,
                        Destination = trip.Title, 
                        Date = $"{trip.StartDate.ToShortDateString()} - {trip.EndDate.ToShortDateString()}", 
                        Status = "Confirmed", 
                        CancellationPolicy = trip.CancellationPolicy 
                    });
                }
                
                UpcomingTripsControl.ItemsSource = _trips;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading upcoming trips: {ex.Message}");
                // Fallback to demo data if database fails
                _trips = new ObservableCollection<TripItem>
                {
                    new TripItem { Destination = "Paris Adventure", Date = "May 15-20, 2025", Status = "Confirmed", CancellationPolicy = "Refundable" },
                    new TripItem { Destination = "Tokyo Explorer", Date = "June 8-15, 2025", Status = "Pending", CancellationPolicy = "Non-Refundable" }
                };
                
                UpcomingTripsControl.ItemsSource = _trips;
            }
        }

        private async void LoadTravelPassesAsync()
        {
            try
            {
                // In a real implementation, this would fetch data from the database
                // Since the repository doesn't have this method yet, using demo data
                _travelPasses = new ObservableCollection<TravelPass>
                {
                    new TravelPass { TripName = "Paris Adventure", ValidFrom = "May 15, 2025", ValidTo = "May 20, 2025", PassCode = "PA-2025-12345", HotelVoucher = "Hotel Paris Voucher", ActivityPass = "Eiffel Tower Pass" },
                    new TravelPass { TripName = "Tokyo Explorer", ValidFrom = "June 8, 2025", ValidTo = "June 15, 2025", PassCode = "TX-2025-67890", HotelVoucher = "Tokyo Hotel Voucher", ActivityPass = "Mt. Fuji Pass" }
                };
                
                TravelPassesControl.ItemsSource = _travelPasses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading travel passes: {ex.Message}");
            }
        }

        private async void LoadItinerariesAsync()
        {
            try
            {
                var upcomingTrips = await _repository.GetUpcomingTrips(_username);
                _itineraries = new ObservableCollection<ItineraryItem>();
                
                if (upcomingTrips.Count > 0)
                {
                    // Since GetTripItineraries is commented out in repository
                    // Using demo data for now
                    _itineraries = new ObservableCollection<ItineraryItem>
                    {
                        new ItineraryItem { Event = "Flight to Paris", EventStartDate = "May 15, 2025", EventEndDate = "May 15, 2025" },
                        new ItineraryItem { Event = "City Tour", EventStartDate = "May 16, 2025", EventEndDate = "May 16, 2025" },
                        new ItineraryItem { Event = "Museum Visit", EventStartDate = "May 17, 2025", EventEndDate = "May 17, 2025" },
                        new ItineraryItem { Event = "Free Day", EventStartDate = "May 18, 2025", EventEndDate = "May 18, 2025" },
                        new ItineraryItem { Event = "Return Flight", EventStartDate = "May 20, 2025", EventEndDate = "May 20, 2025" }
                    };
                }
                
                ItinerariesControl.ItemsSource = _itineraries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading itineraries: {ex.Message}");
                // Fallback to demo data
                _itineraries = new ObservableCollection<ItineraryItem>
                {
                    new ItineraryItem { Event = "Flight to Paris", EventStartDate = "May 15, 2025", EventEndDate = "May 15, 2025" },
                    new ItineraryItem { Event = "City Tour", EventStartDate = "May 16, 2025", EventEndDate = "May 16, 2025" }
                };
                
                ItinerariesControl.ItemsSource = _itineraries;
            }
        }

        private async void LoadTravelHistoryAsync()
        {
            try
            {
                var tripHistory = await _repository.GetTripHistory(_username);
                _travelHistory = new ObservableCollection<TravelHistoryItem>();
                
                foreach (var trip in tripHistory)
                {
                    _travelHistory.Add(new TravelHistoryItem 
                    { 
                        TripName = trip.Title, 
                        StartDate = trip.StartDate.ToShortDateString(), 
                        EndDate = trip.EndDate.ToShortDateString() 
                    });
                }
                
                TravelHistoryControl.ItemsSource = _travelHistory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading travel history: {ex.Message}");
                // Fallback to demo data
                _travelHistory = new ObservableCollection<TravelHistoryItem>
                {
                    new TravelHistoryItem { TripName = "Rome Adventure", StartDate = "March 10, 2025", EndDate = "March 15, 2025" },
                    new TravelHistoryItem { TripName = "Barcelona Weekend", StartDate = "April 5, 2025", EndDate = "April 7, 2025" }
                };
                
                TravelHistoryControl.ItemsSource = _travelHistory;
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TripItem trip)
            {
                Console.WriteLine($"Viewing details for trip: {trip.Destination}");
                // TODO: Implement navigation to trip details page
            }
        }

        private void ViewPass_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TravelPass pass)
            {
                Console.WriteLine($"Viewing pass for trip: {pass.TripName}");
                // TODO: Implement digital pass viewing functionality, e.g., popup with QR code
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            // Enable editing of profile fields
            Console.WriteLine("Editing profile");
            // In a full implementation, you would make the profile fields editable here
        }

        private async void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_profile != null)
                {
                    // In a full implementation, you would gather edited values from UI fields
                    bool success = await _repository.UpdateProfile(_profile);
                    if (success)
                    {
                        Console.WriteLine("Profile updated successfully");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update profile");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
            }
        }
    }

    public class TripItem
    {
        public int TripID { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string CancellationPolicy { get; set; }
        public string ImagePath { get; set; } = "../Assets/paris.png"; // Default image
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