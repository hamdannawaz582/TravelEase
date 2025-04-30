using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Windows.Input;


//  To use Cards in your page
//  1) Add the following in your axaml file
//       <DockPanel>
//          <Button DockPanel.Dock="Top" 
//                  Content="Create Card" 
//                  Click="OnCreateCardClick"
//                  Margin="15" 
//                  Height="30" 
//                  Background="Beige" 
//                  Foreground="Black" />
//          <ScrollViewer>
//              <StackPanel x:Name="CardsContainer" Margin="10" Spacing="10" />
//          </ScrollViewer>
//      </DockPanel>
//  2) Add this function in your axaml.cs file
//      private void OnCreateCardClick(object sender, RoutedEventArgs e)
//          {
//              var card = CardControl.Create(CardsContainer);
//          }

namespace DB_Project
{
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => TitleText.Text;
            set => TitleText.Text = value;
        }

        public string Description
        {
            get => DescriptionText.Text;
            set => DescriptionText.Text = value;
        }

        public void SetCloseAction(Panel container)
        {
            CloseButton.Click += (s, e) => container.Children.Remove(this);
        }
        
        public static CardControl Create(Panel container)
        {
            var card = new CardControl();
            string title = null; 
            string description = null;
            if (title != null) card.Title = title;
            if (description != null) card.Description = description;
            // Set up close functionality
            card.CloseButton.Click += (s, e) => 
            {
                container.Children.Remove(card);
            };
    
            // Add to container
            container.Children.Add(card);
            return card;
        }
    }
}