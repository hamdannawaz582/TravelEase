using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DB_Project;

public partial class HotelSignupPage : UserControl
{
    public HotelSignupPage()
    {
        InitializeComponent();
    }

    private void OnSubmit(object? sender, RoutedEventArgs e)
    {
        var username = this.FindControl<TextBox>("UsernameBox").Text;
        var name = this.FindControl<TextBox>("NameBox").Text;
        var password = this.FindControl<TextBox>("PasswordBox").Text;

        // Handle form submission logic here
        // Example: Save data to the database or validate input
    }
}