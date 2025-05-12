using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DB_Project.CompanyPages;

public partial class CompanyCreate : UserControl
{
    public CompanyCreate()
    {
        InitializeComponent();
    }

    private void ItineraryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: Make a new entry based on the selected values
        LoadItinerary();
    }

    private void LoadItinerary()
    {
        var results = new ObservableCollection<ItineraryEntry>
        {
            new()
            {
                DateTime = "2025-06-10 08:30",
                Action = "Flight departure from JFK Airport to Tokyo"
            },
            new()
            {
                DateTime = "2025-06-11 15:00",
                Action = "Check-in at Shinjuku Grand Hotel"
            },
            new()
            {
                DateTime = "2025-06-12 09:00",
                Action = "Visit Meiji Shrine and Yoyogi Park"
            }
        };

        ItineraryItemsControl.ItemsSource = results;
    }

    private void InclusionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadInclusions();
    }

    private void LoadInclusions()
    {
        var results = new ObservableCollection<InclusionsEntry>
        {
            new()
            {
                DateTime = "2025-06-10 08:00",
                Action = "Airport transfer from home to JFK included"
            },
            new()
            {
                DateTime = "2025-06-11 07:00",
                Action = "Complimentary breakfast at hotel"
            },
            new()
            {
                DateTime = "2025-06-12 10:00",
                Action = "Guided city tour with English-speaking guide"
            }
        };

        InclusionsItemsControl.ItemsSource = results;
    }

    private void SubmitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}

public class ItineraryEntry
{
    public string DateTime { get; set; }
    public string Action { get; set; }
}

public class InclusionsEntry
{
    public string DateTime { get; set; }
    public string Action { get; set; }
}