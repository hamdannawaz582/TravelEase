using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DB_Project.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages;

public partial class TripBookingReport : UserControl
{
    public TripBookingReport(string operatorUsername)
    {
        InitializeComponent();
        LoadReport(operatorUsername);
    }

    private void LoadReport(string operatorUsername)
    {
        // Fetch data for Total Bookings
        var totalBookingsData = GetTotalBookings(operatorUsername);
        TotalBookingsChart.Series = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = totalBookingsData.Values.ToArray(),
                Name = "Total Bookings"
            }
        };
        TotalBookingsChart.XAxes = new[] { new Axis { Labels = totalBookingsData.Keys.ToArray() } };

        // Fetch data for Revenue by Category
        var revenueByCategoryData = GetRevenueByCategory(operatorUsername);
        RevenueByCategoryChart.Series = revenueByCategoryData.Select(kvp => new PieSeries<decimal>
        {
            Values = new[] { kvp.Value },
            Name = kvp.Key
        }).ToArray();

        // Fetch data for Cancellation Rate
        var cancellationRateData = GetCancellationRate(operatorUsername);
        CancellationRateChart.Series = new ISeries[]
        {
            new PieSeries<double> { Values = new[] { cancellationRateData.Confirmed }, Name = "Confirmed" },
            new PieSeries<double> { Values = new[] { cancellationRateData.Canceled }, Name = "Canceled" }
        };

        // Fetch data for Peak Booking Periods
        var peakBookingData = GetPeakBookingPeriods(operatorUsername);
        PeakBookingPeriodsChart.Series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = peakBookingData.Values.ToArray(),
                Name = "Bookings"
            }
        };
        PeakBookingPeriodsChart.XAxes = new[] { new Axis { Labels = peakBookingData.Keys.ToArray() } };
        
        var averageBookingValue = GetAverageBookingValue(operatorUsername);

        // Display Average Booking Value
        AverageBookingValueTextBlock.Text = $"Average Booking Value: {averageBookingValue:C}";

    }

    private Dictionary<string, int> GetTotalBookings(string operatorUsername)
    {
        var data = new Dictionary<string, int>();
        string query = @"
            SELECT t.Title, COUNT(tb.Username) AS TotalBookings
            FROM Trip_Booking tb
            JOIN Trip t ON tb.TripID = t.TripID
            WHERE t.OperatorUsername = @OperatorUsername
            GROUP BY t.Title";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    data[reader["Title"].ToString()] = Convert.ToInt32(reader["TotalBookings"]);
                }
            }
        }
        return data;
    }

    private Dictionary<string, decimal> GetRevenueByCategory(string operatorUsername)
    {
        var data = new Dictionary<string, decimal>();
        string query = @"
            SELECT t.Type, SUM(t.PriceRange) AS Revenue
            FROM Trip_Booking tb
            JOIN Trip t ON tb.TripID = t.TripID
            WHERE t.OperatorUsername = @OperatorUsername
            GROUP BY t.Type";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    data[reader["Type"].ToString()] = Convert.ToDecimal(reader["Revenue"]);
                }
            }
        }
        return data;
    }

    private (double Confirmed, double Canceled) GetCancellationRate(string operatorUsername)
    {
        double confirmed = 0, canceled = 0;
        string query = @"
            SELECT CancelStatus, COUNT(*) AS Count
        FROM Trip_Booking tb
        JOIN Trip t ON tb.TripID = t.TripID
        WHERE t.OperatorUsername = @OperatorUsername
        GROUP BY CancelStatus";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["CancelStatus"] == DBNull.Value)
                        confirmed = Convert.ToDouble(reader["Count"]);
                    else 
                        canceled = Convert.ToDouble(reader["Count"]);
                }
            }
        }
        return (confirmed, canceled);
    }

    private decimal GetAverageBookingValue(string operatorUsername)
    {
        decimal averageBookingValue = 0;
        string query = @"
        SELECT AVG(t.PriceRange) AS AverageBookingValue
        FROM Trip_Booking tb
        JOIN Trip t ON tb.TripID = t.TripID
        WHERE t.OperatorUsername = @OperatorUsername";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read() && reader["AverageBookingValue"] != DBNull.Value)
                {
                    averageBookingValue = Convert.ToDecimal(reader["AverageBookingValue"]);
                }
            }
        }
        return averageBookingValue;
    }
    private Dictionary<string, int> GetPeakBookingPeriods(string operatorUsername)
    {
        var data = new Dictionary<string, int>();
        string query = @"
            SELECT FORMAT(t.StartDate, 'yyyy-MM') AS Month, COUNT(*) AS TotalBookings
            FROM Trip_Booking tb
            JOIN Trip t ON tb.TripID = t.TripID
            WHERE t.OperatorUsername = 'user001'
            GROUP BY FORMAT(t.StartDate, 'yyyy-MM')";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    data[reader["Month"].ToString()] = Convert.ToInt32(reader["TotalBookings"]);
                }
            }
        }
        return data;
    }
}