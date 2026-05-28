using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
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
                await _databaseService.RemoveDuplicateTransactionsAsync();

                var stats = await _databaseService.GetReportStatsAsync();
                TotalRevenueLabel.Text = $"₱{stats.TotalRevenue:N0}";
                TransactionsLabel.Text = stats.TotalOrders.ToString("N0");
                
                var customers = await _databaseService.GetCustomersAsync();
                CustomersHandledLabel.Text = (customers?.Count ?? 0).ToString("N0");

                ConversionRateLabel.Text = "68%"; // Mock metric since no clear Conversion Rate in DB

                var transactions = await _databaseService.GetRecentTransactionsAsync();
                BindableLayout.SetItemsSource(RecentInteractionsLayout, transactions?.Take(4).ToList() ?? new List<SaleTransaction>());

                var products = await _databaseService.GetProductsAsync();
                BindableLayout.SetItemsSource(TopProductsLayout, products?.OrderByDescending(p => p.Price).Take(3).ToList() ?? new List<Product>());
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


