using Microsoft.Maui.Controls;
using System;

namespace CreatiSphere.Views.Admin
{
    public partial class ManageProductsPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        private List<Services.Product> _allProducts = new();
        private Services.Product? _selectedProduct;

        public ManageProductsPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _dbService.GetProductsAsync();
                _allProducts = products;
                BindableLayout.SetItemsSource(ProductsList, products);
                BindableLayout.SetItemsSource(ProductsGrid, products);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load products: " + ex.Message, "OK");
            }
        }

        private void OnAddProductClicked(object? sender, EventArgs e)
        {
            _selectedProduct = null;
            ModalTitle.Text = "Add New Product";
            ClearForm();
            ProductModal.IsVisible = true;
        }

        private void OnEditProductClicked(object? sender, EventArgs e)
        {
            Services.Product? product = null;
            if (sender is BindableObject bo && bo.BindingContext is Services.Product p1)
                product = p1;
            else if (sender is Microsoft.Maui.Controls.Shapes.Path path && path.GestureRecognizers.Count > 0 && path.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is Services.Product p2)
                product = p2;
            else if (e is TappedEventArgs te && te.Parameter is Services.Product p3)
                product = p3;

            if (product != null)
            {
                _selectedProduct = product;
                ModalTitle.Text = "Edit Product";
                PopulateForm(product);
                ProductModal.IsVisible = true;
            }
            else
            {
                // For hardcoded cards, we'll just show the modal for now
                ModalTitle.Text = "Edit Catalog Item";
                ProductModal.IsVisible = true;
            }
        }

        private void OnDeleteProductClicked(object? sender, EventArgs e)
        {
            if (sender is BindableObject bo && bo.BindingContext is Services.Product product)
            {
                _selectedProduct = product;
            }
            else if (sender is Microsoft.Maui.Controls.Shapes.Path path && path.GestureRecognizers.Count > 0 && path.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is Services.Product p)
            {
                _selectedProduct = p;
            }
            ConfirmationModal.IsVisible = true;
        }

        private void OnViewDetailsClicked(object? sender, EventArgs e)
        {
            Services.Product? product = null;
            if (sender is BindableObject bo && bo.BindingContext is Services.Product p1)
                product = p1;
            else if (sender is Microsoft.Maui.Controls.Shapes.Path path && path.GestureRecognizers.Count > 0 && path.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is Services.Product p2)
                product = p2;

            if (product != null)
            {
                _selectedProduct = product;
                DetailName.Text = product.Name;
                DetailCategory.Text = product.Category;
                DetailPrice.Text = product.Price.ToString("C");
                DetailStock.Text = $"{product.Stock} Units";
                DetailID.Text = $"#{product.ProductID}";
                DetailStatus.Text = product.Status;
                DetailImage.Source = product.ImageUrl;
                DetailDescription.Text = !string.IsNullOrWhiteSpace(product.Description) 
                    ? product.Description 
                    : "No description provided for this product.";
                DetailsModal.IsVisible = true;
            }
        }

        private void OnCloseDetailsClicked(object? sender, EventArgs e)
        {
            DetailsModal.IsVisible = false;
        }

        private void OnEditFromDetailsClicked(object? sender, EventArgs e)
        {
            DetailsModal.IsVisible = false;
            if (_selectedProduct != null)
            {
                ModalTitle.Text = "Edit Product";
                PopulateForm(_selectedProduct);
                ProductModal.IsVisible = true;
            }
        }

        private void OnCancelClicked(object? sender, EventArgs e)
        {
            ProductModal.IsVisible = false;
        }

        private void OnCancelConfirmClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
        }

        private async void OnFinalConfirmClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
            if (_selectedProduct != null)
            {
                bool success = await _dbService.DeleteProductAsync(_selectedProduct.ProductID ?? "");
                if (success)
                {
                    await LoadProducts();
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to delete product.", "OK");
                }
            }
        }

        private async void OnUploadImageClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a product image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // For this demo, we'll use the file path. 
                    // In a real app, you might upload this to a server or copy it to local storage.
                    ProductImageUrlEntry.Text = result.FullPath;
                    ProductImagePreview.Source = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Could not pick image: {ex.Message}", "OK");
            }
        }

        private void OnProductImageUrlChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                string url = e.NewTextValue?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(url))
                {
                    ProductImagePreview.Source = "product_placeholder.png";
                    return;
                }

                if (url.Contains(":\\") || url.StartsWith("/") || url.StartsWith("C:"))
                {
                    // Local file path
                    ProductImagePreview.Source = ImageSource.FromFile(url);
                }
                else
                {
                    // URL or Resource
                    ProductImagePreview.Source = url;
                }
            }
            catch
            {
                ProductImagePreview.Source = "product_placeholder.png";
            }
        }

        private async void OnSaveProductClicked(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProductNameEntry.Text) || 
                    ProductCategoryPicker.SelectedItem == null ||
                    !decimal.TryParse(ProductPriceEntry.Text, out decimal price) ||
                    !int.TryParse(ProductStockEntry.Text, out int stock))
                {
                    await DisplayAlertAsync("Validation", "Please fill in all required fields correctly (Name, Category, Price, and Stock).", "OK");
                    return;
                }

                SaveProductBtn.IsEnabled = false;

                var product = new Services.Product
                {
                    ProductID = _selectedProduct?.ProductID,
                    Name = ProductNameEntry.Text,
                    Category = ProductCategoryPicker.SelectedItem.ToString(),
                    Price = price,
                    Stock = stock,
                    Status = stock > 0 ? "In Stock" : "Out of Stock",
                    ImageUrl = string.IsNullOrWhiteSpace(ProductImageUrlEntry.Text) ? "product_placeholder.png" : ProductImageUrlEntry.Text,
                    Description = ProductDescriptionEditor.Text
                };

                bool success;
                bool isNew = (_selectedProduct == null);
                
                if (isNew)
                    success = await _dbService.AddProductAsync(product);
                else
                    success = await _dbService.UpdateProductAsync(product);

                if (success)
                {
                    ProductModal.IsVisible = false;
                    await LoadProducts();
                }
                else
                {
                    await DisplayAlertAsync("Database Error", "The database rejected the update. This usually happens if the connection is lost or the table structure doesn't match.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("System Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
            finally
            {
                SaveProductBtn.IsEnabled = true;
            }
        }

        private void ClearForm()
        {
            ProductNameEntry.Text = "";
            ProductCategoryPicker.SelectedIndex = -1;
            ProductPriceEntry.Text = "";
            ProductStockEntry.Text = "";
            ProductImageUrlEntry.Text = "";
            ProductDescriptionEditor.Text = "";
            ProductImagePreview.Source = "product_placeholder.png";
        }

        private void PopulateForm(Services.Product product)
        {
            ProductNameEntry.Text = product.Name;
            ProductCategoryPicker.SelectedItem = product.Category;
            ProductPriceEntry.Text = product.Price.ToString();
            ProductStockEntry.Text = product.Stock.ToString();
            ProductImageUrlEntry.Text = product.ImageUrl;
            ProductDescriptionEditor.Text = product.Description;
            ProductImagePreview.Source = string.IsNullOrWhiteSpace(product.ImageUrl) ? "product_placeholder.png" : product.ImageUrl;
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//AdminDashboard", false);
        }

        private async void OnManageInventoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageInventoryPage", false);
        }

        private async void OnDigitalAssetManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage", false);
        }

        private async void OnManageProductsClicked(object? sender, TappedEventArgs e)
        {
            // Already on this page
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageUsersPage", false);
        }

        private async void OnCrmManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//CrmManagementPage", false);
        }

        private async void OnCustomOrderManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrderManagementPage", false);
        }

        private async void OnViewSalesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage", false);
        }

        private async void OnBusinessReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//BusinessReportsPage", false);
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }
    }
}

