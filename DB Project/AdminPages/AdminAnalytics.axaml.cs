using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DB_Project.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.AdminPages;

public partial class AdminAnalytics : UserControl
{
    public AdminAnalytics()
    {
        InitializeComponent();
        DataContext = this;

        GreeterText.Text = "Welcome Admin!";

        UserTrafficChart.SetLabel("User Traffic Over Time");
        RevenueChart.SetLabel("Revenue Over Time");
        BookingChart.SetLabel("Booking Over Time");
        UserTrafficChart.SetChart([
        new LineSeries<ObservablePoint>
        {
            Values = [new ObservablePoint(0, 0)],
        },
        ]);
        
        RevenueChart.SetChart([
            new LineSeries<ObservablePoint>
            {
                Values = [new ObservablePoint(0, 0)],
            },
        ]);
        BookingChart.SetChart([
            new LineSeries<ObservablePoint>
            {
                Values = [new ObservablePoint(0, 0)],
            },
        ]);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UserTrafficButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //check ComboBox value and set chart data accordingly
        if (UserTrafficComboBox.SelectedIndex == 0)
        {
            string query = "SELECT DAY(JoinDate) AS D, COUNT(Username) AS C FROM [User] GROUP BY DAY(JoinDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["D"]), Convert.ToInt32(reader["C"])));
                        }

                        UserTrafficChart.SetChart([
                            new LineSeries<ObservablePoint>
                            {
                                Values = series,
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
        } else if (UserTrafficComboBox.SelectedIndex == 1)
        {
            string query = "SELECT MONTH(JoinDate) AS M, COUNT(Username) AS C FROM [User] GROUP BY MONTH(JoinDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["M"]), Convert.ToInt32(reader["C"])));
                        }

                        UserTrafficChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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
            string query = "SELECT YEAR(JoinDate) AS Y, COUNT(Username) AS C FROM [User] GROUP BY YEAR(JoinDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["Y"]), Convert.ToInt32(reader["C"])));
                        }

                        UserTrafficChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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
        //check ComboBox value and set chart data accordingly
        if (RevenueComboBox.SelectedIndex == 0)
        {
            string query = "SELECT DAY(StartDate) as StartDay, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username JOIN Trip t ON t.TripID = tb.TripID GROUP BY DAY(StartDate) ORDER BY DAY(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartDay"]), Convert.ToInt32(reader["C"])));
                        }

                        RevenueChart.SetChart([
                            new LineSeries<ObservablePoint>
                            {
                                Values = series,
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
        } else if (RevenueComboBox.SelectedIndex == 1)
        {
            string query = "SELECT MONTH(StartDate) as StartMonth, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username JOIN Trip t ON t.TripID = tb.TripID GROUP BY MONTH(StartDate) ORDER BY MONTH(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartMonth"]), Convert.ToInt32(reader["C"])));
                        }

                        RevenueChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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
            string query = "SELECT YEAR(StartDate) as StartYear, SUM(t.PriceRange) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username JOIN Trip t ON t.TripID = tb.TripID GROUP BY YEAR(StartDate) ORDER BY YEAR(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartYear"]), Convert.ToInt32(reader["C"])));
                        }

                        RevenueChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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

    private void BookingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //check ComboBox value and set chart data accordingly
        if (BookingComboBox.SelectedIndex == 0)
        {
            string query = "SELECT DAY(StartDate) as StartDay, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID GROUP BY DAY(StartDate) ORDER BY DAY(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartDay"]), Convert.ToInt32(reader["C"])));
                        }

                        BookingChart.SetChart([
                            new LineSeries<ObservablePoint>
                            {
                                Values = series,
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
        } else if (BookingComboBox.SelectedIndex == 1)
        {
            string query = "SELECT MONTH(StartDate) as StartMonth, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID GROUP BY MONTH(StartDate) ORDER BY MONTH(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartMonth"]), Convert.ToInt32(reader["C"])));
                        }

                        BookingChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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
            string query = "SELECT YEAR(StartDate) as StartYear, COUNT(u.Username) as C FROM [User] u JOIN Trip_Booking tb ON tb.Username = u.Username RIGHT JOIN Trip t ON t.TripID = tb.TripID GROUP BY YEAR(StartDate) ORDER BY YEAR(StartDate);";
            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) { Console.Write("Failed");}
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        ObservableCollection<ObservablePoint> series = new ObservableCollection<ObservablePoint>()
                        {
                            
                        };
                        while (reader.Read())
                        {
                            series.Add(new ObservablePoint(Convert.ToInt32(reader["StartYear"]), Convert.ToInt32(reader["C"])));
                        }

                        BookingChart.SetChart([
                                new LineSeries<ObservablePoint>
                                {
                                    Values = series,
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
}