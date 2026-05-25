using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Customer
{
    public partial class RewardsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002;
        private const int NextTierThreshold = 3000;
        private Label? _pointsBalanceLabel;
        private Label? _tierNameLabel;
        private Label? _pointsToGoLabel;
        private ProgressBar? _tierProgressBar;
        private Label? _tierMessageLabel;
        private CollectionView? _rewardsCollectionView;
        private CollectionView? _historyCollectionView;

        public RewardsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindUiReferences();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
            await LoadRewardsAsync();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                var user = await _databaseService.GetAccountInfoByIdAsync(GetCurrentUserId());
                if (user != null)
                {
                    ProfileNameLabel.Text = user.AccountName;
                    ProfileInitialsLabel.Text = user.AccountName?.Length >= 2 ? user.AccountName.Substring(0, 2).ToUpper() : "U";
                    ProfileRoleLabel.Text = "Artisanal Collector";
                }
            }
            catch { }
        }

        private async Task LoadRewardsAsync()
        {
            var rewards = await _databaseService.GetCustomerRewardsAsync(GetCurrentUserId());
            if (rewards != null)
            {
                if (_pointsBalanceLabel != null)
                {
                    _pointsBalanceLabel.Text = rewards.PointsBalance.ToString("N0");
                }

                if (_tierNameLabel != null)
                {
                    _tierNameLabel.Text = rewards.Tier;
                }

                var remaining = Math.Max(0, NextTierThreshold - rewards.PointsBalance);
                if (_pointsToGoLabel != null)
                {
                    _pointsToGoLabel.Text = remaining == 0 ? "Tier reached" : $"{remaining:N0} pts to go";
                }

                if (_tierProgressBar != null)
                {
                    _tierProgressBar.Progress = Math.Min(1, rewards.PointsBalance / (double)NextTierThreshold);
                }

                if (_tierMessageLabel != null)
                {
                    _tierMessageLabel.Text = $"Unlock 20% off all commissions at {NextTierThreshold:N0} pts.";
                }
            }

            if (_rewardsCollectionView != null)
            {
                _rewardsCollectionView.ItemsSource = await _databaseService.GetRedeemableRewardsAsync();
            }

            if (_historyCollectionView != null)
            {
                _historyCollectionView.ItemsSource = await _databaseService.GetRewardHistoryAsync(GetCurrentUserId());
            }
        }

        private static int GetCurrentUserId()
        {
            return UserSession.AccountID != 0 ? UserSession.AccountID : CurrentUserId;
        }

        private async void OnRedeemNowClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Redeem", "Select a reward to redeem.", "OK");
        }

        private async void OnRedeemRewardClicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int rewardId)
            {
                await DisplayAlert("Redeem", $"Reward {rewardId} redemption requested.", "OK");
            }
        }

        private void BindUiReferences()
        {
            _pointsBalanceLabel = (Label)FindByName("PointsBalanceLabel");
            _tierNameLabel = (Label)FindByName("TierNameLabel");
            _pointsToGoLabel = (Label)FindByName("PointsToGoLabel");
            _tierProgressBar = (ProgressBar)FindByName("TierProgressBar");
            _tierMessageLabel = (Label)FindByName("TierMessageLabel");
            _rewardsCollectionView = (CollectionView)FindByName("RewardsCollectionView");
            _historyCollectionView = (CollectionView)FindByName("HistoryCollectionView");
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

        private async void OnFeedbackTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FeedbackPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
