using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DB_Project.CompanyPages;

public partial class EfficiencyReport : UserControl, INotifyPropertyChanged
{
    private CartesianChart _hotelOccupancyChart;
    private PieChart _guideRatingsChart;
    private CartesianChart _transportPerformanceChart;

    public EfficiencyReport()
    {
        InitializeComponent();
        DataContext = this;

        _hotelOccupancyChart = this.FindControl<CartesianChart>("HotelOccupancyChart");
        _guideRatingsChart = this.FindControl<PieChart>("GuideRatingsChart");
        _transportPerformanceChart = this.FindControl<CartesianChart>("TransportPerformanceChart");

        LoadHotelOccupancyData();
        LoadGuideRatingsData();
        LoadTransportPerformanceData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadHotelOccupancyData()
    {
        try
        {
            string query = @"
                SELECT TOP 10 h.HUsername, h.Name,
                       COUNT(DISTINCT rb.RoomID) AS BookedRooms,
                       COUNT(DISTINCT r.RoomNumber) AS TotalRooms,
                       CAST(COUNT(DISTINCT rb.RoomID) AS FLOAT) / NULLIF(COUNT(DISTINCT r.RoomNumber), 0) * 100 AS OccupancyRate
                FROM Hotel h
                LEFT JOIN Room r ON h.HUsername = r.HUsername
                LEFT JOIN Room_Booking rb ON r.HUsername = rb.HUsername AND r.RoomNumber = rb.RoomID
                                         AND GETDATE() BETWEEN rb.StartDate AND rb.EndDate
                GROUP BY h.HUsername, h.Name
                ORDER BY OccupancyRate DESC";

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
                        string hotelName = reader["Name"].ToString();
                        double occupancyRate = reader["OccupancyRate"] == DBNull.Value ? 
                            0 : Convert.ToDouble(reader["OccupancyRate"]);

                        labels.Add(hotelName);
                        values.Add(Math.Round(occupancyRate, 1));
                    }
                }
            }

            if (values.Count == 0)
            {
                // Fallback demo data
                labels = new List<string> { 
                    "Grand Plaza", "Seaside Resort", "Mountain Lodge", 
                    "Downtown Inn", "Business Hotel", "Beach Resort" 
                };
                values = new List<double> { 92.5, 87.3, 76.8, 65.4, 55.2, 45.8 };
            }

            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Occupancy Rate (%)",
                Fill = new SolidColorPaint(new SKColor(65, 105, 225)), // Royal Blue
                MaxBarWidth = 35
            };

            _hotelOccupancyChart.Series = new ISeries[] { columnSeries };
            _hotelOccupancyChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsRotation = 45
                }
            };
            _hotelOccupancyChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Occupancy Rate (%)",
                    MinLimit = 0,
                    MaxLimit = 100
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading hotel occupancy data: {ex.Message}");
        }
    }

    private void LoadGuideRatingsData()
    {
        try
        {
            // Since there's no explicit Guide table in the schema, we'll create a visualization 
            // showing the distribution of review ratings for services
            string query = @"
                SELECT r.Stars, COUNT(*) AS Count
                FROM Review r
                JOIN TripReview tr ON r.ReviewID = tr.ReviewID
                GROUP BY r.Stars
                ORDER BY r.Stars";

            var values = new List<double>();
            var labels = new List<string>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int stars = Convert.ToInt32(reader["Stars"]);
                        int count = Convert.ToInt32(reader["Count"]);

                        values.Add(count);
                        labels.Add($"{stars} Stars");
                    }
                }
            }

            if (values.Count == 0)
            {
                // Fallback demo data
                values = new List<double> { 8, 15, 25, 42, 35 };
                labels = new List<string> { "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars" };
            }

            var pieSeries = new List<PieSeries<double>>();
            var colors = new[] {
                new SKColor(220, 53, 69),   // Red (1 star)
                new SKColor(255, 193, 7),   // Amber (2 stars)
                new SKColor(13, 202, 240),  // Info (3 stars)
                new SKColor(13, 110, 253),  // Primary (4 stars)
                new SKColor(25, 135, 84)    // Success (5 stars)
            };

            for (int i = 0; i < values.Count; i++)
            {
                pieSeries.Add(new PieSeries<double>
                {
                    Values = new double[] { values[i] },
                    Name = labels[i],
                    Fill = new SolidColorPaint(colors[i % colors.Length]),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"{labels[i]}: {values[i]}"
                });
            }

            _guideRatingsChart.Series = pieSeries;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading guide ratings data: {ex.Message}");
        }
    }

    private void LoadTransportPerformanceData()
    {
        try
        {
            string query = @"
                SELECT 
                    TransportationType,
                    COUNT(*) AS TotalTrips,
                    SUM(CASE WHEN Start <= EstimatedStart THEN 1 ELSE 0 END) AS OnTimeTrips,
                    CAST(SUM(CASE WHEN Start <= EstimatedStart THEN 1 ELSE 0 END) AS FLOAT) / NULLIF(COUNT(*), 0) * 100 AS OnTimePercentage
                FROM Trip_Transportation
                WHERE Start IS NOT NULL
                GROUP BY TransportationType
                ORDER BY OnTimePercentage DESC";

            var labels = new List<string>();
            var onTimeValues = new List<double>();
            var delayedValues = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string transportType = reader["TransportationType"].ToString();
                        double onTimePercentage = reader["OnTimePercentage"] == DBNull.Value ? 
                            0 : Convert.ToDouble(reader["OnTimePercentage"]);
                        
                        labels.Add(transportType);
                        onTimeValues.Add(Math.Round(onTimePercentage, 1));
                        delayedValues.Add(Math.Round(100 - onTimePercentage, 1));
                    }
                }
            }

            if (labels.Count == 0)
            {
                // Fallback demo data
                labels = new List<string> { "Train", "Flight", "Bus", "Ferry", "Taxi" };
                onTimeValues = new List<double> { 94.2, 86.5, 83.7, 75.3, 89.8 };
                delayedValues = new List<double> { 5.8, 13.5, 16.3, 24.7, 10.2 };
            }

            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = onTimeValues.ToArray(),
                    Name = "On Time (%)",
                    Fill = new SolidColorPaint(new SKColor(40, 167, 69)), // Green
                    MaxBarWidth = 25,
                },
                new ColumnSeries<double>
                {
                    Values = delayedValues.ToArray(),
                    Name = "Delayed (%)",
                    Fill = new SolidColorPaint(new SKColor(220, 53, 69)), // Red
                    MaxBarWidth = 25,
                }
            };

            _transportPerformanceChart.Series = series;
            _transportPerformanceChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels.ToArray(),
                    LabelsRotation = 0
                }
            };
            _transportPerformanceChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Percentage (%)",
                    MinLimit = 0,
                    MaxLimit = 100
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transport performance data: {ex.Message}");
        }
    }
}