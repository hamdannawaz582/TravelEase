using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DB_Project;

public partial class TravelEaseLogo : UserControl
{
    public TravelEaseLogo()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        TravelEaseLabel.FontSize = Bounds.Height * 0.65;
    }
}