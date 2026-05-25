using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Customer
{
    public partial class FeedbackPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002;

        public FeedbackPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                var user = await _databaseService.GetAccountInfoByIdAsync(CurrentUserId);
                if (user != null)
                {
                    ProfileNameLabel.Text = user.AccountName;
                    ProfileInitialsLabel.Text = user.AccountName?.Length >= 2 ? user.AccountName.Substring(0, 2).ToUpper() : "U";
                    ProfileRoleLabel.Text = "Artisanal Collector";
                }
            }
            catch { }
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomerDashboard");
        }

        private async void OnExploreTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//BrowseMarketplacePage");
        }

        private async void OnCustomOrdersTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrdersPage");
        }

        private async void OnTrackOrdersTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TrackOrdersPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MessagesPage");
        }

        private async void OnRewardsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RewardsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
