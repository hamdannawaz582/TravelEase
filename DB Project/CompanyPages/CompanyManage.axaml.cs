using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages
{
    public partial class CompanyManage : UserControl
    {
        public ObservableCollection<Booking> Bookings { get; set; }
        public string operatorUsername;
        public CompanyManage(string username)
        
        {
            InitializeComponent();
            operatorUsername = username;
            // Console.WriteLine(UserNameText);
            DataContext = this;
            LoadBookings();
            BookingsContainer.ItemsSource = Bookings;
        }

        private void LoadBookings()
        {
            Bookings = new ObservableCollection<Booking>();

            string query = @"
        SELECT t.Title AS TripTitle, tb.Username AS TravelerName
        FROM Trip_Booking tb
        JOIN Trip t ON tb.TripID = t.TripID
        WHERE t.OperatorUsername = @operatorUsername";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@operatorUsername", operatorUsername);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bookings.Add(new Booking
                            {
                                TravelerName = reader["TravelerName"].ToString(),
                                TripTitle = reader["TripTitle"].ToString()
                            });
                        }
                    }
                }

                BookingsContainer.ItemsSource = Bookings;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
        // private void LoadBookings()
        // {
        //     Bookings = new ObservableCollection<Booking>
        //     {
        //         new Booking { TripTitle = "Alps Hiking", TravelerName = "Alice Johnson" },
        //         new Booking { TripTitle = "Desert Safari", TravelerName = "Bob Smith" },
        //         new Booking { TripTitle = "City Lights Tour", TravelerName = "Charlie Rose" }
        //     };
        // }

        private void OnSendReminderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Booking booking)
            {
                button.Content = "Reminder Sent";
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