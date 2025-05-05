using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DB_Project;

public partial class AdminSignupPage : UserControl
{
    public AdminSignupPage()
    {
        InitializeComponent();
    }

    private void OnSubmit(object? sender, RoutedEventArgs e)
    {
        var username = this.FindControl<TextBox>("UsernameBox").Text;
        var email = this.FindControl<TextBox>("EmailBox").Text;
        var password = this.FindControl<TextBox>("PasswordBox").Text;

        // Handle form submission logic here
        // Example: Save data to the database or validate input
    }
}