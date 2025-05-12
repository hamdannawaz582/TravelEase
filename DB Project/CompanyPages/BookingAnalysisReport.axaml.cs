using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DB_Project.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages;

public partial class BookingAnalysisReport : UserControl
{
    public BookingAnalysisReport(string operatorUsername)
    {
        InitializeComponent();
        LoadReport(operatorUsername);
    }

private void LoadReport(string operatorUsername)
{
    // Fetch Abandonment Rate
    var abandonmentRate = GetAbandonmentRate(operatorUsername);
    var bookedRate = 1 - abandonmentRate;

    // Update Abandonment Rate Text
    AbandonmentRateTextBlock.Text = $"Abandonment Rate: {abandonmentRate:P}";

    // Display Abandonment Rate in Pie Chart
    AbandonmentRateChart.Series = new ISeries[]
    {
        new PieSeries<double> { Values = new[] { abandonmentRate }, Name = "Abandoned" },
        new PieSeries<double> { Values = new[] { bookedRate }, Name = "Booked" }
    };
    
    var potentialRevenueLoss = GetPotentialRevenueLoss(operatorUsername);
    PotentialRevenueLossTextBlock.Text = $"Potential Revenue Loss: {potentialRevenueLoss:C}";

}

    private double GetAbandonmentRate(string operatorUsername)
    {
        double totalCart = 0, totalBookings = 0;

        string queryCart = @"
            SELECT COUNT(*) AS TotalCart
            FROM Traveller_Cart tc
            JOIN Trip t ON tc.TripID = t.TripID
            WHERE t.OperatorUsername = @OperatorUsername";

        string queryBookings = @"
            SELECT COUNT(*) AS TotalBookings
            FROM Trip_Booking tb
            JOIN Trip t ON tb.TripID = t.TripID
            WHERE t.OperatorUsername = @OperatorUsername";

        using (var connection = DatabaseService.Instance.CreateConnection())
        {
            // Get total trips in cart
            using (var command = new SqlCommand(queryCart, connection))
            {
                command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
                connection.Open();
                totalCart = Convert.ToDouble(command.ExecuteScalar());
                connection.Close();
            }

            // Get total bookings
            using (var command = new SqlCommand(queryBookings, connection))
            {
                command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
                connection.Open();
                totalBookings = Convert.ToDouble(command.ExecuteScalar());
                connection.Close();
            }
        }

        // Calculate abandonment rate
        return totalCart / (totalCart + totalBookings);
    }

    private decimal GetPotentialRevenueLoss(string operatorUsername)
    {
        decimal totalRevenueLoss = 0;

        string query = @"
            SELECT SUM(t.PriceRange) AS RevenueLoss
            FROM Traveller_Cart tc
            JOIN Trip t ON tc.TripID = t.TripID
            WHERE t.OperatorUsername = @OperatorUsername";

        using (var connection = DatabaseService.Instance.CreateConnection())
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OperatorUsername", operatorUsername);
            connection.Open();
            var result = command.ExecuteScalar();
            if (result != DBNull.Value)
                totalRevenueLoss = Convert.ToDecimal(result);
        }

        return totalRevenueLoss;
    }
}