using Avalonia;
using Avalonia.Controls;
using DB_Project.CompanyPages;
using System;
using Avalonia.Threading;

namespace DB_Project
{
    public partial class OperatorShell : BaseShell
    {
        public OperatorShell(string username) : base(username)
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
                        throw new InvalidOperationException("SplitView control not found in OperatorShell");
                    if (sidebar == null)
                        throw new InvalidOperationException("Sidebar control not found in OperatorShell");
                    if (mainContent == null)
                        throw new InvalidOperationException("MainContent control not found in OperatorShell");

                    InitializeSidebar(splitView, sidebar, mainContent);

                    var analyticsPage = new CompanyAnalytics(username);
                    var createTripPage = new CompanyCreate(username);
                    var companyManagePage = new CompanyManage(username);
                    var manageTripsPage = new CompanyViewEdit(username);
                    var hotelPartnersPage = new CompanyHotel(username);
                    var efficiencyReport = new EfficiencyReport();
                    sidebar.AddTab("Analytics", analyticsPage);
                    sidebar.AddTab("Create Trip", createTripPage);
                    sidebar.AddTab("Manage Bookings", companyManagePage);
                    sidebar.AddTab("Manage Trips", manageTripsPage);
                    sidebar.AddTab("Hotel Partners", hotelPartnersPage);
                    sidebar.AddTab("Efficiency Report", efficiencyReport);

                    ConfigureCommonTabs(sidebar);

                    mainContent.Content = analyticsPage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing OperatorShell: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }
    }
}