using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace DB_Project
{
    public partial class SidebarTab : UserControl
    {
        public string PageName { get; set; } // to know which page to open

        public SidebarTab()
        {
            InitializeComponent();
            this.PointerPressed += OnTabClicked;
        }

        public void SetText(string text)
        {
            TabText.Text = text;
        }

        private void OnTabClicked(object? sender, PointerPressedEventArgs e)
        {
            // raise event to tell parent sidebar that "i was clicked"
            var args = new RoutedEventArgs(ClickedEvent);
            RaiseEvent(args);
        }

        public static readonly RoutedEvent<RoutedEventArgs> ClickedEvent =
            RoutedEvent.Register<SidebarTab, RoutedEventArgs>(nameof(Clicked), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> Clicked
        {
            add => AddHandler(ClickedEvent, value);
            remove => RemoveHandler(ClickedEvent, value);
        }
    }
}