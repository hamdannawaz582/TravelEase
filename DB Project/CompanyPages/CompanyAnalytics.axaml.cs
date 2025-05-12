using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DB_Project.Services;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages;

public partial class CompanyAnalytics : UserControl
{
    private readonly string un;

    public CompanyAnalytics(string username)
    {
        InitializeComponent();
        DataContext = this;

        GreeterText.Text = "Welcome Company!";

        TotalBookingChartControl.SetLabel("Booking Over Time");
        RevenueChart.SetLabel("Revenue Over Time");
        un = username;
        LoadSearchResults();
    }

    private void TotalBookingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TotalBookingComboBox.SelectedIndex == 0)
        {
            var query =
                "SELECT DAY(StartDate) as StartDay, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY DAY(StartDate) ORDER BY DAY(StartDate);";

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

                        TotalBookingChartControl.SetChart([
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
        else if (TotalBookingComboBox.SelectedIndex == 1)
        {
            var query =
                "SELECT MONTH(StartDate) as StartMonth, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY MONTH(StartDate) ORDER BY MONTH(StartDate);";
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
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartMonth"]),
                                Convert.ToInt32(reader["C"])));

                        TotalBookingChartControl.SetChart([
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
                "SELECT YEAR(StartDate) as StartYear, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY YEAR(StartDate) ORDER BY YEAR(StartDate);";
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

                        TotalBookingChartControl.SetChart([
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
                "SELECT DAY(StartDate) as StartDay, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY DAY(StartDate) ORDER BY DAY(StartDate);";

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
                "SELECT MONTH(StartDate) as StartMonth, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY MONTH(StartDate) ORDER BY MONTH(StartDate);";
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
                "SELECT YEAR(StartDate) as StartYear, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID WHERE OperatorUsername = @OperatorUsername GROUP BY YEAR(StartDate) ORDER BY YEAR(StartDate);";
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
        var query =
            "SELECT TOP 10 t.Title as Name, r.Stars as Rating, r.Feedback as Content, r.ReviewTime as Date FROM Operator o JOIN Trip t ON o.Username = t.OperatorUsername JOIN TripReview tr ON t.TripID = tr.TripID JOIN Review r ON tr.ReviewID = r.ReviewID WHERE o.Username = @OperatorUsername ORDER BY Date DESC;";
        var results = new ObservableCollection<ReviewSummary>
        {
           
        };

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

public class ReviewSummary
{
    public string Name { get; set; }
    public string Date { get; set; }
    public int Rating { get; set; }
    public string ContentSummary { get; set; }
}