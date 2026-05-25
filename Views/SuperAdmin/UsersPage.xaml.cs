using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class UsersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public UsersPage()
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _databaseService.GetUsersAsync();
                UsersCollectionView.ItemsSource = users;
                await LoadAdminStats();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load users: {ex.Message}", "OK");
            }
        }

        private async Task LoadAdminStats()
        {
            try
            {
                var stats = await _databaseService.GetAdminStatsAsync();
                TotalAdminsLabel.Text = stats.TotalAdmins.ToString();
                ActiveSessionsLabel.Text = stats.ActiveSessions.ToString();
                PendingRequestsLabel.Text = stats.PendingRequests.ToString();
                
                MonthlyGrowthLabel.Text = stats.MonthlyGrowth;
                PriorityStatusLabel.Text = stats.PriorityStatus;

                // Update priority color if critical
                PriorityStatusLabel.TextColor = stats.PriorityStatus == "Critical" ? Color.FromArgb("#EF4444") : Color.FromArgb("#E11D48");

                // Update progress bar width (max width is roughly 150 in the container)
                RoleDistributionBar.WidthRequest = 150 * stats.SuperAdminRatio;
            }
            catch { }
        }

        private void OnAddUserClicked(object? sender, EventArgs e)
        {
            AddUserModal.IsVisible = true;
        }

        private void OnCancelModalClicked(object? sender, EventArgs e)
        {
            AddUserModal.IsVisible = false;
        }

        private async void OnSaveUserClicked(object? sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NewUserUsername.Text) || 
                    string.IsNullOrWhiteSpace(NewUserEmail.Text) || 
                    NewUserRole.SelectedItem == null)
                {
                    await DisplayAlertAsync("Error", "Please fill in all required fields.", "OK");
                    return;
                }

                // Show loading state (optional, could add an ActivityIndicator to modal)
                
                bool success = await _databaseService.CreateAdminAccountAsync(
                    NewUserUsername.Text ?? "",
                    NewUserEmail.Text ?? "",
                    NewUserRole.SelectedItem?.ToString() ?? "Admin",
                    NewUserPassword.Text ?? "Password123");

                if (success)
                {
                    AddUserModal.IsVisible = false;
                    await DisplayAlertAsync("Success", $"Account for {NewUserUsername.Text} has been created successfully.", "OK");
                    
                    // Clear fields
                    NewUserUsername.Text = string.Empty;
                    NewUserEmail.Text = string.Empty;
                    NewUserRole.SelectedItem = null;
                    NewUserPassword.Text = "Password123";

                    // Refresh data
                    await LoadUsers();
                    await LoadAdminStats();
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to create account. Please check the database connection.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        private async void OnMsmeDirectoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MsmePage");
        }

        private async void OnManageModulesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ModulesPage");
        }

        private async void OnMonitorSystemClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MonitorSystemPage");
        }

        private async void OnSystemReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ReportsPage");
        }

        private async void OnUserOptionsClicked(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is User user)
            {
                string action = await DisplayActionSheetAsync($"Manage {user.Username}", "Cancel", null, 
                    user.Status == "Active" ? "Set Inactive" : "Set Active", "Reset Password", "Delete Account");

                if (action == "Set Inactive" || action == "Set Active")
                {
                    string newStatus = action == "Set Inactive" ? "Inactive" : "Active";
                    bool success = await _databaseService.UpdateUserStatusAsync(user.Email ?? "", newStatus);
                    if (success)
                    {
                        await LoadUsers();
                    }
                    else
                    {
                        await DisplayAlertAsync("Error", "Failed to update user status.", "OK");
                    }
                }
            }
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool answer = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }

        private async void OnFilterChanged(object? sender, EventArgs e)
        {
            if (RoleFilterPicker.SelectedItem == null) return;
            
            string selectedRole = RoleFilterPicker.SelectedItem.ToString() ?? "All Roles";
            var allUsers = await _databaseService.GetUsersAsync();
            
            if (selectedRole == "All Roles")
            {
                UsersCollectionView.ItemsSource = allUsers;
            }
            else
            {
                var filtered = allUsers.Where(u => u.Role == selectedRole).ToList();
                UsersCollectionView.ItemsSource = filtered;
            }
        }
    }
}
