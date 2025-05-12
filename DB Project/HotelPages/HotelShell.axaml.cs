using Avalonia;
using Avalonia.Controls;
using DB_Project.HotelPages;
using System;
using Avalonia.Threading;

namespace DB_Project
{
    public partial class HotelShell : BaseShell
    {
        public HotelShell(string username) : base(username)
        {
            InitializeComponent();
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var splitView = this.FindControl<SplitView>("SplitView");
                    var sidebar = this.FindControl<SidebarControl>("Sidebar");
                    var mainContent = this.FindControl<ContentControl>("MainContent");

                    if (splitView == null)
                        throw new InvalidOperationException("SplitView control not found in HotelShell");
                    if (sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found in HotelShell");
                    if (mainContent == null)
                        throw new InvalidOperationException("MainContent control not found in HotelShell");
                    
                    InitializeSidebar(splitView, sidebar, mainContent);
                    var analyticsPage = new HotelAnalytics(username);
                    var managementPage = new HotelManagement();
                    sidebar.AddTab("Analytics", analyticsPage);
                    sidebar.AddTab("Room Management", managementPage);
                    ConfigureCommonTabs(sidebar);
                    mainContent.Content = analyticsPage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing HotelShell: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }
    }
}