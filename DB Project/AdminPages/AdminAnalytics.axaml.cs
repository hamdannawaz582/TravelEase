using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DB_Project.AdminPages;

public partial class AdminAnalytics : UserControl
{
    public AdminAnalytics()
    {
        InitializeComponent();
        DataContext = this;
       
        Sidebar.SetSplitView(SplitView);
        Sidebar.SetPageHost(MainContent);
        Sidebar.AddTab("Home", new LoginPage());
        Sidebar.AddTab("Settings", new UserControl());
        Sidebar.AddTab("About", new UserControl());

        GreeterText.Text = "Welcome Admin!";
        
        UserTrafficChart.SetLabel("User Traffic Over Time");
        RevenueChart.SetLabel("Revenue Over Time");
        BookingChart.SetLabel("Booking Over Time");
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UserTrafficButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Check ComboBox value and set chart data accordingly
    }

    private void RevenueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Check ComboBox value and set chart data accordingly
    }

    private void BookingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Check ComboBox value and set chart data accordingly
    }
}