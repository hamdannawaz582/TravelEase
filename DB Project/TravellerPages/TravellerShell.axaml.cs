using Avalonia.Controls;
using DB_Project.TravellerPages;

namespace DB_Project
{
    public partial class TravellerShell : BaseShell
    {
        public TravellerShell(string username) : base(username)
        {
            InitializeComponent();
            InitializeSidebar();
            var dashboardPage = new TravellerDashboard(username);
            var searchPage = new TripSearchPage();
            var reviewsPage = new TravellerReview();
            
            //traveler-specific tabs
            Sidebar.AddTab("Dashboard", dashboardPage);
            Sidebar.AddTab("Search Trips", searchPage);
            Sidebar.AddTab("My Reviews", reviewsPage);
            
            //common tabs
            ConfigureCommonTabs();
            
            //default page
            MainContent.Content = dashboardPage;
        }
    }
}