using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

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
            PendingRegistrations = new ObservableCollection<RegistrationRequest>
            {
                new RegistrationRequest { Name = "Alice Johnson", Email = "alice@example.com", Role = "User" },
                new RegistrationRequest { Name = "Bob Smith", Email = "bob@example.com", Role = "Operator" },
                new RegistrationRequest { Name = "Charlie Rose", Email = "charlie@example.com", Role = "User" }
            };
        }

        private void OnApproveClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RegistrationRequest request)
            {
                PendingRegistrations.Remove(request);
                // TODO: update the database with approval status
            }
        }

        private void OnRejectClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RegistrationRequest request)
            {
                PendingRegistrations.Remove(request);
                // TODO: update the database with rejection status
            }
        }
    }

    public class RegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}