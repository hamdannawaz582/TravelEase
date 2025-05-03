using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

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

    private void Login_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine($"Name: {UserName}");
        Console.WriteLine($"Password: {Password}");
        Console.WriteLine($"Login: {Entry}");
        bool isAuthenticated = true;
    
        if (isAuthenticated)
        {
            //finding the main window
            Login.Background = Brushes.Blue;
            if (this.VisualRoot is Window mainWindow)
            {
                ((MainWindow)mainWindow).NavigateToUserDashboard(Entry, UserName);
            }
        }
        else
        {
            // Display authentication error
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