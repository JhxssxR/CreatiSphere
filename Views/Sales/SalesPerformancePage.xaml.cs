using Microsoft.Maui.Controls;
using System;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesPerformancePage : ContentPage
    {
        public SalesPerformancePage()
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


