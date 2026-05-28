using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesCrmPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public SalesCrmPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCrmData();
        }

        private async Task LoadCrmData()
        {
            try
            {
                var customers = await _databaseService.GetCustomersAsync();
                
                TotalCustomersLabel.Text = (customers?.Count ?? 0).ToString("N0");

                BindableLayout.SetItemsSource(CustomersLayout, customers?.Take(10).ToList() ?? new List<User>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading CRM data: " + ex.Message);
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

        private async void OnPerformanceClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesPerformancePage");
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


