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

public partial class PlatformGrowthReport : UserControl, INotifyPropertyChanged
{
    private CartesianChart _userRegistrationsChart;
    private CartesianChart _activeUsersChart;
    private CartesianChart _partnershipGrowthChart;
    private CartesianChart _regionalExpansionChart;

    public PlatformGrowthReport()
    {
        InitializeComponent();
        DataContext = this;

        _userRegistrationsChart = this.FindControl<CartesianChart>("UserRegistrationsChart");
        _activeUsersChart = this.FindControl<CartesianChart>("ActiveUsersChart");
        _partnershipGrowthChart = this.FindControl<CartesianChart>("PartnershipGrowthChart");
        _regionalExpansionChart = this.FindControl<CartesianChart>("RegionalExpansionChart");

        LoadUserRegistrationsData();
        LoadActiveUsersData();
        LoadPartnershipGrowthData();
        LoadRegionalExpansionData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadUserRegistrationsData()
    {
        try
        {
            string query = @"
                SELECT
                    FORMAT(JoinDate, 'yyyy-MM') AS Month,
                    SUM(CASE WHEN t.Username IS NOT NULL THEN 1 ELSE 0 END) AS TravellerCount,
                    SUM(CASE WHEN o.Username IS NOT NULL THEN 1 ELSE 0 END) AS OperatorCount,
                    SUM(CASE WHEN a.Username IS NOT NULL THEN 1 ELSE 0 END) AS AdminCount
                FROM [User] u
                LEFT JOIN Traveller t ON u.Username = t.Username
                LEFT JOIN Operator o ON u.Username = o.Username
                LEFT JOIN Admin a ON u.Username = a.Username
                GROUP BY FORMAT(JoinDate, 'yyyy-MM')
                ORDER BY Month";

            var months = new List<string>();
            var travellerCounts = new List<double>();
            var operatorCounts = new List<double>();
            var adminCounts = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        double travellerCount = Convert.ToDouble(reader["TravellerCount"]);
                        double operatorCount = Convert.ToDouble(reader["OperatorCount"]);
                        double adminCount = Convert.ToDouble(reader["AdminCount"]);

                        months.Add(month);
                        travellerCounts.Add(travellerCount);
                        operatorCounts.Add(operatorCount);
                        adminCounts.Add(adminCount);
                    }
                }
            }

            if (months.Count == 0)
            {
                // Fallback demo data for the last 6 months
                months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
                travellerCounts = new List<double> { 45, 62, 78, 95, 120, 145 };
                operatorCounts = new List<double> { 5, 8, 10, 12, 15, 18 };
                adminCounts = new List<double> { 1, 2, 2, 3, 3, 4 };
            }

