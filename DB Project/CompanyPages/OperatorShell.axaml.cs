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
                    var analyticsPage = new CompanyAnalytics();
                    var createTripPage = new CompanyCreate();
                    var manageTripsPage = new CompanyViewEdit();
                    var hotelPartnersPage = new CompanyHotel();
                    Sidebar.AddTab("Analytics", analyticsPage);
                    Sidebar.AddTab("Create Trip", createTripPage);
                    Sidebar.AddTab("Manage Trips", manageTripsPage);
                    Sidebar.AddTab("Hotel Partners", hotelPartnersPage);
                    ConfigureCommonTabs();
                    MainContent.Content = analyticsPage;
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