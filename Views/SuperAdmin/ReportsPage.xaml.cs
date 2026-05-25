using System;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class ReportsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public ReportsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUserData();
        }

        private void LoadUserData()
        {
            UserNameLabel.Text = UserSession.Username ?? "Super Admin";
            UserRoleLabel.Text = UserSession.IsSuperAdmin ? "SUPER ADMIN" : "ADMIN";
        }

        private async void OnNotificationClicked(object? sender, TappedEventArgs e)
        {
            NotificationBadge.IsVisible = false;
            await DisplayAlertAsync("Notifications", "• 3 new MSME registration requests\n• System audit completed successfully\n• Storage capacity reaching 85% on Node 2", "CLEAR ALL");
        }

        private async void OnProfileClicked(object? sender, TappedEventArgs e)
        {
            await DisplayAlertAsync("Account Settings", $"Logged in as: {UserSession.Username}\nEmail: {UserSession.Email}\nTier: {UserSession.Tier}", "MANAGE ACCOUNT");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportStats();
        }

        private async Task LoadReportStats()
        {
            try
            {
                var stats = await _databaseService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C0");
                NewCreatorsLabel.Text = stats.NewCreators.ToString("N0");
                ConversionRateLabel.Text = stats.ConversionRate.ToString("F1") + "%";
            }
            catch (Exception ex)
            {
                // Fallback or silent error for now
                System.Diagnostics.Debug.WriteLine("Error loading report stats: " + ex.Message);
            }
        }

        private async void OnGenerateReportClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("New Report", "System report generation initiated. You will be notified once the compilation is complete.", "OK");
        }

        private async void OnCompileParametersClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("System Note", "Report archives feature is currently disabled for this scope. Please use the System Reports dashboard for analytics.", "OK");
        }

        private void OnRenameReportClicked(object? sender, EventArgs e)
        {
            // Report Archives functionality disabled
        }

        private void OnCloseRenameModalClicked(object? sender, EventArgs e)
        {
            RenameModal.IsVisible = false;
        }

        private async void OnSaveRenameClicked(object? sender, EventArgs e)
        {
            RenameModal.IsVisible = false;
        }

        private async void OnDownloadReportClicked(object? sender, EventArgs e)
        {
            // Report Archives functionality disabled
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        private async void OnMsmeDirectoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MsmePage");
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//UsersPage");
        }

        private async void OnManageModulesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ModulesPage");
        }

        private async void OnMonitorSystemClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MonitorSystemPage");
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}
