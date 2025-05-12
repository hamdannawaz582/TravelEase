using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                SELECT tb.Username, t.Title, t.TripID, t.StartDate, t.EndDate
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
                                StartDate = Convert.ToDateTime(bookingReader["StartDate"]).ToString("yyyy-MM-dd"),
                                EndDate = Convert.ToDateTime(bookingReader["EndDate"]).ToString("yyyy-MM-dd"),
                                AvailableHotels = new ObservableCollection<string>(hotels),
                                AvailableServices = new ObservableCollection<string>()
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



        
        private void OnAssignHotelClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                string serviceQuery = @"
            SELECT DISTINCT ServiceType
            FROM Room_Service
            WHERE HUsername = @HUsername";
            
            var services = new ObservableCollection<string>();
            
                try
                {
                    using (var connection = DatabaseService.Instance.CreateConnection())
                    using (var command = new SqlCommand(serviceQuery, connection))
                    {
                        command.Parameters.AddWithValue("@HUsername", assignment.Hotel);
                        connection.Open();
            
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                services.Add(reader["ServiceType"].ToString());
                                Console.WriteLine($"Service added: {reader["ServiceType"]}");
                            }
                        }
                    }

                     // Optional fallback
                     assignment.AvailableServices.Clear();
                     foreach (var service in services)
                     {
                         assignment.AvailableServices.Add(service);
                     }
                     assignment.AvailableServices.Add("None");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading services: {ex.Message}");
                }
            }
        }

        private void OnAssignServiceClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Assignment assignment)
            {
                string query = @"
                    SELECT TOP 1 RoomNumber
                    FROM Room_Service
                    WHERE HUsername = @HUsername AND ServiceType = @ServiceType";

                string roomNumber = "";
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HUsername", assignment.Hotel);
                    command.Parameters.AddWithValue("@ServiceType", assignment.Service);
        
                    connection.Open();

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        roomNumber = result.ToString();
                        Console.WriteLine($"Retrieved RoomNumber: {roomNumber}");
                    }
                    else
                    {
                        Console.WriteLine("No matching room found.");
                    }
                }
                
                string insertQuery = @"
                    INSERT INTO Room_Booking (HUsername, RoomId, Username, StartDate, BookingDate, EndDate)
                    VALUES (@HUsername, @RoomId, @Username, @StartDate, @BookingDate, @EndDate);
                ";

                try
                {
                    using (var connection = DatabaseService.Instance.CreateConnection())
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@HUsername", assignment.Hotel);
                        command.Parameters.AddWithValue("@RoomId", roomNumber);
                        command.Parameters.AddWithValue("@Username", assignment.Username);
                        command.Parameters.AddWithValue("@StartDate", assignment.StartDate);
                        command.Parameters.AddWithValue("@BookingDate", 
                            DateTime.ParseExact(assignment.StartDate, "yyyy-MM-dd", null).AddDays(1));
                        command.Parameters.AddWithValue("@EndDate", assignment.EndDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Room booking inserted successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting room booking: {ex.Message}");
                }

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
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ObservableCollection<string> AvailableHotels { get; set; }
        // public ObservableCollection<string> AvailableServices { get; set; }
        private ObservableCollection<string> _availableServices;

        public ObservableCollection<string> AvailableServices
        {
            get => _availableServices;
            set
            {
                _availableServices = value;
                OnPropertyChanged(nameof(AvailableServices));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}