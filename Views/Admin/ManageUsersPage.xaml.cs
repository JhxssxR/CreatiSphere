using Microsoft.Maui.Controls;
using System;
using System.Linq;

namespace CreatiSphere.Views.Admin
{
    public partial class ManageUsersPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;
        private List<Services.User> _allUsers = new();
        private Services.User? _selectedUser;

        public ManageUsersPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ApplyTierRestrictions();
            await LoadUsers();
        }

        private void ApplyTierRestrictions()
        {
            // Set Header Badge
            TierLabel.Text = Services.UserSession.Tier.ToUpper();
            
            // Tier Colors
            switch (Services.UserSession.Tier)
            {
                case "Starter":
                    TierBadge.BackgroundColor = Color.FromArgb("#FEF2F2");
                    TierLabel.TextColor = Color.FromArgb("#B91C1C");
                    break;
                case "Standard":
                    TierBadge.BackgroundColor = Color.FromArgb("#DBEAFE");
                    TierLabel.TextColor = Color.FromArgb("#1E40AF");
                    break;
                case "Enterprise Plus":
                    TierBadge.BackgroundColor = Color.FromArgb("#F0FDF4");
                    TierLabel.TextColor = Color.FromArgb("#15803D");
                    break;
            }


        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _dbService.GetUsersAsync();
                
                if (Services.UserSession.AccountID > 5)
                {
                    users = new List<Services.User>();
                }
                
                _allUsers = users;
                UpdateList(users);

                if (!string.IsNullOrEmpty(Services.UserSession.Username) && Services.UserSession.Username.Length >= 2)
                    HeaderProfileInitials.Text = Services.UserSession.Username.Substring(0, 2).ToUpper();
                else
                    HeaderProfileInitials.Text = "AD";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load users: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//AdminDashboard", false);
        }

        private async void OnManageInventoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageInventoryPage", false);
        }

        private async void OnDigitalAssetManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage", false);
        }

        private async void OnManageProductsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageProductsPage", false);
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            // Already on this page
        }

        private async void OnCrmManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//CrmManagementPage", false);
        }

        private async void OnCustomOrderManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrderManagementPage", false);
        }

        private async void OnViewSalesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage", false);
        }

        private async void OnBusinessReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//BusinessReportsPage", false);
        }

        private void OnEditStatusClicked(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is Services.User user)
            {
                _selectedUser = user;
                ManageUserTitleLabel.Text = $"Manage {user.Username}";
                
                string currentStatus = user.Status ?? "Active";
                ToggleStatusActionLabel.Text = currentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "Set Inactive" : "Set Active";
                
                ManageUserModal.IsVisible = true;
            }
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }

        private void UpdateList(List<Services.User> users)
        {
            BindableLayout.SetItemsSource(UsersList, users);
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnFilterChanged(object? sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string search = SearchEntry.Text?.ToLower() ?? "";
            string role = RoleFilterPicker.SelectedItem?.ToString() ?? "All Roles";
            string status = StatusFilterPicker.SelectedItem?.ToString() ?? "Any Status";

            var filtered = _allUsers.Where(u => 
                (string.IsNullOrWhiteSpace(search) || 
                 (u.Username?.ToLower().Contains(search) ?? false) || 
                 (u.Email?.ToLower().Contains(search) ?? false)) &&
                (role == "All Roles" || u.Role == role) &&
                (status == "Any Status" || u.Status == status)
            ).ToList();

            UpdateList(filtered);
        }

        private void OnDeleteUserClicked(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is Services.User user)
            {
                _selectedUser = user;
                ConfirmTitle.Text = "Delete User";
                ConfirmMessage.Text = $"Are you sure you want to permanently delete {user.Username}? This action cannot be undone.";
                ConfirmBtn.Text = "Delete";
                ConfirmBtn.BackgroundColor = Color.FromArgb("#EF4444");
                ConfirmIconContainer.BackgroundColor = Color.FromArgb("#FEF2F2");
                ConfirmIcon.Fill = Color.FromArgb("#EF4444");
                ConfirmationModal.IsVisible = true;
            }
        }

        private void OnAddUserClicked(object? sender, EventArgs e)
        {
            var allowedRoles = new List<string> { "Customer", "Creator" };
            if (Services.UserSession.Tier == "Standard")
            {
                allowedRoles.Add("Sales");
                allowedRoles.Add("Finance");
            }
            else if (Services.UserSession.Tier == "Enterprise Plus")
            {
                allowedRoles.Add("Sales");
                allowedRoles.Add("Finance");
                allowedRoles.Add("Admin");
                allowedRoles.Add("Super Admin");
            }
            NewUserRolePicker.ItemsSource = allowedRoles;
            AddUserModal.IsVisible = true;
        }

        private void OnCancelClicked(object? sender, EventArgs e)
        {
            AddUserModal.IsVisible = false;
            ClearForm();
        }

        private void OnCancelManageClicked(object? sender, EventArgs e)
        {
            ManageUserModal.IsVisible = false;
            _selectedUser = null;
        }

        private async void OnToggleStatusActionClicked(object? sender, EventArgs e)
        {
            if (_selectedUser != null)
            {
                string currentStatus = _selectedUser.Status ?? "Active";
                string newStatus = currentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
                
                bool success = await _dbService.UpdateUserStatusAsync(_selectedUser.Email ?? "", newStatus);
                if (success)
                {
                    ManageUserModal.IsVisible = false;
                    await LoadUsers();
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to update user status.", "OK");
                }
            }
        }

        private async void OnResetPasswordActionClicked(object? sender, EventArgs e)
        {
            if (_selectedUser != null)
            {
                bool confirm = await DisplayAlertAsync("Reset Password", $"Are you sure you want to reset the password for {_selectedUser.Username}? A temporary password will be generated.", "Reset", "Cancel");
                if (confirm)
                {
                    // Simulated password reset
                    await DisplayAlertAsync("Success", "A new temporary password has been sent to " + _selectedUser.Email, "OK");
                    ManageUserModal.IsVisible = false;
                }
            }
        }

        private void OnDeleteAccountActionClicked(object? sender, EventArgs e)
        {
            if (_selectedUser != null)
            {
                ManageUserModal.IsVisible = false;
                ConfirmTitle.Text = "Delete Account";
                ConfirmMessage.Text = $"Are you sure you want to PERMANENTLY delete {_selectedUser.Username}? This action cannot be undone.";
                ConfirmBtn.Text = "Delete Account";
                ConfirmBtn.BackgroundColor = Color.FromArgb("#EF4444");
                ConfirmIconContainer.BackgroundColor = Color.FromArgb("#FEF2F2");
                ConfirmIcon.Fill = Color.FromArgb("#EF4444");
                ConfirmationModal.IsVisible = true;
            }
        }

        private void OnModalContentClicked(object? sender, EventArgs e)
        {
            // Do nothing, just prevents the modal from closing when clicking inside
        }

        private void ClearForm()
        {
            NewUserUsernameEntry.Text = string.Empty;
            NewUserEmailEntry.Text = string.Empty;
            NewUserPasswordEntry.Text = string.Empty;
            NewUserRolePicker.SelectedIndex = -1;
        }

        private async void OnDownloadClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Download", "User directory export started. Your download will begin shortly.", "OK");
        }

        private async void OnCreateAccountClicked(object? sender, EventArgs e)
        {
            string name = NewUserUsernameEntry.Text;
            string email = NewUserEmailEntry.Text;
            string role = NewUserRolePicker.SelectedItem?.ToString() ?? "";
            string password = NewUserPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlertAsync("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (_allUsers.Count >= Services.UserSession.MaxUsers)
            {
                await DisplayAlertAsync("Tier Limit", "You have reached your tier's user limit. Please upgrade to add more administrators.", "OK");
                return;
            }

            bool success = await _dbService.CreateAdminAccountAsync(name, email, role, password);
            if (success)
            {
                await DisplayAlertAsync("Success", "Administrator account created successfully.", "OK");
                AddUserModal.IsVisible = false;
                ClearForm();
                await LoadUsers();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to create account. Please try again.", "OK");
            }
        }

        private void OnCancelConfirmClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
        }

        private async void OnFinalConfirmClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
            if (_selectedUser != null)
            {
                // Logic based on what we are confirming
                if (ConfirmTitle.Text.Contains("Delete"))
                {
                    bool success = await _dbService.DeleteUserAsync(_selectedUser.Email ?? "");
                    if (success)
                    {
                        await LoadUsers();
                    }
                    else
                    {
                        await DisplayAlertAsync("Error", "Failed to delete account.", "OK");
                    }
                }
            }
            _selectedUser = null;
        }
    }
}

