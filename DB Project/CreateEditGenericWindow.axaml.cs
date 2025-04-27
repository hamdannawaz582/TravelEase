using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Interactivity;

namespace DB_Project
{
    public partial class CreateEditGenericWindow : Window, INotifyPropertyChanged
    {   private bool _isPaneOpen = true;
        private object _currentPage;
        
        public CreateEditGenericWindow()
        {
            InitializeComponent();
            DataContext = this;
            //SwitchToHomePageCommand = new RelayCommand(SwitchToHomePage);
            CurrentPage = new MainWindow();
        }
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set
            {
                if (_isPaneOpen != value)
                {
                    _isPaneOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       // Command to trigger the pane open/close
        public ICommand TriggerPaneCommand { get; }
        public ICommand SwitchToHomePageCommand { get; }
        
        private void TriggerPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }
        
        public void SwitchToHomePage()
        {
            CurrentPage = new MainWindow();
        }
    }
}