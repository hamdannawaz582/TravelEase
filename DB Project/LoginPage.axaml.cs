using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DB_Project.Repositories;
using Microsoft.Data.SqlClient;

namespace DB_Project;

public partial class LoginPage : UserControl
{
    public LoginPage()
    {
        InitializeComponent();
        DataContext = this;
        EntryText.Text = "Traveller Entry";
    }
    public String Entry = "Traveller";

    public String UserName;
    public String Password;

    private async void Login_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine($"Name: {UserName}");
        Console.WriteLine($"Password: {Password}");
        Console.WriteLine($"Login: {Entry}");

        var repo = new TravellerRepository();
        bool isAuthenticated = false;
        try
        {
            switch (Entry)
            {
                case "Traveller":
                    isAuthenticated = repo.AuthenticateTraveller(UserName, Password);
                    break;
                case "Operator":
                    //isAuthenticated = repo.AuthenticateOperator(UserName, Password);
                    break;
                case "Hotel":
                    //isAuthenticated = repo.AuthenticateHotel(UserName, Password);
                    break;
                case "Admin":
                    //isAuthenticated = repo.AuthenticateAdmin(UserName, Password);
                    break;
                default:
                    Console.WriteLine("Invalid entry type");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        if (isAuthenticated)
        {
            Login.Background = Brushes.Blue;
            if (this.VisualRoot is Window mainWindow)
            {
                Console.WriteLine("Loginned");
                ((MainWindow)mainWindow).NavigateToUserDashboard(Entry, UserName);
            }
        }
        else
        {
            Login.Background = Brushes.Red;
        }
    }


    private void UsernameBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UserName = UsernameBox.Text;
    }

    private void PasswordBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Password = PasswordBox.Text;
    }

    private void Signup_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine($"Signup clicked for: {Entry}");
    
        try
        {
            if (this.VisualRoot is Window mainWindow)
            {
                Console.WriteLine("Found main window");
                
                Control signupPage;
                switch (Entry)
                {
                    case "Traveller":
                        signupPage = new TravellerSignupPage();
                        break;
                    case "Operator":
                        signupPage = new OperatorSignupPage();
                        break;
                    case "Hotel":
                        signupPage = new HotelSignupPage();
                        break;
                    case "Admin":
                        signupPage = new AdminSignupPage();
                        break;
                    default:
                        signupPage = new TravellerSignupPage();
                        break;
                }
            
                ((MainWindow)mainWindow).MainContent.Content = signupPage;
                Console.WriteLine("Navigation complete");
            }
            else
            {
                Console.WriteLine("Could not find main window");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during signup navigation: {ex.Message}");
        }
    }

    private void TravellerButton_OnTapped(object? sender, TappedEventArgs e)
    {
        Entry = "Traveller";
        EntryText.Text = $"{Entry} Entry";
    }

    private void OperatorButton_OnTapped(object? sender, TappedEventArgs e)
    {
        Entry = "Operator";
        EntryText.Text = $"{Entry} Entry";
    }

    private void HotelButton_OnTapped(object? sender, TappedEventArgs e)
    {
        Entry = "Hotel";
        EntryText.Text = $"{Entry} Entry";
    }

    private void AdminButton_OnTapped(object? sender, TappedEventArgs e)
    {
        Entry = "Admin";
        EntryText.Text = $"{Entry} Entry";
    }
}