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
        SELECT Title, PriceRange, Type, CancelStatus, GroupSize, StartDate, EndDate, TripID
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
                                EndDate = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd"),
                                TripID = reader["TripID"].ToString()
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
                string query = @"
            UPDATE Trip
            SET Title = @Title,
                PriceRange = @Price,
                Type = @Category,
                CancelStatus = @CancelStatus,
                GroupSize = @GroupSize,
                StartDate = @StartDate,
                EndDate = @EndDate
            WHERE TripID = @TripID";

                try
                {
                    using (var connection = DatabaseService.Instance.CreateConnection())
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", trip.Title);
                        command.Parameters.AddWithValue("@Price", Convert.ToInt32(trip.Price));
                        command.Parameters.AddWithValue("@Category", trip.Category);
                        command.Parameters.AddWithValue("@CancelStatus", trip.CancelStatus == "Not Cancelled" ? 0 : 1);
                        command.Parameters.AddWithValue("@GroupSize", trip.GroupSize);
                        command.Parameters.AddWithValue("@StartDate", DateTime.Parse(trip.StartDate));
                        command.Parameters.AddWithValue("@EndDate", DateTime.Parse(trip.EndDate));
                        command.Parameters.AddWithValue("@TripID", Convert.ToInt32(trip.TripID));

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            Console.WriteLine("Trip updated successfully.");
                        else
                            Console.WriteLine("No trip was updated. Check TripID.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected Error: {ex.Message}");
                }
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
        public string TripID { get; set; }
    }
}