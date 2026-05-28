using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesCustomersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public string TotalCustomers { get; set; } = "0";
        public string ActiveContracts { get; set; } = "0";
        public string AtRisk { get; set; } = "0";

        public SalesCustomersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCustomers();
        }

        private async Task LoadCustomers()
        {
            try
            {
                var customers = await _databaseService.GetCustomersAsync();
                
                int total = customers.Count;
                int inactive = customers.Count(c => c.Status?.Equals("Inactive", StringComparison.OrdinalIgnoreCase) == true);
                
                TotalCustomers = total.ToString("N0");
                ActiveContracts = total.ToString("N0"); // Active Contracts matches total customer count
                AtRisk = inactive.ToString("N0");

                OnPropertyChanged(nameof(TotalCustomers));
                OnPropertyChanged(nameof(ActiveContracts));
                OnPropertyChanged(nameof(AtRisk));

                CustomersList.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load customers: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesDashboardPage");

        private async void OnTransactionsClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesTransactionsPage");

        private async void OnPerformanceClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesPerformancePage");

        private async void OnCrmClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesCrmPage");

        private async void OnReportsClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesReportsPage");

        private async void OnLogoutClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");
    }
}
