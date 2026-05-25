using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesDashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public SalesDashboardPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDashboardStats();
        }

        private async Task LoadDashboardStats()
        {
            try
            {
                var stats = await _databaseService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C0");
                
                // Assuming Active Leads comes from a different source, using orders as proxy for now
                ActiveLeadsLabel.Text = stats.TotalOrders.ToString("N0");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading sales dashboard: " + ex.Message);
            }
        }

        private async void OnCustomersClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCustomersPage");
        }

        private async void OnTransactionsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesTransactionsPage");
        }

        private async void OnPerformanceClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesPerformancePage");
        }

        private async void OnCrmClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCrmPage");
        }

        private async void OnReportsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesReportsPage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}


