using Avalonia.Controls;
using System; // Added for InvalidOperationException

namespace DB_Project
{
    public abstract class BaseShell : UserControl
    {
        // Removed fields: SplitView, Sidebar, MainContent
        protected string Username;

        public BaseShell(string username)
        {
            Username = username;
        }

        // Modified to accept controls as parameters
        protected void InitializeSidebar(SplitView splitView, SidebarControl sidebar, ContentControl mainContent)
        {
            // Add null checks for parameters
            if (splitView == null)
                throw new ArgumentNullException(nameof(splitView), "SplitView cannot be null when initializing sidebar");
            if (sidebar == null)
                throw new ArgumentNullException(nameof(sidebar), "Sidebar cannot be null when initializing sidebar");
            if (mainContent == null)
                throw new ArgumentNullException(nameof(mainContent), "MainContent cannot be null when initializing sidebar");

            // These need to be set in this order
            sidebar.SetSplitView(splitView);
            sidebar.SetPageHost(mainContent);
        }

        // Modified to accept sidebar as a parameter
        protected void ConfigureCommonTabs(SidebarControl sidebar)
        {
            if (sidebar == null)
                throw new ArgumentNullException(nameof(sidebar), "Sidebar cannot be null when configuring common tabs");

            sidebar.AddTab("Login Page", new LoginPage());
        }
    }
}