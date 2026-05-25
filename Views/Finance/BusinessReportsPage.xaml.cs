using System;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Finance
{
    public partial class BusinessReportsPage : ContentPage
    {
        public BusinessReportsPage()
        {
            InitializeComponent();
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceDashboardPage");
        }

        private async void OnMonitorRevenueClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceMonitorRevenuePage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
