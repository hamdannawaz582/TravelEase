using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using DB_Project.Repositories;

namespace DB_Project
{
    public partial class AdminSignupPage : UserControl
    {
        private AdminRepository repo = new AdminRepository();

        public AdminSignupPage()
        {
            InitializeComponent();
        }

        private void OnSubmit(object? sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var email = EmailBox.Text;
            var password = PasswordBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Missing required fields");
                return;
            }

            try
            {
                bool success = repo.RegisterAdmin(username, email, password);

                if (success)
                {
                    if (this.VisualRoot is Window mainWindow)
                    {
                        ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
                    }
                }
                else
                {
                    Console.WriteLine("Failed to register admin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
            }
        }
    }
}