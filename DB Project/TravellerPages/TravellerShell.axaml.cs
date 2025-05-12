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
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var splitView = this.FindControl<SplitView>("SplitView");
                    var sidebar = this.FindControl<SidebarControl>("Sidebar");
                    var mainContent = this.FindControl<ContentControl>("MainContent");

                    if (splitView == null)
                        throw new InvalidOperationException("SplitView control not found in TravellerShell");
                    if (sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found in TravellerShell");
                    if (mainContent == null)
                        throw new InvalidOperationException("MainContent control not found in TravellerShell");

                    InitializeSidebar(splitView, sidebar, mainContent);

                    var dashboardPage = new TravellerDashboard(username);
                    var searchPage = new TripSearchPage(username);
                    var reviewsPage = new TravellerReview(username);

                    sidebar.AddTab("Dashboard", dashboardPage);
                    sidebar.AddTab("Search Trips", searchPage);
                    sidebar.AddTab("My Reviews", reviewsPage);

                    ConfigureCommonTabs(sidebar);

                    mainContent.Content = dashboardPage;
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