using System;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Admin
{
    public partial class AdminSidebar : ContentView
    {
        public static readonly BindableProperty ActivePageProperty =
            BindableProperty.Create(nameof(ActivePage), typeof(string), typeof(AdminSidebar), string.Empty);

        public string ActivePage
        {
            get => (string)GetValue(ActivePageProperty);
            set => SetValue(ActivePageProperty, value);
        }

        public AdminSidebar()
        {
            InitializeComponent();
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AdminDashboard");
        }

        private async void OnManageUsersClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageUsersPage");
        }

        private async void OnManageProductsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageProductsPage");
        }

        private async void OnManageInventoryClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageInventoryPage");
        }

        private async void OnViewSalesClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage");
        }

        private async void OnCustomOrderManagementClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrderManagementPage");
        }

        private async void OnDigitalAssetManagementClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage");
        }

        private async void OnCrmManagementClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CrmManagementPage");
        }

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//BusinessReportsPage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
