using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace DB_Project.AdminPages
{
    public partial class CategoryManagement : UserControl
    {
        public ObservableCollection<Category> Categories { get; set; }

        public CategoryManagement()
        {
            InitializeComponent();
            DataContext = this;
            LoadCategories();
            CategoriesContainer.ItemsSource = Categories;
        }

        private void LoadCategories()
        {
            Categories = new ObservableCollection<Category>
            {
                new Category { Name = "Adventure Tours" },
                new Category { Name = "Luxury Vacations" },
                new Category { Name = "Business Travel" }
            };
        }

        private void OnAddCategoryClick(object sender, RoutedEventArgs e)
        {
            var newCategory = new Category { Name = "New Category" };
            Categories.Add(newCategory);
            CategoriesContainer.ScrollIntoView(newCategory);
        }

        private void OnAddSubcategoryClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnDeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                Categories.Remove(category);
            }
        }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}