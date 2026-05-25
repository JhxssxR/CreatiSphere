using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesTransactionsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public SalesTransactionsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTransactions();
        }

        private async Task LoadTransactions()
        {
            try
            {
                var transactions = await _databaseService.GetRecentTransactionsAsync();
                TransactionsList.ItemsSource = transactions;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load transactions: " + ex.Message, "OK");
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


