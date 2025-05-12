using System;
using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;

namespace DB_Project
{
    public partial class CreateEditGenericWindow : UserControl, INotifyPropertyChanged
    {  
        private object _currentPage;
        
        // public CreateEditGenericWindow()
        // {
        //     InitializeComponent();
        //     DataContext = this;
        //     Sidebar.SetSplitView(SplitView);
        //     Sidebar.SetPageHost(MainContent);
        //     Sidebar.AddTab("Home", new LoginPage());
        //     Sidebar.AddTab("Settings", new UserControl());
        //     Sidebar.AddTab("About", new UserControl());
        // }
        // //=================================================================================
        // public event PropertyChangedEventHandler PropertyChanged;
        // protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        // {
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }
        // private void OnCreateCardClick(object sender, RoutedEventArgs e)
        // {
        //       var card = CardControl.Create(CardsContainer);
        //   }

    }
}