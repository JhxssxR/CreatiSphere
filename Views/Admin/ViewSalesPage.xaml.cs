using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Collections.Generic;
using System.Linq;

namespace CreatiSphere.Views.Admin
{
    public partial class ViewSalesPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;
        private List<SaleTransaction> _allTransactions = new();

        public ViewSalesPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSalesData();
        }

        private async Task LoadSalesData()
        {
            try
            {
                // Load Transactions
                _allTransactions = await _dbService.GetRecentTransactionsAsync();
                BindableLayout.SetItemsSource(TransactionsList, _allTransactions);

                // Load Stats for summary cards
                var stats = await _dbService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C0");
                var today = DateTime.Today;
                var todaysTransactions = _allTransactions.Where(t => DateTime.TryParse(t.Date, out var parsed) && parsed.Date == today).ToList();
                var todayTotal = todaysTransactions.Sum(t => t.Amount);
                TodaySalesLabel.Text = todayTotal.ToString("C0");
                TodayTransactionsLabel.Text = $"{todaysTransactions.Count} transactions today";
                AvgOrderValueLabel.Text = (stats.TotalRevenue / Math.Max(1, stats.TotalOrders)).ToString("C0");
                ConversionRateLabel.Text = stats.ConversionRate.ToString("F1") + "%";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load sales data: " + ex.Message, "OK");
            }
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            string searchTerm = e.NewTextValue?.ToLower() ?? "";
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                BindableLayout.SetItemsSource(TransactionsList, _allTransactions);
                return;
            }

            var filtered = _allTransactions.Where(t => 
                (t.TransactionID?.ToLower()?.Contains(searchTerm) ?? false) || 
                (t.CustomerName?.ToLower()?.Contains(searchTerm) ?? false) ||
                (t.CustomerEmail?.ToLower()?.Contains(searchTerm) ?? false) ||
                (t.Status?.ToLower()?.Contains(searchTerm) ?? false)
            ).ToList();

            BindableLayout.SetItemsSource(TransactionsList, filtered);
        }

        private async void OnFiltersClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Filters", "Filtering options will appear here.", "OK");
        }

        private async void OnExportClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Export", "Sales data exported to CSV successfully.", "OK");
        }

        private async void OnPageClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                await DisplayAlertAsync("Pagination", $"Navigating to page {btn.Text}", "OK");
            }
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
            await Shell.Current.GoToAsync("//CustomOrderManagementPage", false);
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

