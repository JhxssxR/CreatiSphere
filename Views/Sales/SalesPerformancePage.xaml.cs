using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesPerformancePage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public SalesPerformancePage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPerformanceData();
        }

        private async Task LoadPerformanceData()
        {
            try
            {
                var stats = await _databaseService.GetReportStatsAsync();
                MonthlyRevenueLabel.Text = stats.TotalRevenue.ToString("C0");

                var customers = await _databaseService.GetCustomersAsync();
                NewCustomersLabel.Text = (customers?.Count ?? 0).ToString("N0");

                var transactions = await _databaseService.GetRecentTransactionsAsync();
                BindableLayout.SetItemsSource(RecentActivityLayout, transactions?.Take(5).ToList() ?? new List<SaleTransaction>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading sales performance data: " + ex.Message);
            }
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesDashboardPage");
        }

        private async void OnCustomersClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCustomersPage");
        }

        private async void OnTransactionsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesTransactionsPage");
        }

        private async void OnCrmClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCrmPage");
        }

        private async void OnReportsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesReportsPage");
        }
        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}


