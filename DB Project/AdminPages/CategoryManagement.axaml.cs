using Avalonia.Controls;
using DB_Project.Services;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System;

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
            Categories = new ObservableCollection<Category>();

            string query = "SELECT Type FROM Categories";

            try
            {
                using (var connection = DatabaseService.Instance.CreateConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Categories.Add(new Category
                            {
                                Name = reader["Type"].ToString()
                            });
                        }
                    }
                }

                Console.WriteLine("Categories loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }

        }

        private void OnAddCategoryClick(object sender, RoutedEventArgs e)
        {
            var newCategory = new Category { Name = "New Category" };
            Categories.Add(newCategory);
            CategoriesContainer.ScrollIntoView(newCategory);
        }

        private void OnAddSubcategoryClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                string newCategoryName = category.Name?.Trim();

                if (string.IsNullOrWhiteSpace(newCategoryName))
                {
                    Console.WriteLine("Category name cannot be empty.");
                    return;
                }

                string insertQuery = "INSERT INTO Categories (Type) VALUES (@Type)";

                try
                {
                    using (var connection = DatabaseService.Instance.CreateConnection())
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Type", newCategoryName);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Category '{newCategoryName}' added successfully to the database.");
                        }
                        else
                        {
                            Console.WriteLine("Insert failed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting category: {ex.Message}");
                }
            }

        }

        private void OnDeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                string categoryName = category.Name?.Trim();

                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    Console.WriteLine("Category name is empty. Cannot delete.");
                    return;
                }

                string deleteQuery = "DELETE FROM Categories WHERE Type = @Type";

                try
                {
                    using (var connection = DatabaseService.Instance.CreateConnection())
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Type", categoryName);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Category '{categoryName}' deleted from database.");
                            Categories.Remove(category);
                        }
                        else
                        {
                            Console.WriteLine($"No category named '{categoryName}' found in database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting category: {ex.Message}");
                }
            }
        }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}