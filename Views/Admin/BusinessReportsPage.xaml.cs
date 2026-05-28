using Microsoft.Maui.Controls;
using CreatiSphere.Services;
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
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username) ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
            await LoadReportStats();
            await LoadChartData();
        }

        private async Task LoadReportStats()
        {
            try
            {
                if (UserSession.AccountID > 5)
                {
                    TotalRevenueLabel.Text = "₱0.00";
                    NewCreatorsLabel.Text = "0";
                    TotalOrdersLabel.Text = "0";
                    ConversionRateLabel.Text = "0.0%";
                    return;
                }

                var stats = await _dbService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C2", new System.Globalization.CultureInfo("en-PH"));
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
                if (UserSession.AccountID > 5)
                {
                    BindableLayout.SetItemsSource(ChartFlexLayout, new object[0]);
                    return;
                }

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

