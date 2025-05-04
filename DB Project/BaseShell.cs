using Avalonia.Controls;
using System; // Added for InvalidOperationException

namespace DB_Project
{
    public abstract class BaseShell : UserControl
    {
        protected string Username;

        public BaseShell(string username)
        {
            Username = username;
        }
        
        protected void InitializeSidebar(SplitView splitView, SidebarControl sidebar, ContentControl mainContent)
        {
            sidebar.SetSplitView(splitView);
            sidebar.SetPageHost(mainContent);
        }
        //Add common tabs shared between all shells here,, the login page is just added for testing
        protected void ConfigureCommonTabs(SidebarControl sidebar)
        {
            if (sidebar == null)
                throw new ArgumentNullException(nameof(sidebar), "Sidebar cannot be null when configuring common tabs");
            
            sidebar.AddTab("Login Page", new LoginPage());
        }
    }
}