            var series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = travellerCounts.ToArray(),
                    Name = "Travelers",
                    Stroke = new SolidColorPaint(new SKColor(52, 152, 219)) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(52, 152, 219, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(52, 152, 219)),
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = operatorCounts.ToArray(),
                    Name = "Operators",
                    Stroke = new SolidColorPaint(new SKColor(46, 204, 113)) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(46, 204, 113, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(46, 204, 113)),
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = adminCounts.ToArray(),
                    Name = "Admins",
                    Stroke = new SolidColorPaint(new SKColor(231, 76, 60)) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(231, 76, 60, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(231, 76, 60)),
                    GeometrySize = 8
                }
            };

            _userRegistrationsChart.Series = series;
            _userRegistrationsChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _userRegistrationsChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "New Registrations"
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user registration data: {ex.Message}");
        }
    }

    private void LoadActiveUsersData()
    {
        try
        {
            // Since there's no direct "active users" data in the schema,
            // we'll use trip bookings and room bookings to simulate active users
            string query = @"
                SELECT
                    FORMAT(t.StartDate, 'yyyy-MM') AS Month,
                    COUNT(DISTINCT tb.Username) AS ActiveTravellers,
                    COUNT(DISTINCT t.OperatorUsername) AS ActiveOperators
                FROM Trip t
                JOIN Trip_Booking tb ON t.TripID = tb.TripID
                WHERE t.StartDate >= DATEADD(month, -6, GETDATE())
                GROUP BY FORMAT(t.StartDate, 'yyyy-MM')
                ORDER BY Month";

            var months = new List<string>();
            var activeTravellers = new List<double>();
            var activeOperators = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        double travellers = Convert.ToDouble(reader["ActiveTravellers"]);
                        double operators = Convert.ToDouble(reader["ActiveOperators"]);

                        months.Add(month);
                        activeTravellers.Add(travellers);
                        activeOperators.Add(operators);
                    }
                }
            }

            if (months.Count == 0)
            {
                // Fallback demo data
                months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
                activeTravellers = new List<double> { 38, 52, 67, 85, 95, 110 };
                activeOperators = new List<double> { 4, 7, 8, 10, 12, 15 };
            }

            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = activeTravellers.ToArray(),
                    Name = "Active Travelers",
                    Fill = new SolidColorPaint(new SKColor(241, 196, 15)),
                    MaxBarWidth = 30
                },
                new ColumnSeries<double>
                {
                    Values = activeOperators.ToArray(),
                    Name = "Active Operators",
                    Fill = new SolidColorPaint(new SKColor(155, 89, 182)),
                    MaxBarWidth = 30
                }
            };

            _activeUsersChart.Series = series;
            _activeUsersChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _activeUsersChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Active Users Count"
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading active users data: {ex.Message}");
        }
    }

    private void LoadPartnershipGrowthData()
    {
        try
        {
            string query = @"
                SELECT
                    FORMAT(u.JoinDate, 'yyyy-MM') AS Month,
                    COUNT(DISTINCT h.HUsername) AS NewHotels,
                    COUNT(DISTINCT o.Username) AS NewOperators
                FROM [User] u
                LEFT JOIN Hotel h ON u.Username = h.HUsername
                LEFT JOIN Operator o ON u.Username = o.Username
                GROUP BY FORMAT(u.JoinDate, 'yyyy-MM')
                ORDER BY Month";

            var months = new List<string>();
            var newHotels = new List<double>();
            var newOperators = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        double hotels = Convert.ToDouble(reader["NewHotels"]);
                        double operators = Convert.ToDouble(reader["NewOperators"]);

                        months.Add(month);
                        newHotels.Add(hotels);
                        newOperators.Add(operators);
                    }
                }
            }

            if (months.Count == 0)
            {
                // Fallback demo data
                months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
                newHotels = new List<double> { 3, 5, 4, 7, 6, 9 };
                newOperators = new List<double> { 2, 3, 5, 4, 6, 7 };
            }

            var series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = newHotels.ToArray(),
                    Name = "New Hotels",
                    Stroke = new SolidColorPaint(new SKColor(230, 126, 34)) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(230, 126, 34, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(230, 126, 34)),
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = newOperators.ToArray(),
                    Name = "New Operators",
                    Stroke = new SolidColorPaint(new SKColor(52, 73, 94)) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(52, 73, 94, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(52, 73, 94)),
                    GeometrySize = 8
                }
            };

            _partnershipGrowthChart.Series = series;
            _partnershipGrowthChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _partnershipGrowthChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "New Partners"
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading partnership growth data: {ex.Message}");
        }
    }

    private void LoadRegionalExpansionData()
    {
        try
        {
            string query = @"
                SELECT
                    FORMAT(JoinDate, 'yyyy-MM') AS Month,
                    COUNT(DISTINCT Country) AS NewCountries,
                    COUNT(DISTINCT City) AS NewCities
                FROM Destination
                GROUP BY FORMAT(JoinDate, 'yyyy-MM')
                ORDER BY Month";

            var months = new List<string>();
            var newCountries = new List<double>();
            var newCities = new List<double>();
            var cumulativeCountries = new List<double>();
            var cumulativeCities = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    int totalCountries = 0;
                    int totalCities = 0;

                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        double countries = Convert.ToDouble(reader["NewCountries"]);
                        double cities = Convert.ToDouble(reader["NewCities"]);

                        totalCountries += (int)countries;
                        totalCities += (int)cities;

                        months.Add(month);
                        newCountries.Add(countries);
                        newCities.Add(cities);
                        cumulativeCountries.Add(totalCountries);
                        cumulativeCities.Add(totalCities);
                    }
                }
            }

            if (months.Count == 0)
            {
                // Fallback demo data
                months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
                newCountries = new List<double> { 2, 3, 1, 4, 2, 3 };
                newCities = new List<double> { 5, 7, 6, 9, 8, 10 };
                
                // Calculate cumulative values
                int totalCountries = 0;
                int totalCities = 0;
                for (int i = 0; i < newCountries.Count; i++)
                {
                    totalCountries += (int)newCountries[i];
                    totalCities += (int)newCities[i];
                    cumulativeCountries.Add(totalCountries);
                    cumulativeCities.Add(totalCities);
                }
            }

            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = newCities.ToArray(),
                    Name = "New Cities",
                    Fill = new SolidColorPaint(new SKColor(26, 188, 156)),
                    MaxBarWidth = 20
                },
                new ColumnSeries<double>
                {
                    Values = newCountries.ToArray(),
                    Name = "New Countries",
                    Fill = new SolidColorPaint(new SKColor(41, 128, 185)),
                    MaxBarWidth = 20
                },
                new LineSeries<double>
                {
                    Values = cumulativeCities.ToArray(),
                    Name = "Total Cities",
                    Stroke = new SolidColorPaint(new SKColor(22, 160, 133)) { StrokeThickness = 3 },
                    GeometryFill = new SolidColorPaint(new SKColor(22, 160, 133)),
                    GeometrySize = 8,
                    ScalesYAt = 1
                },
                new LineSeries<double>
                {
                    Values = cumulativeCountries.ToArray(),
                    Name = "Total Countries",
                    Stroke = new SolidColorPaint(new SKColor(41, 128, 185)) { StrokeThickness = 3 },
                    GeometryFill = new SolidColorPaint(new SKColor(41, 128, 185)),
                    GeometrySize = 8,
                    ScalesYAt = 1
                }
            };

            _regionalExpansionChart.Series = series;
            _regionalExpansionChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _regionalExpansionChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "New Regions"
                },
                new Axis
                {
                    Name = "Cumulative Total",
                    ShowSeparatorLines = false,
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading regional expansion data: {ex.Message}");
        }
    }
}