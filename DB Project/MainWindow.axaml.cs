using Avalonia.Controls;
using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DB_Project.AdminPages;

namespace DB_Project;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new LoginPage();
    }
    public void NavigateToUserDashboard(string userType, string username)
    {
        switch (userType)
        {
            case "Admin":
                MainContent.Content = new AdminShell(username);
                break;
            case "Traveller":
                MainContent.Content = new TravellerShell(username);
                break;
            case "Operator":
                MainContent.Content = new OperatorShell(username);
                break;
            case "Hotel":
                MainContent.Content = new HotelShell(username);
                break;
            default:
                MainContent.Content = new LoginPage();
                break;
        }
    }
    
}