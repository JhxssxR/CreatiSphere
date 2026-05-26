using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;

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
            ApplyTierRestrictions();
        }

        public void ApplyTierRestrictions()
        {
            // Reset to default (unlocked) state
            ViewSalesItem.Opacity = 1.0;
            CustomOrderManagementItem.Opacity = 1.0;
            DigitalAssetManagementItem.Opacity = 1.0;
            CrmManagementItem.Opacity = 1.0;
            BusinessReportsItem.Opacity = 1.0;

            if (UserSession.Tier == "Starter")
            {
                ViewSalesItem.Opacity = 0.5;
                CustomOrderManagementItem.Opacity = 0.5;
                DigitalAssetManagementItem.Opacity = 0.5;
                CrmManagementItem.Opacity = 0.5;
                BusinessReportsItem.Opacity = 0.5;
            }
            else if (UserSession.Tier == "Standard")
            {
                DigitalAssetManagementItem.Opacity = 0.5;
                BusinessReportsItem.Opacity = 0.5;
            }
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
            if (UserSession.Tier == "Starter")
            {
                await Shell.Current.DisplayAlert("Premium Feature", "View Sales is not available on the Starter tier. Please upgrade to a higher tier to unlock this feature.", "OK");
                return;
            }
            await Shell.Current.GoToAsync("//ViewSalesPage");
        }

        private async void OnCustomOrderManagementClicked(object? sender, EventArgs e)
        {
            if (UserSession.Tier == "Starter")
            {
                await Shell.Current.DisplayAlert("Premium Feature", "Custom Order Management is not available on the Starter tier. Please upgrade to a higher tier to unlock this feature.", "OK");
                return;
            }
            await Shell.Current.GoToAsync("//CustomOrderManagementPage");
        }

        private async void OnDigitalAssetManagementClicked(object? sender, EventArgs e)
        {
            if (UserSession.Tier == "Starter" || UserSession.Tier == "Standard")
            {
                await Shell.Current.DisplayAlert("Premium Feature", "Digital Asset Management is only available on the Enterprise Plus tier. Please upgrade to unlock this feature.", "OK");
                return;
            }
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage");
        }

        private async void OnCrmManagementClicked(object? sender, EventArgs e)
        {
            if (UserSession.Tier == "Starter")
            {
                await Shell.Current.DisplayAlert("Premium Feature", "CRM Management is not available on the Starter tier. Please upgrade to a higher tier to unlock this feature.", "OK");
                return;
            }
            await Shell.Current.GoToAsync("//CrmManagementPage");
        }

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
        {
            if (UserSession.Tier == "Starter" || UserSession.Tier == "Standard")
            {
                await Shell.Current.DisplayAlert("Premium Feature", "Business Reports are only available on the Enterprise Plus tier. Please upgrade to unlock this feature.", "OK");
                return;
            }
            await Shell.Current.GoToAsync("//BusinessReportsPage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
