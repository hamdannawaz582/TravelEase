using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DB_Project.CompanyPages;
using DB_Project.Services;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.HotelPages;


public partial class HotelAnalytics : UserControl
{
    private readonly string un;
    public HotelAnalytics(string username)
    {
        InitializeComponent();
        DataContext = this;

        GreeterText.Text = "Welcome Hotel!";

        TotalOccupancyChartControl.SetLabel("Occupancy Over Time");
        RevenueChart.SetLabel("Revenue Over Time");
        un = username;
        //LoadSearchResults();
    }

    private void TotalOccupancyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TotalOccupanyComboBox.SelectedIndex == 0)
        {
            var query =
                "SELECT DAY(StartDate) as StartDay, COUNT(RoomID) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY DAY(StartDate);";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@HUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartDay"]),
                                Convert.ToInt32(reader["C"])));

                        TotalOccupancyChartControl.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
        else if (TotalOccupanyComboBox.SelectedIndex == 1)
        {
            var query =
                "SELECT MONTH(StartDate) as StartMonth, COUNT(RoomID) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY MONTH(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@HUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartMonth"]),
                                Convert.ToInt32(reader["C"])));

                        TotalOccupancyChartControl.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
        else
        {
            var query =
                "SELECT YEAR(StartDate) as StartYear, COUNT(RoomID) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY YEAR(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@OperatorUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartYear"]),
                                Convert.ToInt32(reader["C"])));

                        TotalOccupancyChartControl.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
    }

    private void RevenueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (RevenueComboBox.SelectedIndex == 0)
        {
            var query =
                "SELECT DAY(StartDate) as StartDay, SUM(Price) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY DAY(StartDate);";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@OperatorUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartDay"]),
                                Convert.ToInt32(reader["C"])));

                        RevenueChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
        else if (RevenueComboBox.SelectedIndex == 1)
        {
            var query =
                "SELECT MONTH(StartDate) as StartMonth, SUM(Price) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY MONTH(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@HUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartMonth"]),
                                Convert.ToInt32(reader["C"])));

                        RevenueChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
        else
        {
            var query =
                "SELECT YEAR(StartDate) as StartYear, SUM(Price) as C FROM Hotel h JOIN Room r ON h.HUsername = r.HUsername JOIN Room_Booking rb ON rb.RoomID = r.RoomNumber WHERE h.HUsername = @HUsername GROUP BY YEAR(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    command.Parameters.AddWithValue("@HUsername", un);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var series = new ObservableCollection<ObservablePoint>();
                        while (reader.Read())
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartYear"]),
                                Convert.ToInt32(reader["C"])));

                        RevenueChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series
                                }
                            ]
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }
    }

    private void ReviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadSearchResults();
    }
    
    private void LoadSearchResults()
    {
        var results = new ObservableCollection<ReviewSummary>
        {
            // new()
            // {
            //     Name = "Alice Johnson",
            //     Date = "2025-05-01",
            //     Rating = 5,
            //     ContentSummary = "An outstanding experience! Everything exceeded my expectations."
            // },
            // new()
            // {
            //     Name = "Bob Lee",
            //     Date = "2025-04-22",
            //     Rating = 3,
            //     ContentSummary = "It was okay, not great but not terrible either. Some parts could be improved."
            // },
            // new()
            // {
            //     Name = "Catherine Smith",
            //     Date = "2025-03-15",
            //     Rating = 1,
            //     ContentSummary = "Very disappointing. Service was poor and the product didn't match the description."
            // }
        };
        
        var query =
            "SELECT TOP 5 h.Name as Name, r.Stars as Rating, r.Feedback as Content, r.ReviewTime as Date FROM Hotel h JOIN HotelReview hr ON h.HUsername = hr.HUsername JOIN Review r ON hr.ReviewID = r.ReviewID WHERE h.HUsername = @HUsername ORDER BY Date DESC;";

        try
        {
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                command.Parameters.AddWithValue("@HUsername", un);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        results.Add(new ReviewSummary
                        {
                            Name = reader["Name"].ToString(),
                            Rating = Convert.ToInt32(reader["Rating"]),
                            ContentSummary = reader["Content"].ToString(),
                            Date = reader["Date"].ToString()
                        });
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error loading: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading: {ex.Message}");
        }

        SearchResultsControl.ItemsSource = results;
    }
}

