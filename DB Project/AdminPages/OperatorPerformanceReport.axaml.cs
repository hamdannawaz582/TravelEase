using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DB_Project.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Data.SqlClient;
using SkiaSharp;

namespace DB_Project.AdminPages;

public partial class OperatorPerformanceReport : UserControl, INotifyPropertyChanged
{
    private CartesianChart RatingsChart;
    private CartesianChart RevenueChart;
    private CartesianChart TimeChart;
    
    public OperatorPerformanceReport()
    {
        InitializeComponent();
        DataContext = this;
        
        RatingsChart = this.FindControl<CartesianChart>("OperatorRatingsChart");
        RevenueChart = this.FindControl<CartesianChart>("OperatorRevenueChart");
        TimeChart = this.FindControl<CartesianChart>("ResponseTimeChart");
        
        LoadOperatorRatings();
        LoadOperatorRevenue();
        LoadResponseTimes();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void LoadOperatorRatings()
    {
        try
        {
            string query = @"
                SELECT TOP 10 o.Username, AVG(CAST(r.Stars AS FLOAT)) AS AvgRating
                FROM Operator o
                JOIN Trip t ON o.Username = t.OperatorUsername
                JOIN TripReview tr ON t.TripID = tr.TripID
                JOIN Review r ON tr.ReviewID = r.ReviewID
                GROUP BY o.Username
                ORDER BY AvgRating DESC";
            
            var labels = new List<string>();
            var values = new List<double>();
            
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string username = reader["Username"].ToString();
                        double rating = Convert.ToDouble(reader["AvgRating"]);
                        
                        labels.Add(username);
                        values.Add(Math.Round(rating, 2));
                    }
                }
            }
            
            if (values.Count == 0)
            {
                // Fallback demo data
                labels = new List<string> { "TourMaster", "AdventurePro", "CultureGuide", "EcoTours", "LuxuryTravel", "CityExplorer" };
                values = new List<double> { 4.8, 4.5, 4.3, 4.2, 4.0, 3.8 };
            }
            
            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Average Rating",
                Fill = new SolidColorPaint(new SKColor(54, 162, 235)),
                MaxBarWidth = 30
            };
            
            RatingsChart.Series = new ISeries[] { columnSeries };
            RatingsChart.XAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Labels = labels.ToArray(),
                    LabelsRotation = 45
                } 
            };
            RatingsChart.YAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Name = "Rating (1-5 stars)",
                    MinLimit = 0,
                    MaxLimit = 5
                } 
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading operator ratings: {ex.Message}");
        }
    }
    
    private void LoadOperatorRevenue()
    {
        try
        {
            string query = @"
                SELECT TOP 10 o.Username, SUM(t.PriceRange) AS TotalRevenue
                FROM Operator o
                JOIN Trip t ON o.Username = t.OperatorUsername
                JOIN Trip_Booking tb ON t.TripID = tb.TripID
                GROUP BY o.Username
                ORDER BY TotalRevenue DESC";
            
            var labels = new List<string>();
            var values = new List<double>();
            
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string username = reader["Username"].ToString();
                        double revenue = Convert.ToDouble(reader["TotalRevenue"]);
                        
                        labels.Add(username);
                        values.Add(revenue);
                    }
                }
            }
            
            if (values.Count == 0)
            {
                // Fallback demo data
                labels = new List<string> { "LuxuryTravel", "AdventurePro", "TourMaster", "CityExplorer", "CultureGuide", "EcoTours" };
                values = new List<double> { 45000, 37500, 32000, 29800, 25400, 21000 };
            }
            
            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Revenue ($)",
                Fill = new SolidColorPaint(new SKColor(75, 192, 192)),
                MaxBarWidth = 30
            };
            
            RevenueChart.Series = new ISeries[] { columnSeries };
            RevenueChart.XAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Labels = labels.ToArray(),
                    LabelsRotation = 45
                } 
            };
            RevenueChart.YAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Name = "Total Revenue ($)"
                } 
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading operator revenue: {ex.Message}");
        }
    }
    
    private void LoadResponseTimes()
    {
        try
        {
            string query = @"
                SELECT TOP 10 o.Username, 
                       AVG(DATEDIFF(HOUR, r.ReviewTime, r.ResponseTime)) AS AvgResponseHours
                FROM Operator o
                JOIN Trip t ON o.Username = t.OperatorUsername
                JOIN TripReview tr ON t.TripID = tr.TripID
                JOIN Review r ON tr.ReviewID = r.ReviewID
                WHERE r.ResponseTime IS NOT NULL
                GROUP BY o.Username
                ORDER BY AvgResponseHours";
            
            var labels = new List<string>();
            var values = new List<double>();
            
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string username = reader["Username"].ToString();
                        double responseTime = Convert.ToDouble(reader["AvgResponseHours"]);
                        
                        labels.Add(username);
                        values.Add(Math.Round(responseTime, 1));
                    }
                }
            }
            
            if (values.Count == 0)
            {
                // Fallback demo data
                labels = new List<string> { "TourMaster", "CityExplorer", "LuxuryTravel", "CultureGuide", "EcoTours", "AdventurePro" };
                values = new List<double> { 3.2, 5.7, 8.4, 12.6, 15.1, 24.5 };
            }
            
            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Response Time (Hours)",
                Fill = new SolidColorPaint(new SKColor(255, 159, 64)),
                MaxBarWidth = 40
            };
            
            TimeChart.Series = new ISeries[] { columnSeries };
            TimeChart.XAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Labels = labels.ToArray(),
                    LabelsRotation = 45
                } 
            };
            TimeChart.YAxes = new Axis[] 
            { 
                new Axis 
                { 
                    Name = "Hours to Respond"
                } 
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading response times: {ex.Message}");
        }
    }
}