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
        private readonly TravellerRepository repo = new TravellerRepository();
        private string Username;
        private TravellerProfile _profile;
        private ObservableCollection<TripItem> _trips;
        private ObservableCollection<TravelPass> _travelPasses;
        private ObservableCollection<ItineraryItem> _itineraries;
        private ObservableCollection<TravelHistoryItem> _travelHistory;
        private ContentControl _pageHost;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TravellerDashboard(string username)
        {
            InitializeComponent();
            Username = username;
            WelcomeMessage.Text = $"Welcome, {username}!";
            if (this.Parent is ContentControl contentControl)
            {
                _pageHost = contentControl;
            }
            LoadProfile();
            LoadUpcomingTripsAsync();
            LoadTravelPassesAsync();
            LoadItinerariesAsync();
            LoadTravelHistoryAsync();
        }

        private async Task LoadProfile()
        {
            try
            {
                _profile = await repo.GetProfile(Username);
                if (_profile != null)
                {
                    EmailTextBox.Text = _profile.Email;
                    FirstNameTextBox.Text = _profile.FirstName;
                    LastNameTextBox.Text = _profile.LastName;
                    NationalityTextBox.Text = _profile.Nationality;
                    AgeTextBox.Text = _profile.Age.ToString();
                    BudgetTextBox.Text = _profile.Budget.ToString();
                    JoinDateText.Text = _profile.JoinDate.ToString("MMMM dd, yyyy");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile: {ex.Message}");
            }
        }
        private async void SaveProfileChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (!int.TryParse(AgeTextBox.Text, out int age))
                {
                    await ShowErrorMessage("Age must be a valid number.");
                    return;
                }
        
                if (!int.TryParse(BudgetTextBox.Text, out int budget))
                {
                    await ShowErrorMessage("Budget must be a valid number.");
                    return;
                }

                var updatedProfile = new TravellerProfile
                {
                    Username = Username,
                    Email = EmailTextBox.Text,
                    FirstName = FirstNameTextBox.Text,
                    LastName = LastNameTextBox.Text,
                    Nationality = NationalityTextBox.Text,
                    Age = age,
                    Budget = budget,
                    JoinDate = _profile.JoinDate  
                };
                
                bool success = await repo.UpdateProfile(updatedProfile);

                if (success)
                {
                    _profile = updatedProfile;
                    await ShowSuccessMessage("Profile updated successfully.");
                }
                else
                {
                    await ShowErrorMessage("Failed to update profile. Please try again.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"Error updating profile: {ex.Message}");
            }
        }

        private async void LoadUpcomingTripsAsync()
        {
            try
            {
                var upcomingTrips = await repo.GetUpcomingTrips(Username);
                _trips = new ObservableCollection<TripItem>();
                
                foreach (var trip in upcomingTrips)
                {
                    _trips.Add(new TripItem 
                    { 
                        TripID = trip.TripID,
                        Destination = trip.Destination, 
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
            if (TravelPassesControl == null)
                return;

            var repo = new TravellerRepository();

            try
            {   var passes = await repo.GetTravelPasses(Username);
                var allDigitalPasses = new ObservableCollection<TravelPass>();
                foreach (var pass in passes)
                {  
                    allDigitalPasses.Add(pass);
                }
                TravelPassesControl.ItemsSource = new ObservableCollection<TravelPass>(passes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading travel passes: {ex.Message}");
                TravelPassesControl.ItemsSource = new ObservableCollection<TravelPass>
                {
                    new TravelPass
                    {
                        TripName = "Indian Heritage Tour",
                        ValidFrom = "Apr 20, 2025",
                        ValidTo = "Apr 30, 2025",
                        PassCode = "PASS-302-IND",
                        HotelVoucher = "Taj Retreat - Check-in on Apr 20",
                        ActivityPass = "Delhi Cultural Tour Pass"
                    },
                    new TravelPass
                    {
                        TripName = "Turkish Tour",
                        ValidFrom = "Jul 10, 2025",
                        ValidTo = "Jul 20, 2025",
                        PassCode = "PASS-305-TUR",
                        HotelVoucher = "Istanbul Palace - Check-in on Jul 10",
                        ActivityPass = "Bosphorus Explorer Pass"
                    }
                };
            }
        }

        private async void LoadItinerariesAsync()
        {
            if (ItinerariesControl == null)
                return;
            var repo = new TravellerRepository();
            try
            {
                var upcomingTrips = await repo.GetUpcomingTrips(Username);
                var allItineraries = new ObservableCollection<ItineraryItem>();
                foreach (var trip in upcomingTrips)
                {
                    var tripItineraries = await repo.GetTripItineraries(trip.TripID);
                    foreach (var item in tripItineraries)
                    {
                        allItineraries.Add(item);
                    }
                }
                ItinerariesControl.ItemsSource = allItineraries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading itineraries: {ex.Message}");
                ItinerariesControl.ItemsSource = new ObservableCollection<ItineraryItem>
                {
                    new ItineraryItem
                    {
                        Event = "City Tour",
                        EventStartDate = "May 15, 2025 9:00 AM",
                        EventEndDate = "May 15, 2025 4:00 PM"
                    },
                    new ItineraryItem
                    {
                        Event = "Museum Visit",
                        EventStartDate = "May 16, 2025 10:00 AM",
                        EventEndDate = "May 16, 2025 1:00 PM"
                    }
                };
            }
        }

        private async void LoadTravelHistoryAsync()
        {
            try
            {
                var tripHistory = await repo.GetTripHistory(Username);
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

                if (this.Parent is ContentControl contentControl)
                {
                    _pageHost = contentControl;
                    _pageHost.Content = new TripDetailsPage(_pageHost, trip.TripID, Username);
                }
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
            Console.WriteLine("Editing profile");
        }

        private async void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_profile != null)
                {bool success = await repo.UpdateProfile(_profile);
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
        private async void RefreshButton_Click(object? sender, RoutedEventArgs e)
        {
            if (RefreshButton != null)
                RefreshButton.IsEnabled = false;
            try
            {
                LoadProfile();
                LoadUpcomingTripsAsync();
                LoadTravelPassesAsync();
                LoadItinerariesAsync();
                LoadTravelHistoryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing dashboard: {ex.Message}");
                WelcomeMessage.Text = "Error refreshing. Try again.";
        
            }
        }
        private async Task ShowErrorMessage(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }
        private async Task ShowSuccessMessage(string message)
        {
            Console.WriteLine($"SUCCESS: {message}");
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
    
    public class TravelHistoryItem
    {
        public string TripName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}