using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages
{
    public partial class CompanyHotel : UserControl
    {
        public ObservableCollection<Assignment> Assignments { get; set; }
        string OperatorUsername;
        public CompanyHotel(string OpUsername)
        {
            InitializeComponent();
            DataContext = this;
            OperatorUsername = OpUsername;
            LoadAssignments();
            AssignmentsContainer.ItemsSource = Assignments;
        }
        
        private void LoadAssignments()
        {
            Assignments = new ObservableCollection<Assignment>();

            string bookingQuery = @"
                SELECT tb.Username, t.Title, t.TripID
                FROM Trip_Booking tb
                JOIN Trip t ON tb.TripID = t.TripID
                WHERE t.OperatorUsername = @OperatorUsername";

            string hotelQuery = "SELECT HUsername FROM Trip_Hotels WHERE TripID = @TripID";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var bookingCommand = new SqlCommand(bookingQuery, connection))
                {
                    bookingCommand.Parameters.AddWithValue("@OperatorUsername", OperatorUsername);
                    connection.Open();

                    using (var bookingReader = bookingCommand.ExecuteReader())
                    {
                        while (bookingReader.Read())
                        {
                            string tripID = bookingReader["TripID"].ToString();
                            var hotels = new List<string>();

                            using (var hotelConnection = DatabaseService.Instance.CreateConnection())
                            using (var hotelCommand = new SqlCommand(hotelQuery, hotelConnection))
                            {
                                hotelCommand.Parameters.AddWithValue("@TripID", tripID);
                                hotelConnection.Open();

                                using (var hotelReader = hotelCommand.ExecuteReader())
                                {
                                    while (hotelReader.Read())
                                    {
                                        hotels.Add(hotelReader["HUsername"].ToString());
                                        
                                    }
                                }
                            }
                            hotels.Add("None");

                            Assignments.Add(new Assignment
                            {
                                Username = bookingReader["Username"].ToString(),
                                TripTitle = bookingReader["Title"].ToString(),
                                TripID = tripID,
                                Hotel = "",
                                Service = "",
                                AvailableHotels = new ObservableCollection<string>(hotels)
                            });
                        }
                    }

                    AssignmentsContainer.ItemsSource = Assignments;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error loading assignments: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error loading assignments: {ex.Message}");
            }
        }



        
        private void OnAssignClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                //TODO: stuff..
            }
        }
    }

    public class Assignment
    {
        public string Username { get; set; }
        public string TripTitle { get; set; }
        public string Hotel { get; set; }
        public string Service { get; set; }
        public string TripID { get; set; }
        public ObservableCollection<string> AvailableHotels { get; set; }

    }
}