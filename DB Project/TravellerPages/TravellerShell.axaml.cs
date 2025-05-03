using Avalonia;
using Avalonia.Controls;
using DB_Project.TravellerPages;
using System;
using Avalonia.Threading;

namespace DB_Project
{
    public partial class TravellerShell : BaseShell
    {
        public TravellerShell(string username) : base(username)
        {
            InitializeComponent();
            
            // Defer initialization until after component initialization is fully complete
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    // Find controls
                    SplitView = this.FindControl<SplitView>("SplitView");
                    Sidebar = this.FindControl<SidebarControl>("Sidebar");
                    MainContent = this.FindControl<ContentControl>("MainContent");
                    
                    // Verify controls were found
                    if (SplitView == null)
                        throw new InvalidOperationException("SplitView control not found");
                    if (Sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found");
                    if (MainContent == null)
                        throw new InvalidOperationException("MainContent control not found");
                    
                    Sidebar.SetSplitView(SplitView);
                    Sidebar.SetPageHost(MainContent);
                    var dashboardPage = new TravellerDashboard(username);
                    var searchPage = new TripSearchPage();
                    var reviewsPage = new TravellerReview();
                    Sidebar.AddTab("Dashboard", dashboardPage);
                    Sidebar.AddTab("Search Trips", searchPage);
                    Sidebar.AddTab("My Reviews", reviewsPage);
                    ConfigureCommonTabs();
                    MainContent.Content = dashboardPage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing TravellerShell: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }
    }
}