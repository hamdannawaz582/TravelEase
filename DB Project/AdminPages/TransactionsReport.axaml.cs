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

public partial class TransactionsReport : UserControl, INotifyPropertyChanged
{
    private PieChart _paymentSuccessChart;
    private CartesianChart _transactionVolumeChart;
    private CartesianChart _chargebackRateChart;
    private PieChart _paymentMethodChart;

    public TransactionsReport()
    {
        InitializeComponent();
        DataContext = this;

        _paymentSuccessChart = this.FindControl<PieChart>("PaymentSuccessChart");
        _transactionVolumeChart = this.FindControl<CartesianChart>("TransactionVolumeChart");
        _chargebackRateChart = this.FindControl<CartesianChart>("ChargebackRateChart");
        _paymentMethodChart = this.FindControl<PieChart>("PaymentMethodChart");

        LoadPaymentSuccessData();
        LoadTransactionVolumeData();
        LoadChargebackRateData();
        LoadPaymentMethodData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadPaymentSuccessData()
    {
        try
        {
            // Since there's no direct payment table, we'll use booking status as a proxy
            // for payment success/failure
            string query = @"
                SELECT 
                    Status, 
                    COUNT(*) as Count
                FROM Trip_Booking
                GROUP BY Status";

            var statusCounts = new Dictionary<string, int>
            {
                { "Approved", 0 },
                { "Pending", 0 },
                { "Rejected", 0 }
            };

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string status = reader["Status"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);
                        if (statusCounts.ContainsKey(status))
                        {
                            statusCounts[status] = count;
                        }
                    }
                }
            }

            bool hasData = statusCounts.Values.Sum() > 0;

            if (!hasData)
            {
                // Fallback demo data
                statusCounts["Approved"] = 85;
                statusCounts["Pending"] = 10;
                statusCounts["Rejected"] = 5;
            }

            var series = new List<PieSeries<double>>();
            
            series.Add(new PieSeries<double>
            {
                Values = new double[] { statusCounts["Approved"] },
                Name = "Successful Payments",
                Fill = new SolidColorPaint(new SKColor(40, 167, 69)), // Green
                InnerRadius = 50
            });
            
            series.Add(new PieSeries<double>
            {
                Values = new double[] { statusCounts["Pending"] },
                Name = "Pending Payments",
                Fill = new SolidColorPaint(new SKColor(255, 193, 7)), // Yellow
                InnerRadius = 50
            });
            
            series.Add(new PieSeries<double>
            {
                Values = new double[] { statusCounts["Rejected"] },
                Name = "Failed Payments",
                Fill = new SolidColorPaint(new SKColor(220, 53, 69)), // Red
                InnerRadius = 50
            });

