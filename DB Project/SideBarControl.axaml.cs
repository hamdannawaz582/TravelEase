using Avalonia.Controls;

namespace DB_Project
{
    public partial class SidebarControl : UserControl
    {
        private ContentControl _pageHost;

        public SidebarControl()
        {
            InitializeComponent();
        }

        public void SetPageHost(ContentControl pageHost)
        {
            _pageHost = pageHost;
        }

        public void AddTab(string title, UserControl page)
        {
            var tab = new SidebarTab();
            tab.SetText(title);
            tab.PageName = title;
            tab.Clicked += (s, e) => 
            {
                if (_pageHost != null)
                {
                    _pageHost.Content = page;
                }
            };

            TabPanel.Children.Add(tab);
        }
    }
}