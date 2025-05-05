using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace DB_Project.CompanyPages
{
    public partial class CompanyViewEdit : UserControl
    {
        public ObservableCollection<Trip> Trips { get; set; }

        public CompanyViewEdit()
        {
            InitializeComponent();
            DataContext = this;
            LoadTrips();
            TripsContainer.ItemsSource = Trips;
        }

        private void LoadTrips()
        {
            Trips = new ObservableCollection<Trip>
            {
                new Trip
                {
                    Title = "Jungle Expedition",
                    Price = "499.99",
                    Category = "Adventure",
                    CancelStatus = "Not Cancelled",
                    GroupSize = 12,
                    StartDate = "2025-06-10",
                    EndDate = "2025-06-20"
                },
                new Trip
                {
                    Title = "Island Escape",
                    Price = "799.00",
                    Category = "Vacation",
                    CancelStatus = "Cancelled",
                    GroupSize = 6,
                    StartDate = "2025-07-01",
                    EndDate = "2025-07-10"
                }
            };
        }

        private void OnEditTripClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Trip trip)
            {
                //TODO: stuff..
            }
        }
    }

    public class Trip
    {
        public string Title { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
        public string CancelStatus { get; set; }
        public int GroupSize { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}