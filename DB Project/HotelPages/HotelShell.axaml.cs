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
            
            // Defer initialization until after component initialization is fully complete
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    //find controls
                    SplitView = this.FindControl<SplitView>("SplitView");
                    Sidebar = this.FindControl<SidebarControl>("Sidebar");
                    MainContent = this.FindControl<ContentControl>("MainContent");
                    
                    if (SplitView == null)
                        throw new InvalidOperationException("SplitView control not found");
                    if (Sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found");
                    if (MainContent == null)
                        throw new InvalidOperationException("MainContent control not found");
                    
                    InitializeSidebar();
                    var analyticsPage = new HotelAnalytics();
                    var managementPage = new HotelManagement();
                    Sidebar.AddTab("Analytics", analyticsPage);
                    Sidebar.AddTab("Room Management", managementPage);
                    ConfigureCommonTabs();
                    MainContent.Content = analyticsPage;
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