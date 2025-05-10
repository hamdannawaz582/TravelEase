using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace DB_Project.AdminPages
{
    public partial class AdminManagement : UserControl
    {
        public ObservableCollection<RegistrationRequest> PendingRegistrations { get; set; }

        public AdminManagement()
        {
            InitializeComponent();
            DataContext = this;
            LoadRegistrations();
            RegistrationsContainer.ItemsSource = PendingRegistrations;
        }
        
        private void LoadRegistrations()
        {
            PendingRegistrations = new ObservableCollection<RegistrationRequest>();

            string connectionString = "Server=localhost,1433;Database=TravelEase;User Id=sa;Password=Racseson1122;Encrypt=false;TrustServerCertificate=true;";
            string query = "SELECT Username, Email, JoinDate FROM [User] WHERE Status = 'Pending'";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PendingRegistrations.Add(new RegistrationRequest
                            {
                                Name = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                JoinDate = Convert.ToDateTime(reader["JoinDate"]).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }

                Console.WriteLine("Pending registrations loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pending registrations: {ex.Message}");
            }
        }


        private void OnApproveClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RegistrationRequest request)
            {
                UpdateUserStatus(request.Name, "Approved");
                PendingRegistrations.Remove(request);
            }
        }

        private void OnRejectClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RegistrationRequest request)
            {
                UpdateUserStatus(request.Name, "Rejected");
                PendingRegistrations.Remove(request);
            }
        }

        private void UpdateUserStatus(string username, string newStatus)
        {
            string connectionString = "Server=localhost,1433;Database=TravelEase;User Id=sa;Password=Racseson1122;Encrypt=false;TrustServerCertificate=true;";
            string query = "UPDATE [User] SET Status = @Status WHERE Username = @Username";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine($"Status updated to {newStatus} for {username}.");
                    else
                        Console.WriteLine($"No rows updated for {username}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
            }
        }
    }

    public class RegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string JoinDate { get; set; }
    }
}