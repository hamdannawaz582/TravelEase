using Avalonia;
using Avalonia.Controls;
using DB_Project.AdminPages;
using System;
using Avalonia.Threading;

namespace DB_Project
{
    public partial class AdminShell : BaseShell
    {
        public AdminShell(string username) : base(username)
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
                    var analyticsPage = new AdminAnalytics();
                    var managementPage = new AdminManagement();
                    var reviewsPage = new AdminReviews();
                    Sidebar.AddTab("Dashboard", analyticsPage);
                    Sidebar.AddTab("Management", managementPage);
                    Sidebar.AddTab("Reviews", reviewsPage);
                    ConfigureCommonTabs();
                    MainContent.Content = analyticsPage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing AdminShell: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }
    }
}