            _paymentSuccessChart.Series = series;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading payment success data: {ex.Message}");
        }
    }

    private void LoadTransactionVolumeData()
    {
        try
        {
            // Use booking dates as a proxy for transaction dates
            string query = @"
                SELECT 
                    FORMAT(tb.StartDate, 'yyyy-MM') AS Month,
                    COUNT(*) AS BookingCount,
                    SUM(t.PriceRange) AS TotalAmount
                FROM Trip_Booking tb
                JOIN Trip t ON tb.TripID = t.TripID
                WHERE tb.Status = 'Approved'
                GROUP BY FORMAT(tb.StartDate, 'yyyy-MM')
                ORDER BY Month";

            var months = new List<string>();
            var counts = new List<double>();
            var amounts = new List<double>();

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        int count = Convert.ToInt32(reader["BookingCount"]);
                        double amount = reader["TotalAmount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TotalAmount"]);

                        months.Add(month);
                        counts.Add(count);
                        amounts.Add(amount / 1000); // Convert to thousands for better readability
                    }
                }
            }

            if (months.Count == 0)
            {
                // Fallback demo data - last 6 months
                months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
                counts = new List<double> { 45, 52, 78, 65, 88, 95 };
                amounts = new List<double> { 22.5, 26.0, 39.0, 32.5, 44.0, 47.5 };
            }

            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = counts.ToArray(),
                    Name = "Transaction Count",
                    Fill = new SolidColorPaint(new SKColor(13, 110, 253)), // Primary blue
                    MaxBarWidth = 30
                },
                new LineSeries<double>
                {
                    Values = amounts.ToArray(),
                    Name = "Transaction Volume (thousands)",
                    Stroke = new SolidColorPaint(new SKColor(255, 193, 7)) { StrokeThickness = 3 }, // Yellow
                    GeometryFill = new SolidColorPaint(new SKColor(255, 193, 7)),
                    GeometrySize = 10,
                    ScalesYAt = 1
                }
            };

            _transactionVolumeChart.Series = series;
            _transactionVolumeChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _transactionVolumeChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Transaction Count"
                },
                new Axis
                {
                    Name = "Volume (thousands)",
                    Position = LiveChartsCore.Measure.AxisPosition.End
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transaction volume data: {ex.Message}");
        }
    }

    private void LoadChargebackRateData()
    {
        try
        {
            // Since there's no direct chargeback data in the schema, we'll create simulated data
            // In a real system, this would query a payment processing table for chargeback rates
            
            // Demo months
            var months = new List<string> { "2023-06", "2023-07", "2023-08", "2023-09", "2023-10", "2023-11" };
            
            // Demo chargeback rates - typically low percentages (0.5% - 1.5%)
            var chargebackRates = new List<double> { 0.8, 0.7, 0.9, 1.2, 0.6, 0.5 };
            
            // Industry average for reference
            var industryAverage = new List<double> { 0.9, 0.9, 0.9, 0.9, 0.9, 0.9 };

            var series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = chargebackRates.ToArray(),
                    Name = "Chargeback Rate (%)",
                    Stroke = new SolidColorPaint(new SKColor(220, 53, 69)) { StrokeThickness = 3 }, // Red
                    Fill = new SolidColorPaint(new SKColor(220, 53, 69, 40)),
                    GeometryFill = new SolidColorPaint(new SKColor(220, 53, 69)),
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = industryAverage.ToArray(),
                    Name = "Industry Average (%)",
                    Stroke = new SolidColorPaint(new SKColor(108, 117, 125)) { StrokeThickness = 2 }, // Gray
                    Fill = null,
                    GeometryFill = null,
                    GeometrySize = 0
                }
            };

            _chargebackRateChart.Series = series;
            _chargebackRateChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = months.ToArray(),
                    LabelsRotation = 45
                }
            };
            _chargebackRateChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Chargeback Rate (%)",
                    MinLimit = 0,
                    MaxLimit = 2
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading chargeback rate data: {ex.Message}");
        }
    }

    private void LoadPaymentMethodData()
    {
        try
        {
            // Since there's no payment method data in the schema, we'll create simulated data
            // In a real system, this would query a payment processing table
            
            // Demo payment methods and their distribution
            var paymentMethods = new Dictionary<string, double>
            {
                { "Credit Card", 65 },
                { "Debit Card", 15 },
                { "PayPal", 12 },
                { "Bank Transfer", 5 },
                { "Apple Pay", 2 },
                { "Google Pay", 1 }
            };

            var series = new List<PieSeries<double>>();
            var colors = new List<SKColor>
            {
                new SKColor(52, 152, 219),  // Blue
                new SKColor(46, 204, 113),  // Green
                new SKColor(155, 89, 182),  // Purple
                new SKColor(230, 126, 34),  // Orange
                new SKColor(52, 73, 94),    // Dark Blue
                new SKColor(149, 165, 166)  // Gray
            };

            int colorIndex = 0;
            foreach (var method in paymentMethods)
            {
                series.Add(new PieSeries<double>
                {
                    Values = new double[] { method.Value },
                    Name = method.Key,
                    Fill = new SolidColorPaint(colors[colorIndex % colors.Count])
                });
                colorIndex++;
            }

            _paymentMethodChart.Series = series;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading payment method data: {ex.Message}");
        }
    }
}