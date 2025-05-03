using Avalonia.Controls;

namespace DB_Project
{
    public abstract class BaseShell : UserControl
    {
        protected SplitView SplitView;
        protected SidebarControl Sidebar;
        protected ContentControl MainContent;
        protected string Username;
        
        public BaseShell(string username)
        {
            Username = username;
        }
        
        protected void InitializeSidebar()
        {
            Sidebar.SetSplitView(SplitView);
            Sidebar.SetPageHost(MainContent);
        }
        
        protected void ConfigureCommonTabs()
        {
            Sidebar.AddTab("Login Page", new LoginPage());
        }
    }
}