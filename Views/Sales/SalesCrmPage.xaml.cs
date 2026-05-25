using Microsoft.Maui.Controls;
using System;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesCrmPage : ContentPage
    {
        public SalesCrmPage()
        {
            InitializeComponent();
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


