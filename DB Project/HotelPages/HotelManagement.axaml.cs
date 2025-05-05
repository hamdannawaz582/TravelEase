using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DB_Project.CompanyPages;

namespace DB_Project.HotelPages;

public partial class HotelManagement : UserControl
{
    public HotelManagement()
    {
        InitializeComponent();
    }

    private void ServiceButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadServices();
    }

    private void AssignmentButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadAssignments();
    }

    private void AcceptButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void RejectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
    
    private void LoadServices()
    {
        var results = new ObservableCollection<ServiceEntry>
        {
            new()
            {
                ServiceName = "Room Cleaning",
                ServiceDescription = "Daily room cleaning including towel replacement and trash removal",
                ServicePrice = 0
            },
            new()
            {
                ServiceName = "Spa Package",
                ServiceDescription = "Includes a 60-minute massage, facial, and sauna access",
                ServicePrice = 120
            },
            new()
            {
                ServiceName = "Airport Shuttle",
                ServiceDescription = "One-way transportation to or from the airport",
                ServicePrice = 30
            }
        };

        ServiceItemsControl.ItemsSource = results;
    }
    
    private void LoadAssignments()
    {
        var results = new ObservableCollection<AssignmentEntry>
        {
            new()
            {
                ServiceName = "Room Cleaning",
                RoomNumber = "305",
                DateTime = "2025-06-12 10:00"
            },
            new()
            {
                ServiceName = "Spa Package",
                RoomNumber = "212",
                DateTime = "2025-06-12 14:00"
            },
            new()
            {
                ServiceName = "Airport Shuttle",
                RoomNumber = "101",
                DateTime = "2025-06-13 06:30"
            }
        };

        AssignmentItemsControl.ItemsSource = results;
    }
}

public class ServiceEntry
{
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
    public int ServicePrice { get; set; }
}

public class AssignmentEntry
{
    public string ServiceName { get; set; }
    public string RoomNumber { get; set; }
    public string DateTime { get; set; }
}