using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace DB_Project;

public partial class TravellerSignupPage : UserControl
{
    public TravellerSignupPage()
    {
        InitializeComponent();
    }

    private void OnSubmit(object? sender, RoutedEventArgs e)
    {
        var username = this.FindControl<TextBox>("UsernameBox").Text;
        var email = this.FindControl<TextBox>("EmailBox").Text;
        var password = this.FindControl<TextBox>("PasswordBox").Text;
        var fname = this.FindControl<TextBox>("FNameBox").Text;
        var lname = this.FindControl<TextBox>("LNameBox").Text;
        var nationality = this.FindControl<TextBox>("NationalityBox").Text;
        var age = this.FindControl<TextBox>("AgeBox").Text;
        var budget = this.FindControl<TextBox>("BudgetBox").Text;

        // FIXME: Add validation and database registration logic here
        
        // Return to login page
        if (this.VisualRoot is Window mainWindow)
        {
            // Create new login page
            ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
        }
    }
}