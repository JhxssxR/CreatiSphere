using Microsoft.Maui.Controls;
using CreatiSphere.Services;
namespace CreatiSphere.Views.Admin
{
    public partial class CrmManagementPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        public CrmManagementPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username) ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
            await LoadStats();
        }

        private async Task LoadStats()
        {
            try
            {
                if (UserSession.AccountID > 5)
                {
                    TotalCustomersLabel.Text = "0";
                    ActiveLeadsLabel.Text = "0";
                    TotalPortfolioSpendLabel.Text = "$0.0M";
                    return;
                }

                var stats = await _dbService.GetReportStatsAsync();
                TotalCustomersLabel.Text = stats.TotalCustomers.ToString("N0");
                ActiveLeadsLabel.Text = stats.ActiveLeads.ToString("N0");
                TotalPortfolioSpendLabel.Text = (stats.TotalPortfolioSpend / 1000000m).ToString("C1") + "M";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
        }

        private async void OnFilterClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Filter", "CRM filtering options will be available soon.", "OK");
        }

        private async void OnAddCustomerClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Add Customer", "New customer registration form opened.", "OK");
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
            // Already on this page
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
            bool answer = await DisplayAlertAsync("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }
    }
}

