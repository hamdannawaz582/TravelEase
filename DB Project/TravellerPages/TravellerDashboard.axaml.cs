using Avalonia.Controls;
using System.Collections.ObjectModel;

namespace DB_Project.TravellerPages
{
    public partial class TravellerDashboard : UserControl
    {
        private string _username;
        public TravellerDashboard(string username)
        {
            InitializeComponent();
            _username = username;
            
            WelcomeMessage.Text = $"Welcome, {username}!";
            
            // Set profile information
            UsernameBlock.Text = $"Username: {username}";
            TripCountBlock.Text = "Total Trips: 5"; // Replace with actual data
            MemberSinceBlock.Text = "Member Since: January 2025"; // Replace with actual data
            
            // Load upcoming trips (replace with database queries)
            LoadUpcomingTrips();
        }
        
        private void LoadUpcomingTrips()
        {
            var trips = new ObservableCollection<TripItem>
            {
                new TripItem { 
                    Destination = "Paris Adventure", 
                    Date = "May 15-20, 2025", 
                    Status = "Confirmed", 
                    ImagePath = "../Assets/paris.png" 
                },
                new TripItem { 
                    Destination = "Tokyo Explorer", 
                    Date = "June 8-15, 2025", 
                    Status = "Pending", 
                    ImagePath = "../Assets/tokyo.png" 
                }
                // Add more trips as needed
            };
            
            UpcomingTripsControl.ItemsSource = trips;
        }
    }
    
    public class TripItem
    {
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
    }
}