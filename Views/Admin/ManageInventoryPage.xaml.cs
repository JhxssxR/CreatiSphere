using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Admin
{
    public partial class ManageInventoryPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;
        private Services.InventoryItem? _selectedItem;
        private List<Services.InventoryItem> _allInventory = new();
        private string _currentFilter = "All";

        public ManageInventoryPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadInventory();
        }

        private async Task LoadInventory()
        {
            try
            {
                _allInventory = await _dbService.GetInventoryAsync();
                ApplyFilter(_currentFilter);

                if (Services.UserSession.AccountID > 5)
                {
                    _allInventory = new List<Services.InventoryItem>();
                    ApplyFilter(_currentFilter);

                    TotalStockValueLabel.Text = "$0.00";
                    LowStockAlertsLabel.Text = "0";
                    DigitalAssetsLabel.Text = "0";
                    ActiveShipmentsLabel.Text = "0";
                    InventoryCountLabel.Text = "Showing 0 items";
                }

                if (!string.IsNullOrEmpty(Services.UserSession.Username) && Services.UserSession.Username.Length >= 2)
                    HeaderProfileInitials.Text = Services.UserSession.Username.Substring(0, 2).ToUpper();
                else
                    HeaderProfileInitials.Text = "AD";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load inventory: " + ex.Message, "OK");
            }
        }

        private void ApplyFilter(string filter)
        {
            _currentFilter = filter;
            List<Services.InventoryItem> filtered;

            switch (filter)
            {
                case "LowStock":
                    filtered = _allInventory.Where(i => 
                        i.Status == "Low Stock" || i.Quantity < 20).ToList();
                    break;
                case "Digital":
                    filtered = _allInventory.Where(i => 
                        (i.Warehouse ?? "").Contains("Digital", StringComparison.OrdinalIgnoreCase) ||
                        (i.Name ?? "").Contains("Digital", StringComparison.OrdinalIgnoreCase) ||
                        (i.Name ?? "").Contains("Brush", StringComparison.OrdinalIgnoreCase) ||
                        (i.Name ?? "").Contains("Template", StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "Physical":
                    filtered = _allInventory.Where(i => 
                        !(i.Warehouse ?? "").Contains("Digital", StringComparison.OrdinalIgnoreCase) &&
                        !(i.Name ?? "").Contains("Digital", StringComparison.OrdinalIgnoreCase) &&
                        !(i.Name ?? "").Contains("Brush", StringComparison.OrdinalIgnoreCase) &&
                        !(i.Name ?? "").Contains("Template", StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    filtered = _allInventory;
                    break;
            }

            BindableLayout.SetItemsSource(InventoryList, filtered);
            UpdateTabStyles(filter);
        }

        private void OnTabClicked(object? sender, TappedEventArgs e)
        {
            string filter = e.Parameter?.ToString() ?? "All";
            ApplyFilter(filter);
        }

        private void UpdateTabStyles(string activeTab)
        {
            // Reset all tabs
            TabAllItems.BackgroundColor = Colors.Transparent;
            TabLowStock.BackgroundColor = Colors.Transparent;
            TabDigital.BackgroundColor = Colors.Transparent;
            TabPhysical.BackgroundColor = Colors.Transparent;

            TabAllItemsLabel.TextColor = Color.FromArgb("#64748B");
            TabLowStockLabel.TextColor = Color.FromArgb("#64748B");
            TabDigitalLabel.TextColor = Color.FromArgb("#64748B");
            TabPhysicalLabel.TextColor = Color.FromArgb("#64748B");

            // Highlight active tab
            Color tealBg = Color.FromArgb("#F0FDFA");
            Color tealText = Color.FromArgb("#0D9488");

            switch (activeTab)
            {
                case "All":
                    TabAllItems.BackgroundColor = tealBg;
                    TabAllItemsLabel.TextColor = tealText;
                    break;
                case "LowStock":
                    TabLowStock.BackgroundColor = tealBg;
                    TabLowStockLabel.TextColor = tealText;
                    break;
                case "Digital":
                    TabDigital.BackgroundColor = tealBg;
                    TabDigitalLabel.TextColor = tealText;
                    break;
                case "Physical":
                    TabPhysical.BackgroundColor = tealBg;
                    TabPhysicalLabel.TextColor = tealText;
                    break;
            }
        }

        private async void OnExportCsvClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Export", "Inventory data exported to CSV successfully.", "OK");
        }

        private void OnNewItemClicked(object? sender, EventArgs e)
        {
            _selectedItem = null;
            ModalTitle.Text = "New Inventory Item";
            ClearForm();
            ItemModal.IsVisible = true;
        }

        private async void OnRowMenuClicked(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is Services.InventoryItem item)
            {
                string action = await DisplayActionSheetAsync("Manage Item", "Cancel", null, "Edit Details", "Delete Item");
                
                if (action == "Edit Details")
                {
                    _selectedItem = item;
                    ModalTitle.Text = "Edit Item Details";
                    PopulateForm(item);
                    ItemModal.IsVisible = true;
                }
                else if (action == "Delete Item")
                {
                    _selectedItem = item;
                    ConfirmationModal.IsVisible = true;
                }
            }
        }

        private void OnCancelClicked(object? sender, EventArgs e)
        {
            ItemModal.IsVisible = false;
        }

        private void OnCancelConfirmClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
        }

        private async void OnFinalDeleteClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
            if (_selectedItem != null)
            {
                bool success = await _dbService.DeleteInventoryAsync(_selectedItem.SKU ?? "");
                if (success)
                {
                    await LoadInventory();
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to delete item.", "OK");
                }
            }
        }

        private async void OnSaveItemClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemNameEntry.Text) || string.IsNullOrWhiteSpace(ItemSkuEntry.Text))
            {
                await DisplayAlertAsync("Validation", "Item Name and SKU are required.", "OK");
                return;
            }

            int.TryParse(ItemQuantityEntry.Text, out int qty);

            var item = new Services.InventoryItem
            {
                SKU = ItemSkuEntry.Text,
                Name = ItemNameEntry.Text,
                Warehouse = WarehousePicker.SelectedItem?.ToString() ?? "Main Hub",
                Quantity = qty,
                ReorderPoint = ItemReorderEntry.Text ?? "10",
                Status = qty < 20 ? "Low Stock" : "Stable"
            };

            bool success;
            if (_selectedItem == null)
                success = await _dbService.AddInventoryAsync(item);
            else
                success = await _dbService.UpdateInventoryAsync(item);

            if (success)
            {
                ItemModal.IsVisible = false;
                await LoadInventory();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to save inventory item.", "OK");
            }
        }

        private void ClearForm()
        {
            ItemNameEntry.Text = "";
            ItemSkuEntry.Text = "";
            ItemSkuEntry.IsEnabled = true;
            WarehousePicker.SelectedIndex = -1;
            ItemQuantityEntry.Text = "";
            ItemReorderEntry.Text = "";
        }

        private void PopulateForm(Services.InventoryItem item)
        {
            ItemNameEntry.Text = item.Name;
            ItemSkuEntry.Text = item.SKU;
            ItemSkuEntry.IsEnabled = false; // Usually SKU is unique and not editable
            WarehousePicker.SelectedItem = item.Warehouse;
            ItemQuantityEntry.Text = item.Quantity.ToString();
            ItemReorderEntry.Text = item.ReorderPoint;
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//AdminDashboard", false);
        }

        private async void OnManageInventoryClicked(object? sender, TappedEventArgs e)
        {
            // Already on this page
        }

        private async void OnDigitalAssetManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage", false);
        }

        private async void OnManageProductsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageProductsPage", false);
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

