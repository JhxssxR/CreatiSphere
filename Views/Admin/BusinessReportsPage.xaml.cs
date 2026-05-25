using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Admin
{
    public partial class BusinessReportsPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        public BusinessReportsPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportStats();
            await LoadChartData();
        }

        private async Task LoadReportStats()
        {
            try
            {
                var stats = await _dbService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C");
                NewCreatorsLabel.Text = stats.NewCreators.ToString("N0");
                TotalOrdersLabel.Text = stats.TotalOrders.ToString("N0");
                ConversionRateLabel.Text = stats.ConversionRate.ToString("F2") + "%";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load report statistics: " + ex.Message, "OK");
            }
        }

        private async Task LoadChartData()
        {
            try
            {
                var chartData = await _dbService.GetWeeklyRevenueGrowthAsync();
                BindableLayout.SetItemsSource(ChartFlexLayout, chartData);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load chart data: " + ex.Message, "OK");
            }
        }

        private async void OnExportClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Export", "Report data exported to PDF successfully.", "OK");
        }

        private async void OnRefreshDataClicked(object? sender, EventArgs e)
        {
            await LoadReportStats();
            await LoadChartData();
            await DisplayAlertAsync("Refresh", "Analytics data has been updated.", "OK");
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

        private async void OnViewSalesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage", false);
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

