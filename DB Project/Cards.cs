using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DB_Project;

public class Cards
{
    public static Border CreateCard(Panel container)
    {
        var card = new Border
        {
            Background = Brushes.LightGray,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Margin = new Thickness(5),
            Padding = new Thickness(10),
        };

        var closeButton = new Button
        {
            Content = " ×",
            Width = 20,
            Height = 22,
            FontSize = 14,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Padding = new Thickness(0),
        };
        
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var topRowPanel = new Grid();
        topRowPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        topRowPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        var title = new TextBlock
        {
            Text = "Card Title",
            FontWeight = Avalonia.Media.FontWeight.Bold,
            FontSize = 16,
            Foreground = Brushes.Black,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);
        Grid.SetColumn(closeButton, 1);

        topRowPanel.Children.Add(title);
        topRowPanel.Children.Add(closeButton);

        Grid.SetRow(topRowPanel, 0);

        var description = new TextBlock
        {
            Text = "This is a newly created card.",
            Foreground = Brushes.Black,
            Margin = new Thickness(0, 5, 0, 0)
        };
        Grid.SetRow(description, 1);

        grid.Children.Add(topRowPanel);
        grid.Children.Add(description);

        card.Child = grid;

        // Add the card to the container
        container.Children.Add(card);

        // Set up the remove functionality
        closeButton.Click += (s, e) => container.Children.Remove(card);

        return card;
    }
}