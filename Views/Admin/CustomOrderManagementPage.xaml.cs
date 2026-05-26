using Microsoft.Maui.Controls;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Admin
{
    public partial class CustomOrderManagementPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        public CustomOrderManagementPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username) ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                if (UserSession.AccountID > 5)
                {
                    BindableLayout.SetItemsSource(NewOrdersList, new List<CustomOrder>());
                    BindableLayout.SetItemsSource(InProgressOrdersList, new List<CustomOrder>());
                    BindableLayout.SetItemsSource(ReviewOrdersList, new List<CustomOrder>());
                    
                    NewRequestsCountLabel.Text = "0";
                    InProgressCountLabel.Text = "0";
                    ReviewCountLabel.Text = "0";
                    TotalVolumeLabel.Text = "$0";
                    return;
                }

                var orders = await _dbService.GetCustomOrdersAsync();
                
                var newOrders = orders.Where(o => o.Status == "New Request" || string.IsNullOrEmpty(o.Status)).ToList();
                var inProgressOrders = orders.Where(o => o.Status == "Sketching" || o.Status == "In Progress").ToList();
                var reviewOrders = orders.Where(o => o.Status == "Review").ToList();

                BindableLayout.SetItemsSource(NewOrdersList, newOrders);
                BindableLayout.SetItemsSource(InProgressOrdersList, inProgressOrders);
                BindableLayout.SetItemsSource(ReviewOrdersList, reviewOrders);

                NewRequestsCountLabel.Text = newOrders.Count.ToString();
                InProgressCountLabel.Text = inProgressOrders.Count.ToString();
                ReviewCountLabel.Text = reviewOrders.Count.ToString();

                // Fetch real stats
                var stats = await _dbService.GetReportStatsAsync();
                TotalVolumeLabel.Text = stats.TotalRevenue.ToString("C0");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load orders: " + ex.Message, "OK");
            }
        }

        private CustomOrder? _selectedOrder;

        private void OnOrderSelected(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is CustomOrder order)
            {
                _selectedOrder = order;
                DetailTitleLabel.Text = order.Title;
                DetailIdLabel.Text = $"#CO-{order.OrderID}";
                DetailPriorityLabel.Text = $"Priority: {order.Priority}";
                DetailCategoryLabel.Text = order.Category;
                DetailBriefLabel.Text = order.Description;
                DetailResolutionLabel.Text = order.Resolution;
                DetailColorProfileLabel.Text = order.ColorProfile;
                DetailDeliverablesLabel.Text = order.Deliverables;
                DetailArtistNameLabel.Text = !string.IsNullOrEmpty(order.AssignedArtist) ? order.AssignedArtist : "Unassigned";
            }
        }

        private void OnUpdateStatusClicked(object? sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                SelectionModal.IsVisible = true;
                return;
            }

            // Show modern modal
            var statusIndex = StatusPicker.Items.IndexOf(_selectedOrder.Status ?? "");
            StatusPicker.SelectedIndex = statusIndex >= 0 ? statusIndex : 0;
            StatusModal.IsVisible = true;
        }

        private void OnCloseModalClicked(object? sender, EventArgs e)
        {
            StatusModal.IsVisible = false;
        }

        private void OnCloseSelectionModalClicked(object? sender, EventArgs e)
        {
            SelectionModal.IsVisible = false;
        }

        private async void OnConfirmStatusUpdate(object? sender, EventArgs e)
        {
            if (_selectedOrder == null || StatusPicker.SelectedIndex < 0) return;

            string newStatus = StatusPicker.Items[StatusPicker.SelectedIndex];
            
            try
            {
                bool success = await _dbService.UpdateCustomOrderStatusAsync(_selectedOrder.OrderID ?? string.Empty, newStatus);
                
                if (success)
                {
                    _selectedOrder.Status = newStatus;
                    StatusModal.IsVisible = false;
                    await DisplayAlertAsync("Success", $"Order status updated to {newStatus}", "OK");
                    await LoadOrders(); // Refresh list
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to update status in database.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to update status: " + ex.Message, "OK");
            }
        }

        private async void OnNewOrderClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("New Order", "Custom order specification form opened.", "OK");
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
            // Already on this page
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
            bool answer = await DisplayAlertAsync("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }
    }
}

