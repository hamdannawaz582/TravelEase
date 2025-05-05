using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace DB_Project.CompanyPages
{
    public partial class CompanyHotel : UserControl
    {
        public ObservableCollection<Assignment> Assignments { get; set; }

        public CompanyHotel()
        {
            InitializeComponent();
            DataContext = this;
            LoadAssignments();
            AssignmentsContainer.ItemsSource = Assignments;
        }

        private void LoadAssignments()
        {
            Assignments = new ObservableCollection<Assignment>
            {
                new Assignment
                {
                    Username = "john_doe",
                    TripTitle = "Summer Vacation",
                    Hotel = "Grand Plaza Hotel",
                    Service = "Airport Transfer"
                },
                new Assignment
                {
                    Username = "jane_smith",
                    TripTitle = "Business Conference",
                    Hotel = "Seaside Resort",
                    Service = "Daily Breakfast"
                }
            };
        }

        private void OnAssignClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                // Add your assignment logic here
                // Example: Save to database, update status, etc.
                // You can access all fields:
                // assignment.Username
                // assignment.TripTitle
                // assignment.Hotel
                // assignment.Service
            }
        }
    }

    public class Assignment
    {
        public string Username { get; set; }
        public string TripTitle { get; set; }
        public string Hotel { get; set; }
        public string Service { get; set; }
    }
}