using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Data.Converters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Globalization;
using Avalonia.Layout;

namespace DB_Project
{
    public partial class SidebarControl : UserControl, INotifyPropertyChanged
    {   private bool _isPaneOpen = true;
        private ContentControl _pageHost;
        private SplitView _splitView;

        public SidebarControl()
        {
            InitializeComponent();
            //_splitView = this.FindControl<SplitView>("SplitView");
        }

        public void SetPageHost(ContentControl pageHost)
        {
            _pageHost = pageHost;
        }
        public void SetSplitView(SplitView splitView)
        {
            _splitView = splitView;
            _splitView.IsPaneOpen = _isPaneOpen;
            DataContext = this;
        }
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (_isPaneOpen != value)
                {
                    _isPaneOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void AddTab(string title, UserControl page)
        {
            var tab = new SidebarTab();
            tab.SetText(title);
            tab.PageName = title;
            tab.Clicked += (s, e) => 
            {
                if (_pageHost != null)
                {   //_pageHost.Content = null;//clears the page first
                    _pageHost.Content = page;
                }
            };

            TabPanel.Children.Add(tab);
        }
        private void OnTogglePaneClick(object sender, RoutedEventArgs e)
        {
            if (_splitView != null)
            {
                _splitView.IsPaneOpen = !_splitView.IsPaneOpen;
            }
            var travelEaseLogo = this.FindControl<TravelEaseLogo>("TravelEaseLogo");
            if (travelEaseLogo != null)
            {
                travelEaseLogo.HorizontalAlignment = _splitView.IsPaneOpen ? 
                    HorizontalAlignment.Center : HorizontalAlignment.Center;
                travelEaseLogo.Width = _splitView.IsPaneOpen ? 150 : 40;
        
                var travelEaseLabel = travelEaseLogo.FindControl<TextBlock>("TravelEaseLabel");
                if (travelEaseLabel != null)
                {
                    travelEaseLabel.IsVisible = _splitView.IsPaneOpen;
                    travelEaseLabel.HorizontalAlignment = _splitView.IsPaneOpen ? 
                        HorizontalAlignment.Center : HorizontalAlignment.Center;
                }
            }
            foreach (var child in TabPanel.Children)
            {
                if (child is SidebarTab tab)
                {
                    var textBlock = tab.FindControl<TextBlock>("TabText");
                    if (textBlock != null)
                    {
                        textBlock.IsVisible = _splitView.IsPaneOpen;
                    }
                }
            }
            
        }
        
    }
}