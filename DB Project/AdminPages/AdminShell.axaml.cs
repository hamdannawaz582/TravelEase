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
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    //find controls
                    var splitView = this.FindControl<SplitView>("SplitView");
                    var sidebar = this.FindControl<SidebarControl>("Sidebar");
                    var mainContent = this.FindControl<ContentControl>("MainContent");
                    
                    if (splitView == null)
                        throw new InvalidOperationException("SplitView control not found in AdminShell");
                    if (sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found in AdminShell");
                    if (mainContent == null)
                        throw new InvalidOperationException("MainContent control not found in AdminShell");
                    
                    InitializeSidebar(splitView, sidebar, mainContent);
                    var analyticsPage = new AdminAnalytics();
                    var managementPage = new AdminManagement();
                    var catergoryManagement = new CategoryManagement();
                    var reviewsPage = new AdminReviews();
                    Sidebar.AddTab("Analytics", analyticsPage);
                    Sidebar.AddTab("Admin Management", managementPage);
                    Sidebar.AddTab("Category Management", catergoryManagement);
                    Sidebar.AddTab("Reviews", reviewsPage);
                    ConfigureCommonTabs(sidebar);
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