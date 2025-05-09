using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Microsoft.Data.SqlClient;
using DB_Project.Services;

namespace DB_Project;

public partial class TravellerSignupPage : UserControl
{
    public TravellerSignupPage()
    {
        InitializeComponent();
    }

    // Update OnSubmit in TravellerSignupPage.axaml.cs
private async void OnSubmit(object? sender, RoutedEventArgs e)
{
    var username = UsernameBox.Text;
    var email = EmailBox.Text;
    var password = PasswordBox.Text;
    var fname = FNameBox.Text;
    var lname = LNameBox.Text;
    var nationality = NationalityBox.Text;
    int.TryParse(AgeBox.Text, out int age);
    int.TryParse(BudgetBox.Text, out int budget);
    
    try
    {
        using var connection = DatabaseService.Instance.CreateConnection();
        await connection.OpenAsync();
        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        
        try
        {
            var userCommand = new SqlCommand(
                "INSERT INTO [User] (Username, Email, Password, JoinDate) " +
                "VALUES (@Username, @Email, @Password, @JoinDate)", 
                connection, transaction);
                
            userCommand.Parameters.AddWithValue("@Username", username);
            userCommand.Parameters.AddWithValue("@Email", email);
            userCommand.Parameters.AddWithValue("@Password", password);
            userCommand.Parameters.AddWithValue("@JoinDate", DateTime.Now);
            
            await userCommand.ExecuteNonQueryAsync();
            var travellerCommand = new SqlCommand(
                "INSERT INTO Traveller (Username, FName, LName, Nationality, Age, Budget) " +
                "VALUES (@Username, @FName, @LName, @Nationality, @Age, @Budget)", 
                connection, transaction);
                
            travellerCommand.Parameters.AddWithValue("@Username", username);
            travellerCommand.Parameters.AddWithValue("@FName", fname);
            travellerCommand.Parameters.AddWithValue("@LName", lname);
            travellerCommand.Parameters.AddWithValue("@Nationality", nationality);
            travellerCommand.Parameters.AddWithValue("@Age", age);
            travellerCommand.Parameters.AddWithValue("@Budget", budget);
            
            await travellerCommand.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
            
            if (this.VisualRoot is Window mainWindow)
            {
                ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Registration error: {ex.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
    }
}
}