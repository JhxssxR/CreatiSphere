using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Creator
{
    public partial class CatalogManagerPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private int _currentPage = 1;
        private int _pageSize = 4;
        private string _currentCategory = "All Products";
        private int _totalPages = 1;
        private Product? _editingProduct = null;
        private string? _selectedImageUrl = null;

        public CatalogManagerPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load Stats
                var stats = await _dbService.GetCatalogStatsAsync();
                TotalProductsLabel.Text = stats.Total.ToString("D2");
                ActiveListingsLabel.Text = stats.Active.ToString("D2");
                LowStockLabel.Text = stats.LowStock.ToString("D2");
                AvgPriceLabel.Text = "₱" + stats.AvgPrice.ToString("N0");

                // Load Products
                var (items, totalCount) = await _dbService.GetProductsAsync(_currentPage, _pageSize, _currentCategory);
                BindableLayout.SetItemsSource(ProductsListContainer, items);

                _totalPages = (int)Math.Ceiling(totalCount / (double)_pageSize);
                if (_totalPages == 0) _totalPages = 1;
                CurrentPageLabel.Text = _currentPage.ToString();
                TotalPagesLabel.Text = _totalPages.ToString();

                UpdateTabStyles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading catalog data: {ex}");
            }
        }

        private void UpdateTabStyles()
        {
            // Reset all
            TabAllBorder.BackgroundColor = Colors.Transparent;
            TabAllLabel.TextColor = Color.FromArgb("#64748B");
            TabPhysicalBorder.BackgroundColor = Colors.Transparent;
            TabPhysicalLabel.TextColor = Color.FromArgb("#64748B");
            TabDigitalBorder.BackgroundColor = Colors.Transparent;
            TabDigitalLabel.TextColor = Color.FromArgb("#64748B");

            // Set active
            if (_currentCategory == "All Products")
            {
                TabAllBorder.BackgroundColor = Color.FromArgb("#DBEAFE");
                TabAllLabel.TextColor = Color.FromArgb("#1E40AF");
            }
            else if (_currentCategory == "Physical Prints")
            {
                TabPhysicalBorder.BackgroundColor = Color.FromArgb("#DBEAFE");
                TabPhysicalLabel.TextColor = Color.FromArgb("#1E40AF");
            }
            else if (_currentCategory == "Digital Assets")
            {
                TabDigitalBorder.BackgroundColor = Color.FromArgb("#DBEAFE");
                TabDigitalLabel.TextColor = Color.FromArgb("#1E40AF");
            }
        }

        private async void OnFilterTapped(object? sender, EventArgs e)
        {
            if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is string category)
            {
                _currentCategory = category;
                _currentPage = 1;
                await LoadDataAsync();
            }
        }

        private async void OnPrevPageTapped(object? sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadDataAsync();
            }
        }

        private async void OnNextPageTapped(object? sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadDataAsync();
            }
        }

        // Modal Methods
        private void OnAddProductClicked(object? sender, EventArgs e)
        {
            _editingProduct = null;
            _selectedImageUrl = null;
            ProductImageUrlLabel.Text = "No image selected";
            ModalTitleLabel.Text = "Add New Product";
            ProductNameEntry.Text = "";
            ProductCategoryPicker.SelectedIndex = 0;
            ProductPriceEntry.Text = "";
            ProductStockEntry.Text = "";
            ProductStatusPicker.SelectedIndex = 0;
            
            ProductModal.IsVisible = true;
        }

        private void OnEditProductTapped(object? sender, EventArgs e)
        {
            if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is Product p)
            {
                _editingProduct = p;
                _selectedImageUrl = p.ImageUrl;
                ProductImageUrlLabel.Text = string.IsNullOrEmpty(p.ImageUrl) ? "No image selected" : System.IO.Path.GetFileName(p.ImageUrl);
                ModalTitleLabel.Text = "Edit Product";
                ProductNameEntry.Text = p.Name;
                ProductCategoryPicker.SelectedItem = p.Category;
                ProductPriceEntry.Text = p.Price.ToString("0.##");
                ProductStockEntry.Text = p.Stock.ToString();
                ProductStatusPicker.SelectedItem = p.Status;
                
                ProductModal.IsVisible = true;
            }
        }

        private async void OnViewProductTapped(object? sender, EventArgs e)
        {
            if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is Product p)
            {
                await DisplayAlert("Product Details", 
                    $"Name: {p.Name}\n" +
                    $"SKU: {p.ProductID}\n" +
                    $"Category: {p.Category}\n" +
                    $"Price: ₱{p.Price:F2}\n" +
                    $"Stock: {p.Stock}\n" +
                    $"Status: {p.Status}", "Close");
            }
        }

        private async void OnChooseProductImageClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Product Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    var localFolder = FileSystem.AppDataDirectory;
                    var localPath = System.IO.Path.Combine(localFolder, result.FileName);

                    using (var stream = await result.OpenReadAsync())
                    using (var localStream = System.IO.File.OpenWrite(localPath))
                    {
                        await stream.CopyToAsync(localStream);
                    }

                    _selectedImageUrl = localPath;
                    ProductImageUrlLabel.Text = result.FileName;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteProductTapped(object? sender, EventArgs e)
        {
            if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is Product p)
            {
                bool answer = await DisplayAlert("Delete Product", $"Are you sure you want to delete {p.Name}?", "Yes", "No");
                if (answer && !string.IsNullOrEmpty(p.ProductID))
                {
                    await _dbService.DeleteProductAsync(p.ProductID);
                    await LoadDataAsync();
                }
            }
        }

        private void OnCloseModalClicked(object? sender, EventArgs e)
        {
            ProductModal.IsVisible = false;
        }

        private async void OnSaveProductClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameEntry.Text))
            {
                await DisplayAlert("Validation", "Product Name is required.", "OK");
                return;
            }

            decimal.TryParse(ProductPriceEntry.Text, out decimal price);
            int.TryParse(ProductStockEntry.Text, out int stock);

            Product product = _editingProduct ?? new Product();
            product.Name = ProductNameEntry.Text;
            product.Category = ProductCategoryPicker.SelectedItem?.ToString() ?? "Physical Prints";
            product.Price = price;
            product.Stock = stock;
            product.Status = ProductStatusPicker.SelectedItem?.ToString() ?? "In Stock";
            product.ImageUrl = string.IsNullOrEmpty(_selectedImageUrl) ? "https://via.placeholder.com/150" : _selectedImageUrl;

            if (_editingProduct == null)
            {
                await _dbService.AddProductAsync(product);
            }
            else
            {
                await _dbService.UpdateProductAsync(product);
            }

            ProductModal.IsVisible = false;
            await LoadDataAsync();
        }

        // Navigation
        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorDashboardPage");
        }

        private async void OnAssetLibraryTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AssetLibraryPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorMessagesPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
