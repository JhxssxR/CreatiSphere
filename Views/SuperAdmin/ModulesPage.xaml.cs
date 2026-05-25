using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;
using System.Linq;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class ModulesPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private System.Collections.Generic.List<SystemModule> _allModules = new();

        public ModulesPage()
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
            await LoadModules();
        }

        private async Task LoadModules()
        {
            try
            {
                _allModules = await _databaseService.GetSystemModulesAsync();
                ApplyFilters();

                TotalModulesLabel.Text = _allModules.Count.ToString();
                ActiveModulesLabel.Text = _allModules.Count(m => m.Status == "Active").ToString();
                BetaModulesLabel.Text = _allModules.Count(m => m.Status == "Beta").ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load modules: {ex.Message}", "OK");
            }
        }

        private void ApplyFilters()
        {
            string searchText = SearchEntry.Text ?? "";
            var filtered = _allModules.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(m => m.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            ModulesCollectionView.ItemsSource = filtered.ToList();
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnAddModuleClicked(object? sender, EventArgs e)
        {
            NewModuleNameEntry.Text = string.Empty;
            NewModuleStarterSwitch.IsToggled = true;
            NewModuleStandardSwitch.IsToggled = true;
            NewModuleEnterpriseSwitch.IsToggled = true;
            AddModuleModal.IsVisible = true;
        }

        private void OnCancelAddModuleClicked(object? sender, EventArgs e)
        {
            AddModuleModal.IsVisible = false;
        }

        private async void OnDeployModuleClicked(object? sender, EventArgs e)
        {
            string moduleName = NewModuleNameEntry.Text?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                await DisplayAlertAsync("Required", "Please enter a module name.", "OK");
                return;
            }

            var newModule = new SystemModule
            {
                Name = moduleName,
                Version = "v1.0.0",
                Status = "Active",
                EnabledCount = 0,
                IsStarterEnabled = NewModuleStarterSwitch.IsToggled,
                IsStandardEnabled = NewModuleStandardSwitch.IsToggled,
                IsEnterpriseEnabled = NewModuleEnterpriseSwitch.IsToggled
            };

            bool success = await _databaseService.AddSystemModuleAsync(newModule);

            if (success)
            {
                AddModuleModal.IsVisible = false;
                await DisplayAlertAsync("Deployment Success", $"The '{moduleName}' module has been successfully provisioned and is now live.", "OK");
                await LoadModules();
            }
            else
            {
                await DisplayAlertAsync("Deployment Error", "Failed to save the new module to the database.", "OK");
            }
        }

        private SystemModule? _selectedModule;
        private async void OnConfigureClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is SystemModule module)
            {
                _selectedModule = module;
                ConfigModalTitle.Text = $"Configure {module.Name}";
                StatusPicker.SelectedItem = module.Status;
                ConfigModal.IsVisible = true;
            }
        }

        private async void OnPermissionsClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is SystemModule module)
            {
                _selectedModule = module;
                PermissionsModalTitle.Text = $"Access Control: {module.Name}";
                
                StarterTierSwitch.IsToggled = module.IsStarterEnabled;
                StandardTierSwitch.IsToggled = module.IsStandardEnabled;
                EnterpriseTierSwitch.IsToggled = module.IsEnterpriseEnabled;
                
                PermissionsModal.IsVisible = true;
            }
        }

        private void OnCancelConfigClicked(object? sender, EventArgs e)
        {
            ConfigModal.IsVisible = false;
        }

        private async void OnSaveConfigClicked(object? sender, EventArgs e)
        {
            if (_selectedModule != null && StatusPicker.SelectedItem != null)
            {
                string newStatus = StatusPicker.SelectedItem.ToString() ?? "Active";
                bool success = await _databaseService.UpdateSystemModuleStatusAsync(_selectedModule.Name, newStatus);
                
                if (success)
                {
                    ConfigModal.IsVisible = false;
                    await LoadModules();
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to update module status.", "OK");
                }
            }
        }

        private void OnCancelPermissionsClicked(object? sender, EventArgs e)
        {
            PermissionsModal.IsVisible = false;
        }

        private async void OnSavePermissionsClicked(object? sender, EventArgs e)
        {
            if (_selectedModule != null)
            {
                bool starter = StarterTierSwitch.IsToggled;
                bool standard = StandardTierSwitch.IsToggled;
                bool enterprise = EnterpriseTierSwitch.IsToggled;

                bool success = await _databaseService.UpdateModulePermissionsAsync(_selectedModule.Name, starter, standard, enterprise);
                
                if (success)
                {
                    PermissionsModal.IsVisible = false;
                    await LoadModules();
                    await DisplayAlertAsync("Success", "Permissions updated successfully.", "OK");
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to update permissions.", "OK");
                }
            }
        }

        private async void OnDeleteModuleClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is SystemModule module)
            {
                bool confirm = await DisplayAlertAsync("Delete Module", $"Are you sure you want to permanently delete the '{module.Name}' module? This action cannot be undone.", "Delete", "Cancel");
                if (confirm)
                {
                    bool success = await _databaseService.DeleteSystemModuleAsync(module.Name);
                    if (success)
                    {
                        await LoadModules();
                    }
                    else
                    {
                        await DisplayAlertAsync("Error", "Failed to delete module from database.", "OK");
                    }
                }
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

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//UsersPage");
        }

        private async void OnMonitorSystemClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MonitorSystemPage");
        }

        private async void OnSystemReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ReportsPage");
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
