using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using DB_Project.Repositories;

namespace DB_Project
{
    public partial class HotelSignupPage : UserControl
    {
        private readonly HotelRepository _repository = new HotelRepository();

        public HotelSignupPage()
        {
            InitializeComponent();
        }

        private void OnSubmit(object? sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var name = NameBox.Text;
            var email = EmailBox.Text;
            var password = PasswordBox.Text;
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(name) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("All fields are required!");
                return;
            }
            
            bool success = _repository.RegisterHotel(username, email, password, name);
            
            if (success)
            {
                if (this.VisualRoot is Window mainWindow)
                {
                    ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
                }
            }
            else
            {
                Console.WriteLine("Registration failed. Please try again.");
            }
        }
    }
}