using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DB_Project.Services;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Data.SqlClient;

namespace DB_Project.CompanyPages;

public partial class CompanyCreate : UserControl
{
    
    public ObservableCollection<InclusionsEntry> inclusions;
    public ObservableCollection<ItineraryEntry> itineraries;
    private readonly string un;
    public CompanyCreate(string username)
    {
        InitializeComponent();
        inclusions = new ObservableCollection<InclusionsEntry>();
        itineraries = new ObservableCollection<ItineraryEntry>();
        ItineraryItemsControl.ItemsSource = itineraries;
        InclusionsItemsControl.ItemsSource = inclusions;
        un = username;
    }

    

    private void ItineraryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //LoadItinerary();
        
        itineraries.Add(new() {Action = ActionTextBox.Text, DateTime = (ItineraryDatePicker.SelectedDate.Value.Date + ItineraryTimePicker.SelectedTime.Value).ToString("yyyy-MM-dd HH:mm:ss")});
    }

    private void LoadItinerary()
    {
    }

    private void InclusionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //LoadInclusions();
        inclusions.Add(new() {Action = InclusionTextBox.Text, Price = Convert.ToInt32(InclusionPrice.Text)});
    }

    private void LoadInclusions()
    {
        var results = new ObservableCollection<InclusionsEntry>
        {
            // new()
            // {
            //     DateTime = "2025-06-10 08:00",
            //     Action = "Airport transfer from home to JFK included"
            // },
            // new()
            // {
            //     DateTime = "2025-06-11 07:00",
            //     Action = "Complimentary breakfast at hotel"
            // },
            // new()
            // {
            //     DateTime = "2025-06-12 10:00",
            //     Action = "Guided city tour with English-speaking guide"
            // }
        };

    }

    private void SubmitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        int tid;
        try
        {
            var query =
                "INSERT INTO Trip (Title, [Type], CancellationPolicy, GroupSize, StartDate, EndDate, PriceRange, OperatorUsername) OUTPUT inserted.TripID VALUES (@Title, @Type, @CancellationPolicy, @GroupSize, @StartDate, @EndDate, @Price, @OperatorUsername);";
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                command.Parameters.AddWithValue("@OperatorUsername", un);
                command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                command.Parameters.AddWithValue("@Type", TypeTextBox.Text);
                command.Parameters.AddWithValue("@CancellationPolicy", (CancelPolicyComboBox.SelectedIndex == 0 ? "Refundable" : "Non-Refundable"));
                command.Parameters.AddWithValue("@GroupSize", Convert.ToInt32(GroupSizeTextBox.Text));
                command.Parameters.AddWithValue("@StartDate", StartDatePicker.SelectedDate.Value.Date);
                command.Parameters.AddWithValue("@EndDate", EndDatePicker.SelectedDate.Value.Date);
                command.Parameters.AddWithValue("@Price", Convert.ToInt32(PriceTextBox.Text));
                connection.Open();

                tid = (int)command.ExecuteScalar();
                connection.Close();
            }

            int existingval = -1;
            
            string checkQuery = "SELECT DestID FROM Destination WHERE Location = @Location AND City = @City AND Country = @Country";

            using (var connection = DatabaseService.Instance.CreateConnection())
            using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
            {
                if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                checkCmd.Parameters.AddWithValue("@Location", LocationTextBox.Text);
                checkCmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                checkCmd.Parameters.AddWithValue("@Country", CountryTextBox.Text);
                connection.Open();
                var existingId = checkCmd.ExecuteScalar();
                if (existingId != null && existingId != DBNull.Value)
                {
                    existingval = Convert.ToInt32(existingId);
                }
                connection.Close();
            }

            if (existingval == -1)
            {
                string insertQuery = "INSERT INTO Destination (Location, City, Country, JoinDate) OUTPUT INSERTED.DestID VALUES (@Location, @City, @Country, @JoinDate)";
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                {
                    if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                    insertCmd.Parameters.AddWithValue("@Location", LocationTextBox.Text);
                    insertCmd.Parameters.AddWithValue("@City", CityTextBox.Text);
                    insertCmd.Parameters.AddWithValue("@Country", CountryTextBox.Text);
                    insertCmd.Parameters.AddWithValue("@JoinDate", DateTime.Today);
                    connection.Open();
                    existingval = (int)insertCmd.ExecuteScalar();
                    connection.Close();
                }
            }

            string tripdestquery = "INSERT INTO Trip_Destination VALUES (@TID, @DID)";
            using (var connection = DatabaseService.Instance.CreateConnection())
            using (SqlCommand insertCmd = new SqlCommand(tripdestquery, connection))
            {
                if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                insertCmd.Parameters.AddWithValue("@TID", tid);
                insertCmd.Parameters.AddWithValue("@DID", existingval);
                connection.Open();
                insertCmd.ExecuteNonQuery();
                connection.Close();
            }

            using (var connection = DatabaseService.Instance.CreateConnection())
            {
                string inclusionquery = "INSERT INTO Trip_Inclusions VALUES (@TID, @Action, @Price);";
                string itineraryquery = "INSERT INTO Trip_Itinerary VALUES (@TID, @Action, @StartDate, @EndDate);";
                foreach (var entry in inclusions)
                {
                    using (SqlCommand insertCmd = new SqlCommand(inclusionquery, connection))
                    {
                        if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                        insertCmd.Parameters.AddWithValue("@TID", tid);
                        insertCmd.Parameters.AddWithValue("@Action", entry.Action);
                        insertCmd.Parameters.AddWithValue("@Price", entry.Price);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                foreach (var entry in itineraries)
                {
                    using (SqlCommand insertCmd = new SqlCommand(itineraryquery, connection))
                    {
                        if (DatabaseService.Instance.TestConnection() == false) Console.Write("Failed");
                        insertCmd.Parameters.AddWithValue("@TID", tid);
                        insertCmd.Parameters.AddWithValue("@Action", entry.Action);
                        insertCmd.Parameters.AddWithValue("@StartDate", entry.DateTime);
                        insertCmd.Parameters.AddWithValue("@EndDate", entry.DateTime);
                        insertCmd.ExecuteNonQuery();
                    }
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

public class ItineraryEntry
{
    public string DateTime { get; set; }
    public string Action { get; set; }
}

public class InclusionsEntry
{
    public int Price { get; set; }
    public string Action { get; set; }
}