using System;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Views.Creator
{
    public partial class CatalogManagerPage : ContentPage
    {
        public CatalogManagerPage()
        {
            InitializeComponent();
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorDashboardPage");
        }

        private async void OnAssetLibraryTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AssetLibraryPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
