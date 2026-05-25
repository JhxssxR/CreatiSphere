using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesCustomersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public SalesCustomersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
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
                CustomersList.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load customers: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesDashboardPage");
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
        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}


