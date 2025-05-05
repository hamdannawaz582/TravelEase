using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace DB_Project.CompanyPages
{
    public partial class CompanyManage : UserControl
    {
        public ObservableCollection<Booking> Bookings { get; set; }

        public CompanyManage()
        {
            InitializeComponent();
            DataContext = this;
            LoadBookings();
            BookingsContainer.ItemsSource = Bookings;
        }

        private void LoadBookings()
        {
            Bookings = new ObservableCollection<Booking>
            {
                new Booking { TripTitle = "Alps Hiking", TravelerName = "Alice Johnson" },
                new Booking { TripTitle = "Desert Safari", TravelerName = "Bob Smith" },
                new Booking { TripTitle = "City Lights Tour", TravelerName = "Charlie Rose" }
            };
        }

        private void OnSendReminderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Booking booking)
            {
                button.Content = "✓ Reminder Sent";
            }
        }

        private void OnCancelBookingClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Booking booking)
            {
                Bookings.Remove(booking);
                BookingsContainer.ItemsSource = new ObservableCollection<Booking>(Bookings); //refresh
            }
        }
    }

    public class Booking
    {
        public string TripTitle { get; set; }
        public string TravelerName { get; set; }
    }
}