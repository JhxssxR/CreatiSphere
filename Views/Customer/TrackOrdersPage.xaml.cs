using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Customer
{
    public partial class TrackOrdersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002;

        public TrackOrdersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
            await LoadCommissions();
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

        private async Task LoadCommissions()
        {
            try
            {
                var commissions = await _databaseService.GetCustomOrdersAsync();
                BindableLayout.SetItemsSource(CommissionsContainer, commissions);
            }
            catch (Exception ex)
            {
                await this.DisplayAlertAsync("Error", "Could not load commissions: " + ex.Message, "OK");
            }
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

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MessagesPage");
        }

        private async void OnFeedbackTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FeedbackPage");
        }

        private async void OnRewardsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RewardsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnViewDetailsClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var order = button?.CommandParameter as CustomOrder ?? (button?.BindingContext as CustomOrder);
            
            if (order != null)
            {
                OrderDetailsModal.BindingContext = order;
                OrderDetailsModal.IsVisible = true;
            }
        }

        private void OnCloseModalTapped(object? sender, TappedEventArgs e)
        {
            OrderDetailsModal.IsVisible = false;
        }
    }
}
