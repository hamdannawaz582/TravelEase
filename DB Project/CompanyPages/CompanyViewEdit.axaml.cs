using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using DB_Project.Services;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages
{
    public partial class CompanyViewEdit : UserControl
    {
        public ObservableCollection<Trip> Trips { get; set; }
        string OperatorUsername;
        public CompanyViewEdit(string OpUsername)
        {
            InitializeComponent();
            DataContext = this;
            OperatorUsername = OpUsername;
            LoadTrips();
            TripsContainer.ItemsSource = Trips;
        }

        private void LoadTrips()
        {
            Trips = new ObservableCollection<Trip>();

            string query = @"
        SELECT Title, PriceRange, Type, CancelStatus, GroupSize, StartDate, EndDate
        FROM Trip
        WHERE OperatorUsername = @OperatorUsername";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OperatorUsername", OperatorUsername);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string CStatus;
                            if (reader["CancelStatus"] == DBNull.Value)
                            {
                                CStatus = "Cancelled";
                            }
                            else
                            {
                                CStatus = "Not Cancelled";
                            }
                            Trips.Add(new Trip
                            {
                                Title = reader["Title"].ToString(),
                                Price = reader["PriceRange"].ToString(),
                                Category = reader["Type"].ToString(),
                                CancelStatus = CStatus,
                                GroupSize = Convert.ToInt32(reader["GroupSize"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd"),
                                EndDate = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }

                TripsContainer.ItemsSource = Trips;
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