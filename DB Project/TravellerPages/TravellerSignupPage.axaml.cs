using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using DB_Project.Repositories;

namespace DB_Project
{
    public partial class TravellerSignupPage : UserControl
    {
        private TravellerRepository repo = new TravellerRepository();

        public TravellerSignupPage()
        {
            InitializeComponent();
        }

        private void OnSubmit(object? sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var email = EmailBox.Text;
            var password = PasswordBox.Text;
            var fname = FNameBox.Text;
            var lname = LNameBox.Text;
            var nationality = NationalityBox.Text;
            int.TryParse(AgeBox.Text, out int age);
            int.TryParse(BudgetBox.Text, out int budget);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nationality))
            {
                Console.WriteLine("Missing required fields");
                return;
            }

            try
            {
                bool success = repo.RegisterTraveller(
                    username, email, password, fname, lname, nationality, age, budget);

                if (success)
                {
                    if (this.VisualRoot is Window mainWindow)
                    {
                        ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
                    }
                }
                else
                {
                    Console.WriteLine("Failed to register traveller");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
            }
        }
    }
}