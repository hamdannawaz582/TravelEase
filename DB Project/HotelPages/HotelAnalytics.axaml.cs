using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DB_Project.CompanyPages;

namespace DB_Project.HotelPages;


public partial class HotelAnalytics : UserControl
{
    public HotelAnalytics()
    {
        InitializeComponent();
        DataContext = this;

        GreeterText.Text = "Welcome Hotel!";

        TotalOccupancyChartControl.SetLabel("Occupancy Over Time");
        RevenueChart.SetLabel("Revenue Over Time");
        LoadSearchResults();
    }

    private void TotalOccupancyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void RevenueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void ReviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadSearchResults();
    }
    
    private void LoadSearchResults()
    {
        var results = new ObservableCollection<ReviewSummary>
        {
            new()
            {
                Name = "Alice Johnson",
                Date = "2025-05-01",
                Rating = 5,
                ContentSummary = "An outstanding experience! Everything exceeded my expectations."
            },
            new()
            {
                Name = "Bob Lee",
                Date = "2025-04-22",
                Rating = 3,
                ContentSummary = "It was okay, not great but not terrible either. Some parts could be improved."
            },
            new()
            {
                Name = "Catherine Smith",
                Date = "2025-03-15",
                Rating = 1,
                ContentSummary = "Very disappointing. Service was poor and the product didn't match the description."
            }
        };

        SearchResultsControl.ItemsSource = results;
    }
}

