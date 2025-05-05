using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
namespace DB_Project;

public partial class OperatorSignupPage : UserControl
{
    public OperatorSignupPage()
    {
        InitializeComponent();
    }

    private void OnSubmit(object? sender, RoutedEventArgs e)
    {
        var username = this.FindControl<TextBox>("UsernameBox").Text;
        var email = this.FindControl<TextBox>("EmailBox").Text;
        var password = this.FindControl<TextBox>("PasswordBox").Text;
        DateTime joinDate = DateTime.Now;
        
        if (this.VisualRoot is Window mainWindow)
        {
            ((MainWindow)mainWindow).MainContent.Content = new LoginPage();
        }
    }
}