using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace DB_Project.TravellerPages
{
    public partial class TripSearchPage : UserControl
    {
        public TripSearchPage()
        {
            InitializeComponent();
            
            // Set up price slider event
            PriceSlider.PropertyChanged += (sender, e) => {
                if (e.Property.Name == "Value")
                {
                    UpdatePriceLabel();
                }
            };
            
            UpdatePriceLabel();
            
            // Load some initial results for demo
            LoadSearchResults();
        }
        
        private void UpdatePriceLabel()
        {
            if (PriceDisplay != null)
            {
                PriceDisplay.Text = $"${(int)PriceSlider.Value}";
            }
        }
        
        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            // In a real app, this would query the database
            // For now, just reload the same demo data
            LoadSearchResults();
        }
        
        private void LoadSearchResults()
        {
            var results = new ObservableCollection<TripSearchResult>
            {
                new TripSearchResult {
                    Name = "Paris Explorer",
                    Destination = "Paris, France",
                    Description = "Discover the City of Light with our guided tour...",
                    Duration = "5 days",
                    Price = "$1,299",
                    ImagePath = "../Assets/paris.png"
                },
                new TripSearchResult {
                    Name = "Rome Adventure",
                    Destination = "Rome, Italy",
                    Description = "Walk through ancient history in the Eternal City...",
                    Duration = "7 days",
                    Price = "$1,599",
                    ImagePath = "../Assets/rome.png"
                }
                // Add more results as needed
            };
            
            SearchResultsControl.ItemsSource = results;
        }
    }
    
    public class TripSearchResult
    {
        public string Name { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        public string ImagePath { get; set; }
    }
}