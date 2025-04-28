using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DB_Project;

public partial class ChartPageSample : UserControl
{
    public ChartPageSample()
    {
        InitializeComponent();
        Cc.SetChart([
                new LineSeries<ObservablePoint>
                {
                    Values = [
                        new ObservablePoint(0, 4),
                        new ObservablePoint(1, 150),
                        new ObservablePoint(3, 8),
                        new ObservablePoint(18, 6),
                        new ObservablePoint(20, 12)
                    ]
                }
            ]
        );
        Cc.SetLabel("Test Data");
    }
}