using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;
using System.Linq;

namespace CreatiSphere.Views.Creator
{
    public partial class CreatorDashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 2002; // Mocked for demonstration

        public CreatorDashboardPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                var assets = await _databaseService.GetCreatorAssetsAsync(CurrentUserId);
                ViewsLabel.Text = assets.Sum(a => a.Views).ToString("N0");
                SalesLabel.Text = assets.Sum(a => a.Sales).ToString("N0");
                EarningsLabel.Text = assets.Sum(a => a.Price * a.Sales).ToString("C");

                var commissions = await _databaseService.GetCommissionsAsync(CurrentUserId, isCreator: true);
                NewCommissionsLabel.Text = commissions.Count.ToString();
                CommissionsCollectionView.ItemsSource = commissions;

                PortfolioCollectionView.ItemsSource = assets.Take(3).ToList();

                // Welcome message
                WelcomeLabel.Text = "Welcome back, Creator.";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load dashboard: {ex.Message}", "OK");
            }
        }

        private async void OnAssetLibraryTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AssetLibraryPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnCatalogManagerTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatalogManagerPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
