using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Customer
{
    public partial class CustomerDashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private static readonly string[] HighlightImages = { "artist1.png", "artist2.png", "artist3.png", "redesign_bg.png" };
        private static readonly string[] ActivityIcons = { "artist1.png", "artist2.png", "artist3.png", "avatar.png" };

        public CustomerDashboardPage()
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
                var currentUserId = GetCurrentUserId();
                var rewards = await _databaseService.GetCustomerRewardsAsync(currentUserId);
                if (rewards != null)
                {
                    PointsLabel.Text = rewards.PointsBalance.ToString("N0");
                    TierLabel.Text = $"{rewards.Tier} Tier";
                }

                var commissions = await _databaseService.GetCommissionsAsync(currentUserId);
                EnsureCommissionImages(commissions);
                CommissionsCollectionView.ItemsSource = commissions;

                var activities = await _databaseService.GetUserActivityAsync(currentUserId);
                EnsureActivityIcons(activities);
                ActivityCollectionView.ItemsSource = activities;

                // Profile info
                var user = await _databaseService.GetAccountInfoByIdAsync(currentUserId);
                if (user != null)
                {
                    ProfileNameLabel.Text = user.AccountName;
                    ProfileInitialsLabel.Text = user.AccountName?.Length >= 2 ? user.AccountName.Substring(0, 2).ToUpper() : "U";
                    ProfileRoleLabel.Text = "Artisanal Collector"; // Role-specific logic can be added later
                    WelcomeLabel.Text = $"Welcome back, {user.AccountName}.";
                }
                else
                {
                    WelcomeLabel.Text = "Welcome back.";
                }

                WelcomeSubtitleLabel.Text = $"You have {commissions.Count} active commission{(commissions.Count == 1 ? string.Empty : "s")} and {activities.Count} recent update{(activities.Count == 1 ? string.Empty : "s")}.";
                ActiveCommissionsCountLabel.Text = commissions.Count.ToString();
                RecentUpdatesCountLabel.Text = activities.Count.ToString();

                HighlightsCollectionView.ItemsSource = BuildHighlights(commissions, activities, rewards);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load dashboard: {ex.Message}", "OK");
            }
        }

        private static int GetCurrentUserId()
        {
            return UserSession.AccountID != 0 ? UserSession.AccountID : 1002;
        }

        private static void EnsureCommissionImages(IList<Commission> commissions)
        {
            for (int i = 0; i < commissions.Count; i++)
            {
                var commission = commissions[i];
                if (string.IsNullOrWhiteSpace(commission.ImagePath) || commission.ImagePath.Equals("commission_placeholder.png", StringComparison.OrdinalIgnoreCase))
                {
                    commission.ImagePath = HighlightImages[i % HighlightImages.Length];
                }
            }
        }

        private static void EnsureActivityIcons(IList<Activity> activities)
        {
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities[i];
                if (string.IsNullOrWhiteSpace(activity.Icon) || !activity.Icon.Contains(".") || activity.Icon.Equals("Edit", StringComparison.OrdinalIgnoreCase))
                {
                    activity.Icon = ActivityIcons[i % ActivityIcons.Length];
                }
            }
        }

        private static List<HighlightCard> BuildHighlights(IList<Commission> commissions, IList<Activity> activities, Reward? rewards)
        {
            var highlights = new List<HighlightCard>();

            if (commissions.Count > 0)
            {
                var commission = commissions[0];
                highlights.Add(new HighlightCard
                {
                    Category = "Commission Spotlight",
                    Title = commission.Title,
                    ActionText = "View Brief",
                    ImagePath = commission.ImagePath
                });
            }

            if (rewards != null)
            {
                highlights.Add(new HighlightCard
                {
                    Category = "Rewards & Loyalty",
                    Title = $"{rewards.Tier} perks · {rewards.PointsBalance:N0} pts",
                    ActionText = "Redeem",
                    ImagePath = HighlightImages[1 % HighlightImages.Length]
                });
            }

            if (highlights.Count < 2 && activities.Count > 0)
            {
                var activity = activities[0];
                highlights.Add(new HighlightCard
                {
                    Category = "Activity Pulse",
                    Title = activity.Title,
                    ActionText = "View Update",
                    ImagePath = HighlightImages[2 % HighlightImages.Length]
                });
            }

            return highlights;
        }

        private async void OnBrowseTapped(object? sender, EventArgs e)
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

        private async void OnRewardsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RewardsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

    public class HighlightCard
    {
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
    }
}
