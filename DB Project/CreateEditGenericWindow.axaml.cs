using System;
using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;

namespace DB_Project
{
    public partial class CreateEditGenericWindow : Window, INotifyPropertyChanged
    {   private bool _isPaneOpen = true;
        private object _currentPage;
        public ObservableCollection<Tuple<string, Action>> Items { get; set; }
        
        public CreateEditGenericWindow()
        {
            InitializeComponent();
            DataContext = this;

            Items = new ObservableCollection<Tuple<string, Action>>
            {
                new Tuple<string, Action>("Home", () =>
                {
                    var mainWindow = new MainWindow();
                    this.Content = mainWindow.Content;
                }),
                new Tuple<string, Action>("Settings", () => Console.WriteLine("Settings selected")),
                new Tuple<string, Action>("About", () => Console.WriteLine("About selected"))
            };
        }
        //=================================================================================
        //To close the nav pane
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (_isPaneOpen != value)
                {
                    _isPaneOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        private void OnTogglePaneClick(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !IsPaneOpen;
            Console.WriteLine($"IsPaneOpen: {IsPaneOpen}");


        }
        //=================================================================================
        //To change the page
        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is Tuple<string, Action> selectedItem)
            {
                selectedItem.Item2.Invoke();
            }
        }
        //=================================================================================
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}