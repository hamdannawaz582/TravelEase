using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using DB_Project.Models;
using DB_Project.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DB_Project.TravellerPages
{
    public partial class TripSearchPage : UserControl
    {
        private readonly TravellerRepository _repository = new TravellerRepository();
        private string _username;
        
        public TripSearchPage(string username)
        {
            InitializeComponent();
            _username = username;
            
            PriceSlider.PropertyChanged += (sender, e) => {
                if (e.Property.Name == "Value")
                {
                    UpdatePriceLabel();
                }
            };
            
            InitializeControls();
            UpdatePriceLabel();
            LoadSearchResults();
        }
        
        private void InitializeControls()
        {
            GroupSizeCombo.Items.Clear();
            GroupSizeCombo.Items.Add("Any Size");
            GroupSizeCombo.Items.Add("Small (1-5)");
            GroupSizeCombo.Items.Add("Medium (6-15)");
            GroupSizeCombo.Items.Add("Large (16+)");
            GroupSizeCombo.SelectedIndex = 0;
            StartDatePicker.SelectedDate = DateTime.Now.Date;
            EndDatePicker.SelectedDate = DateTime.Now.Date.AddDays(7);
            LoadAccessibilityOptions();
        }
        
        private async void LoadAccessibilityOptions()
        {
            try
            {
                var options = await _repository.GetAccessibilityOptions();
                AccessibilityOptionsPanel.Children.Clear();
                
                foreach (var option in options)
                {
                    var checkBox = new CheckBox { Content = option.Option, Tag = option.AccessibilityID };
                    AccessibilityOptionsPanel.Children.Add(checkBox);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading accessibility options: {ex.Message}");
                // Add a couple of fallback options
                AccessibilityOptionsPanel.Children.Add(new CheckBox { Content = "Wheelchair Access" });
                AccessibilityOptionsPanel.Children.Add(new CheckBox { Content = "Sign Language" });
            }
        }
        
        private void UpdatePriceLabel()
        {
            if (PriceDisplay != null)
            {
                PriceDisplay.Text = $"${(int)PriceSlider.Value}";
            }
        }
        
        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            SearchButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            
            try
            {
                string destination = DestinationBox.Text?.Trim();
                DateTime? startDate = StartDatePicker.SelectedDate?.Date;
                DateTime? endDate = EndDatePicker.SelectedDate?.Date;
                string tripType = (TripTypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                int maxPrice = (int)PriceSlider.Value;
                
                int? minGroupSize = null;
                int? maxGroupSize = null;
                string groupSizeSelection = GroupSizeCombo.SelectedItem?.ToString();
                if (groupSizeSelection == "Small (1-5)")
                {
                    maxGroupSize = 5;
                }
                else if (groupSizeSelection == "Medium (6-15)")
                {
                    minGroupSize = 6;
                    maxGroupSize = 15;
                }
                else if (groupSizeSelection == "Large (16+)")
                {
                    minGroupSize = 16;
                }

                var accessibilityIds = new List<int>();
                foreach (CheckBox checkBox in AccessibilityOptionsPanel.Children)
                {
                    if (checkBox.IsChecked == true && checkBox.Tag is int id)
                    {
                        accessibilityIds.Add(id);
                    }
                }
                
                var searchResults = await _repository.SearchTrips(
                    destination, 
                    startDate, 
                    endDate, 
                    tripType, 
                    maxPrice,
                    minGroupSize,
                    maxGroupSize,
                    accessibilityIds
                );
                
                await LoadSearchResultsFromTrips(searchResults);
                
                // Handle no results
                if (searchResults.Count == 0)
                {
                    NoResultsMessage.IsVisible = true;
                }
                else
                {
                    NoResultsMessage.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                ErrorMessage.Text = "An error occurred while searching. Please try again.";
                ErrorMessage.IsVisible = true;
            }
            finally
            {
                SearchButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
            }
        }
        
        private async Task LoadSearchResultsFromTrips(List<Trip> trips)
        {
            var results = new ObservableCollection<TripSearchResult>();
            
            foreach (var trip in trips)
            {
                results.Add(new TripSearchResult {
                    TripID = trip.TripID,
                    Name = trip.Title,
                    Destination = trip.Destination,
                    Description = $"{trip.Type} trip with {trip.CancellationPolicy} policy",
                    Duration = GetDuration(trip.StartDate, trip.EndDate),
                    Price = $"${trip.PriceRange}",
                    GroupSize = trip.GroupSize,
                    StartDate = trip.StartDate,
                    EndDate = trip.EndDate,
                    ImagePath = GetImageForDestination(trip.Destination)
                });
            }
            
            SearchResultsControl.ItemsSource = results;
        }
        
        private string GetDuration(DateTime start, DateTime end)
        {
            int days = (end - start).Days + 1;
            return days == 1 ? "1 day" : $"{days} days";
        }
        
        private string GetImageForDestination(string destination)
        {
            if (destination.Contains("Paris"))
                return "../Assets/paris.png";
            if (destination.Contains("Rome"))
                return "../Assets/rome.png";
            if (destination.Contains("Tokyo"))
                return "../Assets/tokyo.png";
            
            return "../Assets/default_destination.png";
        }
        
        private void LoadSearchResults()
        {
            var results = new ObservableCollection<TripSearchResult>
            {
                new TripSearchResult {
                    TripID = 1,
                    Name = "Paris Explorer",
                    Destination = "Paris, France",
                    Description = "Discover the City of Light with our guided tour...",
                    Duration = "5 days",
                    Price = "$1,299",
                    GroupSize = 15,
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(35),
                    ImagePath = "../Assets/paris.png"
                },
                new TripSearchResult {
                    TripID = 2,
                    Name = "Rome Adventure",
                    Destination = "Rome, Italy",
                    Description = "Walk through ancient history in the Eternal City...",
                    Duration = "7 days",
                    Price = "$1,599",
                    GroupSize = 10,
                    StartDate = DateTime.Now.AddDays(45),
                    EndDate = DateTime.Now.AddDays(52),
                    ImagePath = "../Assets/rome.png"
                }
            };
            
            SearchResultsControl.ItemsSource = results;
        }
        
        private async void OnBookButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TripSearchResult trip)
            {
                if (string.IsNullOrEmpty(_username))
                {
                    await ShowMessageDialog("Please log in to book a trip");
                    return;
                }
                var paymentPage = new TripPaymentPage(trip.TripID.ToString(), _username);
                
                if (this.Parent is ContentControl contentControl)
                {
                    contentControl.Content = paymentPage;
                }
            }
        }
        private async void OnWishlistButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TripSearchResult trip)
            {
                if (string.IsNullOrEmpty(_username))
                {
                    await ShowMessageDialog("Please log in to add to wishlist");
                    return;
                }
                
                try
                {
                    bool success = await _repository.AddToWishlist(_username, trip.TripID);
                    if (success)
                    {
                        await ShowMessageDialog($"Added to wishlist: {trip.Name}");
                    }
                    else
                    {
                        await ShowMessageDialog("Failed to add to wishlist. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    await ShowMessageDialog($"Error adding to wishlist: {ex.Message}");
                }
            }
        }
        
        private async Task ShowMessageDialog(string message)
        {
            var dialog = new Window
            {
                Width = 300,
                Height = 150,
                Title = "TravelEase",
                Content = new TextBlock
                {
                    Text = message,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Margin = new Avalonia.Thickness(20),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            };
            
            await dialog.ShowDialog((Window)this.VisualRoot);
        }
    }
    
    public class TripSearchResult
    {
        public int TripID { get; set; }
        public string Name { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        public int GroupSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImagePath { get; set; }
    }
}