using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DB_Project;

public partial class ChartControl : UserControl
{
    public ObservableCollection<ISeries> Series { get; set; }
    public ChartControl()
    {
        InitializeComponent();
        Series = [
            new LineSeries<ObservablePoint>
            {
                Values = [
                    new ObservablePoint(0, 4),
                    new ObservablePoint(1, 3),
                    new ObservablePoint(3, 8),
                    new ObservablePoint(18, 6),
                    new ObservablePoint(20, 12)
                ]
            }
        ];
        DataContext = this;
        
    }
    
    public void SetChart(ISeries[] series)
    {
        Series.Clear();
        foreach (var point in series)
        {
            Series.Add(point);
        }
    }
    public void SetLabel(string label) => Label.Text = label;
}