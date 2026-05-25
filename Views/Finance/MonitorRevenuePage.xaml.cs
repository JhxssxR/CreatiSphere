using System;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Finance
{
    public partial class MonitorRevenuePage : ContentPage
    {
        public MonitorRevenuePage()
        {
            InitializeComponent();
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceDashboardPage");
        }

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceBusinessReportsPage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
