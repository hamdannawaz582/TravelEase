using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DB_Project.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Data.SqlClient;
using SkiaSharp;

namespace DB_Project.AdminPages;

public partial class TravellerDemographicsReport : UserControl, INotifyPropertyChanged
{
    private PieChart _ageDistributionChart;
    private PieChart _nationalityDistributionChart;
    private CartesianChart _tripTypeChart;
    private CartesianChart _budgetByNationalityChart;

    public TravellerDemographicsReport()
    {
        InitializeComponent();
        DataContext = this;

        _ageDistributionChart = this.FindControl<PieChart>("AgeDistributionChart");
        _nationalityDistributionChart = this.FindControl<PieChart>("NationalityDistributionChart");
        _tripTypeChart = this.FindControl<CartesianChart>("TripTypeChart");
        _budgetByNationalityChart = this.FindControl<CartesianChart>("BudgetByNationalityChart");

        LoadAgeDistribution();
        LoadNationalityDistribution();
        LoadPopularTripTypes();
        LoadAverageBudgetByNationality();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadAgeDistribution()
    {
        try
        {
            string query = @"
                SELECT
                    CASE
                        WHEN Age < 20 THEN '< 20'
                        WHEN Age BETWEEN 20 AND 30 THEN '20-30'
                        WHEN Age BETWEEN 31 AND 40 THEN '31-40'
                        WHEN Age BETWEEN 41 AND 50 THEN '41-50'
                        WHEN Age > 50 THEN '50+'
                    END AS AgeGroup,
                    COUNT(*) AS Count
                FROM Traveller
                GROUP BY
                    CASE
                        WHEN Age < 20 THEN '< 20'
                        WHEN Age BETWEEN 20 AND 30 THEN '20-30'
                        WHEN Age BETWEEN 31 AND 40 THEN '31-40'
                        WHEN Age BETWEEN 41 AND 50 THEN '41-50'
                        WHEN Age > 50 THEN '50+'
                    END
                ORDER BY AgeGroup";

            var ageSeries = new List<PieSeries<double>>();
            var colors = new[] {
                new SKColor(0, 191, 255),   // Deep Sky Blue
                new SKColor(30, 144, 255),  // Dodger Blue
                new SKColor(0, 0, 205),     // Medium Blue
                new SKColor(0, 0, 139),     // Dark Blue
                new SKColor(25, 25, 112)    // Midnight Blue
            };

            int colorIndex = 0;

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ageGroup = reader["AgeGroup"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);

                        var color = colors[colorIndex % colors.Length];
                        colorIndex++;

                        ageSeries.Add(new PieSeries<double>
                        {
                            Values = new double[] { count },
                            Name = ageGroup,
                            Fill = new SolidColorPaint(color),
                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                            DataLabelsFormatter = point => $"{ageGroup}: {count}"
                        });
                    }
                }
            }

            if (ageSeries.Count == 0)
            {
                // Fallback demo data
                ageSeries.Add(new PieSeries<double> { Values = new double[] { 15 }, Name = "< 20", Fill = new SolidColorPaint(colors[0]) });
                ageSeries.Add(new PieSeries<double> { Values = new double[] { 42 }, Name = "20-30", Fill = new SolidColorPaint(colors[1]) });
                ageSeries.Add(new PieSeries<double> { Values = new double[] { 28 }, Name = "31-40", Fill = new SolidColorPaint(colors[2]) });
                ageSeries.Add(new PieSeries<double> { Values = new double[] { 20 }, Name = "41-50", Fill = new SolidColorPaint(colors[3]) });
                ageSeries.Add(new PieSeries<double> { Values = new double[] { 12 }, Name = "50+", Fill = new SolidColorPaint(colors[4]) });
            }

            _ageDistributionChart.Series = ageSeries;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading age distribution: {ex.Message}");
        }
    }

    private void LoadNationalityDistribution()
    {
        try
        {
            string query = @"
                SELECT TOP 5 Nationality, COUNT(*) AS Count
                FROM Traveller
                GROUP BY Nationality
                ORDER BY Count DESC";

            var nationalitySeries = new List<PieSeries<double>>();
            var colors = new[] {
                new SKColor(255, 99, 132),  // Red
                new SKColor(255, 159, 64),  // Orange
                new SKColor(255, 205, 86),  // Yellow
                new SKColor(75, 192, 192),  // Green
                new SKColor(54, 162, 235)   // Blue
            };

            int colorIndex = 0;

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nationality = reader["Nationality"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);

                        var color = colors[colorIndex % colors.Length];
                        colorIndex++;

                        nationalitySeries.Add(new PieSeries<double>
                        {
                            Values = new double[] { count },
                            Name = nationality,
                            Fill = new SolidColorPaint(color),
                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                            DataLabelsFormatter = point => $"{nationality}: {count}"
                        });
                    }
                }
            }

            if (nationalitySeries.Count == 0)
            {
                // Fallback demo data
                nationalitySeries.Add(new PieSeries<double> { Values = new double[] { 32 }, Name = "USA", Fill = new SolidColorPaint(colors[0]) });
                nationalitySeries.Add(new PieSeries<double> { Values = new double[] { 24 }, Name = "UK", Fill = new SolidColorPaint(colors[1]) });
                nationalitySeries.Add(new PieSeries<double> { Values = new double[] { 18 }, Name = "Germany", Fill = new SolidColorPaint(colors[2]) });
                nationalitySeries.Add(new PieSeries<double> { Values = new double[] { 14 }, Name = "France", Fill = new SolidColorPaint(colors[3]) });
                nationalitySeries.Add(new PieSeries<double> { Values = new double[] { 10 }, Name = "Japan", Fill = new SolidColorPaint(colors[4]) });
            }

            _nationalityDistributionChart.Series = nationalitySeries;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading nationality distribution: {ex.Message}");
        }
    }

    private void LoadPopularTripTypes()
    {
        try
        {
            string query = @"
                SELECT TOP 5 t.Type, COUNT(*) AS BookingCount
                FROM Trip t
                JOIN Trip_Booking tb ON t.TripID = tb.TripID
                GROUP BY t.Type
                ORDER BY BookingCount DESC";

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
                        string tripType = reader["Type"].ToString();
                        int count = Convert.ToInt32(reader["BookingCount"]);

                        values.Add(count);
                        labels.Add(tripType);
                    }
                }
            }

            if (values.Count == 0)
            {
                // Fallback demo data
                values = new List<double> { 45, 32, 27, 20, 18 };
                labels = new List<string> { "Adventure", "Cultural", "Beach", "City Tour", "Hiking" };
            }

            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Trip Types",
                Fill = new SolidColorPaint(SKColors.RoyalBlue)
            };

            _tripTypeChart.Series = new ISeries[] { columnSeries };
            _tripTypeChart.XAxes = new Axis[] { new Axis { Labels = labels.ToArray(), LabelsRotation = 45 } };
            _tripTypeChart.YAxes = new Axis[] { new Axis { Name = "Booking Count" } };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading popular trip types: {ex.Message}");
        }
    }

    private void LoadAverageBudgetByNationality()
    {
        try
        {
            string query = @"
                SELECT TOP 5 Nationality, AVG(Budget) AS AvgBudget
                FROM Traveller
                GROUP BY Nationality
                ORDER BY AvgBudget DESC";

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
                        string nationality = reader["Nationality"].ToString();
                        double avgBudget = Convert.ToDouble(reader["AvgBudget"]);

                        values.Add(avgBudget);
                        labels.Add(nationality);
                    }
                }
            }

            if (values.Count == 0)
            {
                // Fallback demo data
                values = new List<double> { 5000, 4500, 4000, 3800, 3500 };
                labels = new List<string> { "Switzerland", "USA", "UAE", "Canada", "Australia" };
            }

            var columnSeries = new ColumnSeries<double>
            {
                Values = values.ToArray(),
                Name = "Average Budget",
                Fill = new SolidColorPaint(SKColors.ForestGreen)
            };

            _budgetByNationalityChart.Series = new ISeries[] { columnSeries };
            _budgetByNationalityChart.XAxes = new Axis[] { new Axis { Labels = labels.ToArray(), LabelsRotation = 45 } };
            _budgetByNationalityChart.YAxes = new Axis[] { new Axis { Name = "Average Budget ($)" } };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading budget by nationality: {ex.Message}");
        }
    }
}